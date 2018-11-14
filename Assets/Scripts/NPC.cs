using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using RogoDigital.Lipsync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
	[Header("NPC Info")]
	public string npcName = "Default-NPC";

	[Header("AI")]
	public float playerInteractionRange = 3f;

	private Player _player;
	private ClientScript _client;
	private LipSync _lipSync;
	private AudioSource _audioSource;

	private bool _processingMessage;

	private List<ServerPackage> _messageQueue = new List<ServerPackage>();

	private void Start() {
		NPCManager.Instance.RegisterNPC(this);

		_player = Player.Instance;

		_client = GetComponent<ClientScript>();
		_lipSync = GetComponent<LipSync>();
		_audioSource = GetComponent<AudioSource>();

		//_client.onMessageReceived += OnMessageReceived;
		//_client.OpenServerChannel();

		Speaker.OnSpeakAudioGenerationComplete += (pModel) => StartCoroutine(LoadAudioRoutine(pModel));
		LipSyncRuntimeGenerator.onPhonemeGenerateSuccess += OnPhonemeGenerationComplete;
		LipSyncRuntimeGenerator.onPhonemeGenerateFail += OnPhonemeGenerationFail;
	}

	public void SendUserMessage(string pMessage) {
		if (string.IsNullOrEmpty(pMessage) || string.IsNullOrWhiteSpace(pMessage))
			return;

		if (_client.IsConnected) {
			Debug.Log("Sending message: " + pMessage);
			_client.SendUserMessage(pMessage);
		}

		//FOR DEBUGGING ONLY
		else {
			ServerPackage package = new ServerPackage {
				text = pMessage
			};
			OnMessageReceived(package);
		}
		//
	}

	public void Interact() {
		Vector3 localEuler = transform.eulerAngles;
		transform.LookAt(_player.transform);
		transform.eulerAngles = new Vector3(localEuler.x, transform.eulerAngles.y, localEuler.z);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Semicolon)) {
			ServerPackage p = new ServerPackage {
				text = "This is an example for the lipsync generation in runtime."
			};
			OnMessageReceived(p);
		}

		UpdateMessageQueue();
	}

	/// <summary>
	/// This queue exists to make sure the NPC is not overriding its own spoken lines.
	/// </summary>
	private void UpdateMessageQueue() {
		if (_processingMessage || (_lipSync.IsPlaying && _audioSource.isPlaying))
			return;

		if (_messageQueue.Count > 0) {
			Debug.LogWarning("Dequeuing ServerPackage");
			GenerateLipSync(_messageQueue[0]);
			_messageQueue.RemoveAt(0);
		}
	}

	#region LipSync/Audio Generation
	private IEnumerator LoadAudioRoutine(Wrapper pWrapper) {
		Debug.Log("Audio generation complete");
		string filePath = pWrapper.OutputFile;

		if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || Path.GetFileNameWithoutExtension(filePath).IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
			Debug.LogError("AudioClip loading failed. Audio path or filename contained invalid characters.");
			yield return null;
		}

		Debug.Log("Loading generated audio file from: " + filePath);

		WWW www = new WWW("file://" + filePath);
		AudioClip audioClip = www.GetAudioClip(true, true);

		//wait until the audio file has been loaded.
		yield return new WaitUntil(() => audioClip.loadState == AudioDataLoadState.Loaded);

		Debug.Log("Audio file has been loaded");

		LipSyncRuntimeGenerator.GeneratePhonemes(audioClip);
		yield return null;
	}

	private void GenerateLipSync(ServerPackage pPackage) {
		_processingMessage = true;
		LipSyncRuntimeGenerator.GenerateAudioFile(pPackage.text, npcName);
	}

	private void OnMessageReceived(ServerPackage pPackage) {
		if (!(_lipSync.IsPlaying && _audioSource.isPlaying)) {
			GenerateLipSync(pPackage);
			return;
		}

		Debug.LogWarning("NPC is busy, adding ServerPackage to queue");
		_messageQueue.Add(pPackage);
	}

	private void OnPhonemeGenerationFail(string pError) {
		Debug.LogError(pError);
	}

	private void OnPhonemeGenerationComplete(AudioClip pAudioClip, List<PhonemeMarker> pMarkers) {
		LipSyncData data = LipSyncRuntimeManager.Instance.lipSyncData;
		data.clip = pAudioClip;
		data.phonemeData = pMarkers.ToArray();

		Debug.Log("Playing LipSync data");
		_lipSync.Play(data);
		_audioSource.clip = pAudioClip;
		_audioSource.Play();
		_processingMessage = false;
	}
	#endregion

	private float GetRandomFloat(int MinValue, int MaxValue) {
		return ((float)new System.Random().Next(MinValue, MaxValue));
	}

	public bool InInteractRange {
		get {
			return Vector3.Distance(transform.position, _player.transform.position) < playerInteractionRange;
		}
	}
}