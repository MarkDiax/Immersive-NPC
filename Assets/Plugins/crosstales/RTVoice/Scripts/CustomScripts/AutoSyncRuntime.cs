using UnityEngine;

using System.IO;
using System.Collections.Generic;

using RogoDigital.Lipsync;
using System;
using UnityEditor;
using RogoDigital;
using System.Collections;

namespace Crosstales.RTVoice
{
	public class AutoSyncRuntime
	{
		private static List<PhonemeMarker>[] tempData;
		private static AudioClip tempClip;
		private static AutoSyncOptions tempOptions;
		private static AutoSyncDataReadyDelegate tempDelegate;
		private static AutoSyncFailedDelegate tempFailDelegate;
		private static string[] tempPaths;

		private static int multiFileIndex = 0;
		private static float multiFileLength = 0;
		private static float multiFileOffset = 0;

		private static string savedSoxPath;

		public static string audioPath;

		public static void SetAudioPath(string pAudioPath) {
			audioPath = pAudioPath;
		}

		/// <summary>
		/// Begin processing an audioclip. Phoneme data will be passed along with the input AudioClip to the AutoSyncDataReadyDelegate callback. 
		/// </summary>
		/// <param name="clip">AudioClip to be processed.</param>
		/// <param name="languageModel">Name of a language model present in the project.</param>
		/// <param name="	dataReadyCallback">Method that will receive the results of the process.</param>
		/// <param name="progressPrefix">Prefix shown on the progress bar.</param>
		/// <param name="enableConversion">If true, audio files will be temporarily converted if possible to maximise compatibility.</param>
		public static void ProcessAudio(AudioClip clip, AutoSyncDataReadyDelegate dataReadyCallback, AutoSyncFailedDelegate failedCallback, string progressPrefix, AutoSyncOptions options, string soxPath = null) {
			if (clip == null) {
				Debug.LogError("AudioClip to process is NULL!");
				return;
			}

			if (!string.IsNullOrEmpty(soxPath)) {
				savedSoxPath = soxPath;
			}
			else {
				if (string.IsNullOrEmpty(savedSoxPath)) {
					Debug.LogError("Sox Path is empty!");
					return;
				}
				soxPath = savedSoxPath;
			}

			bool converted = false;

			if (audioPath != null) {
				Debug.Log("Generating LipSync phonemes from: " + audioPath);

				// Check Path
				if (audioPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || Path.GetFileNameWithoutExtension(audioPath).IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
					failedCallback.Invoke("AutoSync failed. Audio path or filename contained invalid characters.");
					return;
				}

				bool failed = false;
				// Convert to acceptable format
				if (options.useAudioConversion) {
					converted = true;

					string tempFileDir = LipSyncRuntimeManager.Instance.StreamingAssetsAudioPath + "Temp/";
					if (Directory.Exists(tempFileDir))
						Directory.Delete(tempFileDir, true);
					Directory.CreateDirectory(tempFileDir);

					string newAudioPath = tempFileDir + Path.GetFileNameWithoutExtension(audioPath) + "_temp_converted.wav";

					// Convert to compatible .wav file
					string soXArgs = "\"" + audioPath + "\" -c 1 -b 16 -e s -r 16k \"" + newAudioPath + "\"";
					audioPath = newAudioPath;

					Debug.Log("Creating temporary file: " + audioPath);

					System.Diagnostics.Process process = new System.Diagnostics.Process();
					process.StartInfo.FileName = savedSoxPath;
					process.StartInfo.Arguments = soXArgs;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.RedirectStandardError = true;

					process.ErrorDataReceived += (object e, System.Diagnostics.DataReceivedEventArgs outLine) => {
						if (!string.IsNullOrEmpty(outLine.Data)) {
							if (outLine.Data.Contains("FAIL")) {
								failed = true;
								converted = false;
								process.Close();
								failedCallback.Invoke("AutoSync: SoX Conversion Failed: " + outLine.Data);
							}
						}
					};

					process.Start();
					process.BeginErrorReadLine();
					process.WaitForExit(5000);
				}

				if (!File.Exists(audioPath) || failed)
					return;

				// Load Language Model
				AutoSyncRTLanguageModel model = AutoSyncRTLanguageModel.Load(options.languageModel);
				if (model == null) {
					if (converted) {
						if (File.Exists(audioPath)) {
							File.Delete(audioPath);
						}
					}
					failedCallback.Invoke("AutoSync Failed: Language Model was not loaded.");
					return;
				}
				string basePath = model.GetBasePath();

				List<string> args = new List<string>();
				args.Add("-infile"); args.Add(audioPath);
				args.Add("-hmm"); args.Add(basePath + model.hmmDir);
				args.Add("-allphone"); args.Add(basePath + model.allphoneFile);
				if (options.allphone_ciEnabled) { args.Add("-allphone_ci"); args.Add("yes"); }
				if (options.backtraceEnabled) { args.Add("-backtrace"); args.Add("yes"); }
				args.Add("-time"); args.Add("yes");
				args.Add("-beam"); args.Add("1e" + options.beamExponent);
				args.Add("-pbeam"); args.Add("1e" + options.pbeamExponent);
				args.Add("-lw"); args.Add(options.lwValue.ToString());

				SphinxWrapper.Recognize(args.ToArray());

				ContinuationManager.Add(() => SphinxWrapper.isFinished, () => {
					if (SphinxWrapper.error != null) {
						failedCallback.Invoke("AutoSync Failed.");
						return;
					}

					List<PhonemeMarker> data = ParseOutput(
							SphinxWrapper.result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries),
							model,
							clip
						);


					if (options.useAudioConversion) data = CleanupOutput(data, options.cleanupAggression);

					dataReadyCallback.Invoke(
						clip,
						data
					);

					if (converted) {
						if (File.Exists(audioPath)) {
							File.Delete(audioPath);
						}
					}
				});
			}
		}

