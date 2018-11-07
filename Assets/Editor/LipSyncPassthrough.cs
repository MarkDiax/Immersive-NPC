using UnityEngine;
using RogoDigital.Lipsync;
using Crosstales.RTVoice;
using System.Collections.Generic;

public class LipSyncPassthrough : MonoBehaviour
{
	private bool _isLoadingAudio;
	private AudioClip _audioClip;
	private LipSync _lipSync;
	private ClientScript _client;
	private Crosstales.RTVoice.Model.Wrapper _currentVoiceModel;

	public void Awake() {
		_client = GetComponent<ClientScript>();
		_lipSync = _client.GetComponent<LipSync>();

		Speaker.OnSpeakAudioGenerationComplete += OnTTSGenerated;
	}

	private void OnTTSGenerated(Crosstales.RTVoice.Model.Wrapper pWrapper) {
		_currentVoiceModel = pWrapper;

		string filePath = "D:/Projects/_unity/Immersive-NPC/" + _currentVoiceModel.OutputFile;
		Debug.Log("Loading from: " + filePath);
		WWW www = new WWW("file://" + filePath);

		_audioClip = www.GetAudioClip(true, true);
		GeneratePhenomes(_audioClip);
	}

	public void GeneratePhenomes(AudioClip inputAudio) {
		Debug.Log("Generating Phenomes");

		AutoSync.AutoSyncOptions audioOptions = new AutoSync.AutoSyncOptions("English (US)", true, AutoSync.AutoSyncOptionsPreset.Default);

		if (inputAudio == null)
			Debug.LogError("Cannot generate phenomes, inputAudio is NULL!");

		AutoSync.ProcessAudio(inputAudio, ReadyTest, FailedTest, audioOptions);
	}

	private void ReadyTest(AudioClip clip, List<PhonemeMarker> markers) {
		Debug.Log("Phenomes for " + clip + " has been generated!");

		LipSyncData data = _client.lipdata;// CreateInstance<LipSyncData>();
		data.clip = clip;
		data.phonemeData = markers.ToArray();
		//data.emotionData = new EmotionMarker[0];
		//data.gestureData = new GestureMarker[0];

		Debug.Log("Playing LipSync data");
		_lipSync.Play(data);
		_client.audioSource.clip = clip;
		_client.audioSource.Play();
		//Speaker.SpeakWithUID(_currentVoiceModel);
	}

	private void FailedTest(string pError) {
		Debug.LogError(pError);
	}
}