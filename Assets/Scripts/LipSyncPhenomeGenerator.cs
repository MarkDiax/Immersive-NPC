using UnityEngine;
using UnityEditor;
using RogoDigital.Lipsync;
using Crosstales.RTVoice;
using System;
using System.IO;
using System.Collections.Generic;

public class LipSyncPhenomeGenerator : MonoBehaviour
{
	//generate phenomes from this clip
	public AudioClip inputClip;

	//place generated audioclip in output channel
	public AudioSource outputChannel;
	public bool autoplay;

	public void GeneratePhenomes(AudioClip pInputAudio) {
	}
}