		/// <summary>
		/// Begin processing an audioclip. Phoneme data will be passed along with the input AudioClip to the AutoSyncDataReadyDelegate callback. 
		/// </summary>
		/// <param name="clip">AudioClip to be processed.</param>
		/// <param name="languageModel">Name of a language model present in the project.</param>
		/// <param name="callback">Method that will receive the results of the process.</param>
		/// <param name="enableConversion">If true, audio files will be temporarily converted if possible to maximise compatibility.</param>
		public static void ProcessAudio(AudioClip clip, AutoSyncDataReadyDelegate callback, AutoSyncFailedDelegate failedCallback, AutoSyncOptions options, string soxPath = null) {
			ProcessAudio(clip, callback, failedCallback, "", options, soxPath);
		}

		public static List<PhonemeMarker> CleanupOutput(List<PhonemeMarker> data, float aggressiveness) {
			List<PhonemeMarker> output = new List<PhonemeMarker>(data);
			List<bool> markedForDeletion = new List<bool>();
			output.Sort(LipSync.SortTime);

			for (int m = 0; m < data.Count; m++) {
				if (m > 0) {
					if (data[m].time - data[m - 1].time < aggressiveness && !markedForDeletion[m - 1]) {
						markedForDeletion.Add(true);
					}
					else {
						markedForDeletion.Add(false);
					}
				}
				else {
					markedForDeletion.Add(false);
				}
			}

			for (int m = 0; m < markedForDeletion.Count; m++) {
				if (markedForDeletion[m]) {
					output.Remove(data[m]);
				}
			}

			return output;
		}

