using UnityEngine;
using System.Collections.Generic;
using RogoDigital.Lipsync;
using System;

namespace Crosstales.RTVoice
{
	public class LipSyncPhenomeGenerator
	{
		public delegate void OnPhenomeGenerationSuccess(AudioClip audioClip, List<PhonemeMarker> markers);
		public delegate void OnPhenomeGenerationFail(string Error);

		public static OnPhenomeGenerationSuccess onPhenomeGenerateSuccess;
		public static OnPhenomeGenerationFail onPhenomeGenerateFail;

		//place generated audioclip in output channel
		public AudioSource outputChannel;
		public bool autoplay;

		/// <summary>
		/// Generates Phenome set based on the audio clip.
		/// </summary>
		/// <param name="pInputAudio"></param>
		public static void GeneratePhenomes(AudioClip pInputAudio) {
			if (pInputAudio == null) {
				Debug.LogError("Cannot generate phenomes, inputAudio is NULL!");
				return;
			}

			Debug.Log("Generating Phenomes");
			AutoSyncRuntime.AutoSyncOptions audioOptions = new AutoSyncRuntime.AutoSyncOptions("English (US)", true, AutoSyncRuntime.AutoSyncOptionsPreset.Default);
			AutoSyncRuntime.ProcessAudio(pInputAudio, SuccessCallback, FailedCallback, audioOptions, "D:/Program Files (x86)/sox-14-4-2/sox.exe");
		}

		private static void SuccessCallback(AudioClip pAudioClip, List<PhonemeMarker> pMarkers) {
			Debug.Log("Phenomes for " + pAudioClip.name + " has been generated!");
			onPhenomeGenerateSuccess.Invoke(pAudioClip, pMarkers);
		}

		private static void FailedCallback(string pError) {
			onPhenomeGenerateFail.Invoke(pError);
			Debug.LogError(pError);
		}
	}
}