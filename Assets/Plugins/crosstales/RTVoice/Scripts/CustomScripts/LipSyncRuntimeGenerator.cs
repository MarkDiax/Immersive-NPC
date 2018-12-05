using UnityEngine;
using System.Collections.Generic;
using RogoDigital.Lipsync;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml;

namespace RogoDigital.Lipsync
{
	public class LipSyncRuntimeGenerator
	{
		public delegate void OnPhonemeGenerationSuccess(AudioClip audioClip, List<PhonemeMarker> markers);
		public delegate void OnPhonemeGenerationFail(string Error);
		public delegate void OnSpeakerAudioGenerated(Wrapper pWrapper);

		public static OnPhonemeGenerationSuccess onPhonemeGenerateSuccess;
		public static OnPhonemeGenerationFail onPhonemeGenerateFail;
		public static OnSpeakerAudioGenerated onSpeakerAudioGenerated;

		private static int _audioFileIndex;
		private static string _audioFileName;

		/// <summary>
		/// Generates Phenome set based on the audio clip.
		/// </summary>
		/// <param name="pInputAudio"></param>
		public static void GeneratePhonemes(AudioClip pInputAudio) {
			if (pInputAudio == null) {
				Debug.LogError("Cannot generate phenomes, inputAudio is NULL!");
				return;
			}

			AutoSyncRuntime.AutoSyncOptions audioOptions = new AutoSyncRuntime.AutoSyncOptions("English (US)", true, AutoSyncRuntime.AutoSyncOptionsPreset.Default);
			string audioFilePath = LipSyncRuntimeManager.Instance.StreamingAssetsAudioPath + _audioFileName + ".wav";

			AutoSyncRuntime.SetAudioPath(audioFilePath);
			AutoSyncRuntime.ProcessAudio(pInputAudio, SuccessCallback, FailedCallback, audioOptions, "D:/Program Files (x86)/sox-14-4-2/sox.exe");
		}

		private static void SuccessCallback(AudioClip pAudioClip, List<PhonemeMarker> pMarkers) {
			Debug.Log("Phonemes have been sucessfully generated!");
			onPhonemeGenerateSuccess.Invoke(pAudioClip, pMarkers);
		}

		private static void FailedCallback(string pError) {
			onPhonemeGenerateFail.Invoke(pError);
			Debug.LogError(pError);
		}

		public static void GenerateAudioFile(string pTextToGenerateFrom, string pFilePrefix) {
			string fileDir = LipSyncRuntimeManager.Instance.StreamingAssetsAudioPath;

			if (!Directory.Exists(fileDir))
				Directory.CreateDirectory(fileDir);

			_audioFileName = pFilePrefix + "_VoiceLine" + _audioFileIndex;
			Debug.Log("Generating TTS audio file for text: " + pTextToGenerateFrom +
				"\n as: " + fileDir + _audioFileName);

			string filename = Application.streamingAssetsPath + "/XML/MaryTTS.xml";
			XmlTextWriter xmlWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("maryxml");
			xmlWriter.WriteAttributeString("xmlns", "http://mary.dfki.de/2002/MaryXML");
			xmlWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
			xmlWriter.WriteAttributeString("version", "0.5");
			xmlWriter.WriteAttributeString("xml:lang", "en-GB");

			xmlWriter.WriteStartElement("prosody");
			xmlWriter.WriteAttributeString("rate", "-15%");
			xmlWriter.WriteAttributeString("pitch", "-40%");
			xmlWriter.WriteAttributeString("accent-prominence", "90%");
			xmlWriter.WriteAttributeString("vowel-duration", "80%");

			xmlWriter.WriteString(pTextToGenerateFrom);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			xmlWriter.Close();

			XDocument xmlDoc = XDocument.Load(Application.streamingAssetsPath + "/XML/MaryTTS.xml");

			Speaker.Generate(xmlDoc.ToString(), fileDir + _audioFileName, Speaker.VoiceForName(LipSyncRuntimeManager.Instance.rtVoiceModel));
			_audioFileIndex++;
		}
	}
}