		private static List<PhonemeMarker> ParseOutput(string[] lines, AutoSyncRTLanguageModel lm, AudioClip clip) {
			List<PhonemeMarker> results = new List<PhonemeMarker>();

			Dictionary<string, string> phonemeMapper = new Dictionary<string, string>();

			LipSyncProject settings = LipSyncRuntimeManager.Instance.projectSettings;

			if (lm.phonemeMapper.Length == 0) {
				// Default Phoneme Mapper
				phonemeMapper = new Dictionary<string, string>() {
					// Vowels
					{"IY"          , "E"},
					{"IH"          , "AI"},
					{"EH"          , "E"},
					{"AE"          , "AI"},
					{"AH"          , "U"},
					{"UW"          , "O"},
					{"UH"          , "U"},
					{"AA"          , "AI"},
					{"AO"          , "AI"},
					{"EY"          , "AI"},
					{"AY"          , "AI"},
					{"OY"          , "O"},
					{"AW"          , "AI"},
					{"OW"          , "O"},
					{"ER"          , "U"},

					// Consonants
					{"JH"          , "CDGKNRSThYZ"},
					{"L"           , "L"},
					{"R"           , "CDGKNRSThYZ"},
					{"Y"           , "CDGKNRSThYZ"},
					{"W"           , "WQ"},
					{"M"           , "MBP"},
					{"N"           , "CDGKNRSThYZ"},
					{"NG"          , "CDGKNRSThYZ"},
					{"CH"          , "CDGKNRSThYZ"},
					{"J"           , "CDGKNRSThYZ"},
					{"DH"          , "CDGKNRSThYZ"},
					{"B"           , "MBP"},
					{"D"           , "CDGKNRSThYZ"},
					{"G"           , "CDGKNRSThYZ"},
					{"P"           , "MBP"},
					{"T"           , "CDGKNRSThYZ"},
					{"K"           , "CDGKNRSThYZ"},
					{"Z"           , "CDGKNRSThYZ"},
					{"ZH"          , "CDGKNRSThYZ"},
					{"V"           , "FV"},
					{"F"           , "FV"},
					{"TH"          , "CDGKNRSThYZ"},
					{"S"           , "CDGKNRSThYZ"},
					{"SH"          , "CDGKNRSThYZ"},
					{"HH"          , "CDGKNRSThYZ"},
				};
			}
			else {
				// LM Phoneme Mapper
				foreach (AutoSyncRTLanguageModel.PhonemeMapping mapping in lm.phonemeMapper) {
					phonemeMapper.Add(mapping.label, mapping.phonemeName);
				}
			}

			foreach (string line in lines) {
				if (string.IsNullOrEmpty(line))
					break;
				string[] tokens = line.Split(' ');

				try {
					if (tokens[0] != "SIL") {
						string phonemeName = phonemeMapper[tokens[0]];
						float startTime = float.Parse(tokens[1]) / clip.length;

						bool found = false;
						int phoneme;
						for (phoneme = 0; phoneme < settings.phonemeSet.phonemes.Length; phoneme++) {
							if (settings.phonemeSet.phonemes[phoneme].name == phonemeName) {
								found = true;
								break;
							}
						}

						if (found) {
							results.Add(new PhonemeMarker(phoneme, startTime));
						}
						else {
							Debug.LogWarning("Phoneme mapper returned '" + phonemeName + "' but this phoneme does not exist in the current set. Skipping this entry.");
						}
					}
				}
				catch (ArgumentOutOfRangeException) {
					Debug.LogWarning("Phoneme Label missing from return data. Skipping this entry.");
				}
				catch (KeyNotFoundException) {
					Debug.LogWarning("Phoneme Label '" + tokens[0] + "' not found in phoneme mapper. Skipping this entry.");
				}
			}

			return results;
		}

		public delegate void AutoSyncDataReadyDelegate(AudioClip clip, List<PhonemeMarker> markers);
		public delegate void AutoSyncFailedDelegate(string error);
		[Obsolete("Use AutoSyncDataReadyDelegate instead.")]
		public delegate void AutoSyncDataReady(AudioClip clip, List<PhonemeMarker> markers);

		public enum AutoSyncOptionsPreset
		{
			Default,
			HighQuality,
		}

		public struct AutoSyncOptions
		{
			public string languageModel;
			public bool useAudioConversion;
			public bool allphone_ciEnabled;
			public bool backtraceEnabled;
			public int beamExponent;
			public int pbeamExponent;
			public float lwValue;
			public bool doCleanup;
			public float cleanupAggression;

			public AutoSyncOptions(string languageModel, bool useAudioConversion, bool allphone_ciEnabled, bool backtraceEnabled, int beamExponent, int pbeamExponent, float lwValue, bool doCleanup, float cleanupAggression) {
				this.languageModel = languageModel;
				this.useAudioConversion = useAudioConversion;
				this.allphone_ciEnabled = allphone_ciEnabled;
				this.backtraceEnabled = backtraceEnabled;
				this.beamExponent = beamExponent;
				this.pbeamExponent = pbeamExponent;
				this.lwValue = lwValue;
				this.doCleanup = doCleanup;
				this.cleanupAggression = cleanupAggression;
			}

			public AutoSyncOptions(string languageModel, bool useAudioConversion, AutoSyncOptionsPreset preset) {
				this.languageModel = languageModel;
				this.useAudioConversion = useAudioConversion;

				if (preset == AutoSyncOptionsPreset.HighQuality) {
					this.allphone_ciEnabled = false;
					this.backtraceEnabled = true;
					this.beamExponent = -40;
					this.pbeamExponent = -40;
					this.lwValue = 15f;
					this.doCleanup = true;
					this.cleanupAggression = 0.003f;
				}
				else {
					this.allphone_ciEnabled = true;
					this.backtraceEnabled = false;
					this.beamExponent = -20;
					this.pbeamExponent = -20;
					this.lwValue = 2.5f;
					this.doCleanup = false;
					this.cleanupAggression = 0;
				}
			}
		}
	}
}