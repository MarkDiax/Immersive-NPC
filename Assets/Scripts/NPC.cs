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

	[Header("UI")]
	public InteractionPrompt interactionPrompt;
	public TextMesh dialogueText;

	[Header("AI")]
	public float rotationSpeed = 20;
	public readonly float playerInteractionRange = 2f;

	private Player _player;
	private ClientScript _client;
	private LipSync _lipSync;
	private AudioSource _audioSource;
	private string _currentMessage;
	private bool _isInteracting = false;
	private Coroutine _interactRoutine;

	private void Start() {
		NPCManager.Instance.RegisterNPC(this);

		_player = Player.Instance;
		_player.onInteractStop.AddListener(StopInteraction);

		_client = GetComponent<ClientScript>();
		_lipSync = GetComponent<LipSync>();
		_audioSource = GetComponent<AudioSource>();

		_client.onMessageReceived += GenerateAudioFile;
		_client.OpenServerChannel();

		Speaker.OnSpeakAudioGenerationComplete += (pModel) => StartCoroutine(LoadAudioRoutine(pModel));
		LipSyncRuntimeGenerator.onPhonemeGenerateSuccess += OnPhonemeGenerationComplete;
		LipSyncRuntimeGenerator.onPhonemeGenerateFail += OnPhonemeGenerationFail;
	}

	public void SendUserMessage(string pMessage) {
		Debug.Log("Sending message: " + pMessage);
		_client.SendUserMessage(pMessage);
	}

	private void StopInteraction() {
		StopCoroutine(_interactRoutine);
		dialogueText.gameObject.SetActive(false);

		_isInteracting = false;
	}

	public void Interact() {
		_interactRoutine = StartCoroutine(Interacting());
	}

	private IEnumerator Interacting() {
		//_isInteracting = true;
		//dialogueText.gameObject.SetActive(true);
		//interactionPrompt.EnableText(true);

		//while (true) {
		//	lock (_client.messageQueue) {
		//		if (_client.messageQueue.Count > 0) {
		//			dialogueText.text = _client.GetMessage;

		//		}
		//	}

		//	transform.LookAt(_player.transform);

		yield return new WaitForEndOfFrame();
		//}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Semicolon)) {
			LipSyncRuntimeGenerator.GenerateAudioFile("This is an example for the lipsync generation in runtime.", npcName);
		}

		//if (_isInteracting) {
		//	interactionPrompt.SetText("Press MB2 to stop interaction");
		//}
		//else {
		//	interactionPrompt.SetText("Press MB1 to interact");
		//}

		//if (!InInteractRange) {
		//	interactionPrompt.EnableText(false);
		//}
		//else interactionPrompt.EnableText(true);
	}

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

	#region LipSync/Audio Generation

	private void GenerateAudioFile(ServerPackage pPackage) {
		if (!Speaker.isSpeaking)
			LipSyncRuntimeGenerator.GenerateAudioFile(pPackage.text, npcName);
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