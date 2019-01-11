﻿using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using RogoDigital.Lipsync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NPC : MonoBehaviour
{
	[Header("NPC Info")]
	public string npcName = "Default-NPC";

	[Header("AI")]
	public float playerInteractionRange = 3f;

	[Header("Voice Settings")]
	public string maryttsVoiceName = "dfki-spike-hsmm";
	public NPCVoiceSettings normalVoiceSettings;
	public NPCVoiceSettings happyVoiceSettings;
	public NPCVoiceSettings angryVoiceSettings;
	public NPCVoiceSettings surprisedVoiceSettings;
	public NPCVoiceSettings confusedVoiceSettings;

	[Header("Voice Generation")]
	public LipSyncRuntimeGenerator.MaryXMLAttribute[] maryXMLAttributes;

	private enum FacialExpressions
	{
		Happy, Angry, Confused, Surprised
	}

	private string _currentExpression;

	private Player _player;
	private ClientScript _client;
	private LipSync _lipSync;
	private AudioSource _audioSource;
	private Animator _animator;

	private bool _processingMessage;

	private List<ServerPackage> _messageQueue = new List<ServerPackage>();

	private void Start()
	{
		NPCManager.Instance.RegisterNPC(this);

		_player = Player.Instance;

		_client = GetComponent<ClientScript>();
		_lipSync = GetComponent<LipSync>();
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();

		_client.onMessageReceived += OnMessageReceived;
		_client.onAnimationEventReceived += OnAnimationEventReceived;
		_client.OpenServerChannel();

		Speaker.OnSpeakAudioGenerationComplete += (pModel) => StartCoroutine(LoadAudioRoutine(pModel));
		LipSyncRuntimeGenerator.onPhonemeGenerateSuccess += OnPhonemeGenerationComplete;
		LipSyncRuntimeGenerator.onPhonemeGenerateFail += OnPhonemeGenerationFail;
	}

	public void SaveVoiceSettings()
	{
		NPCVoiceSettings voiceSettings = ScriptableObject.CreateInstance<NPCVoiceSettings>();
		voiceSettings.maryttsVoiceName = maryttsVoiceName;
		voiceSettings.maryXMLAttributes = maryXMLAttributes;

		string npcAssetPath = "/ScriptableObjects/VoiceSettings/" + npcName + "/";
		string folderPath = Application.dataPath + npcAssetPath;

		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string fileName = npcName + "_VoiceSettings";
		AssetDatabase.CreateAsset(voiceSettings, "Assets" + npcAssetPath + fileName + ".asset");
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = voiceSettings;
	}

	public void SendUserMessage(string pMessage)
	{
		if (string.IsNullOrEmpty(pMessage) || string.IsNullOrWhiteSpace(pMessage))
			return;

		if (_client.IsConnected)
		{
			Debug.Log("Sending message: " + pMessage);
			_client.SendUserMessage(pMessage);
		}

		//FOR DEBUGGING ONLY
		else
		{
			ServerPackage package = new ServerPackage
			{
				text = pMessage
			};
			OnMessageReceived(package);
		}
		//
	}

	public void Interact()
	{
		Vector3 localEuler = transform.eulerAngles;
		transform.LookAt(_player.transform);
		transform.eulerAngles = new Vector3(localEuler.x, transform.eulerAngles.y, localEuler.z);
	}

	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.Keypad1))
		{
			OnAnimationEventReceived("Happy");
		}
		if (Input.GetKeyDown(KeyCode.Keypad2))
		{
			OnAnimationEventReceived("Angry");
		}
		if (Input.GetKeyDown(KeyCode.Keypad3))
		{
			OnAnimationEventReceived("Confused");
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			OnAnimationEventReceived("Surprised");
		}

		if (Input.GetKeyDown(KeyCode.Semicolon))
		{
			ServerPackage p = new ServerPackage
			{
				text = "This is an example for the lipsync generation and voice synthesis for this NPC. Pause. My name is Akkamass!"
			};
			OnMessageReceived(p);
		}

		UpdateMessageQueue();
	}

	/// <summary>
	/// This queue exists to make sure the NPC is not overriding its own spoken lines.
	/// </summary>
	private void UpdateMessageQueue()
	{
		if (_processingMessage || (_lipSync.IsPlaying && _audioSource.isPlaying))
			return;

		if (_messageQueue.Count > 0)
		{
			Debug.LogWarning("Dequeuing ServerPackage");
			OnMessageReceived(_messageQueue[0]);
			_messageQueue.RemoveAt(0);
		}
	}

	#region LipSync/Audio Generation
	private IEnumerator LoadAudioRoutine(Wrapper pWrapper)
	{
		Debug.Log("Audio generation complete");
		string filePath = pWrapper.OutputFile;

		if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || Path.GetFileNameWithoutExtension(filePath).IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
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

	private void GenerateLipSync(ServerPackage pPackage)
	{
		_processingMessage = true;
		LipSyncRuntimeGenerator.GenerateAudioFile(pPackage.text, npcName, maryttsVoiceName, maryXMLAttributes);
	}

	private void OnMessageReceived(ServerPackage pPackage)
	{
		if (!(_lipSync.IsPlaying && _audioSource.isPlaying))
		{
			GenerateLipSync(pPackage);
			GameManger.Instance.AddToChatlog(npcName + ": " + pPackage.text);
			return;
		}

		Debug.LogWarning("NPC is busy, adding ServerPackage to queue");
		_messageQueue.Add(pPackage);
	}

	private void OnAnimationEventReceived(string pAnimationEvent)
	{
		//reset the expressions
		foreach (FacialExpressions expression in Enum.GetValues(typeof(FacialExpressions)))
		{
			if (expression.ToString() != _currentExpression && expression.ToString() != pAnimationEvent)
				SetFloatAnimator(expression.ToString(), 0, 0.5f);
		}

		if (string.IsNullOrEmpty(_currentExpression) || string.IsNullOrWhiteSpace(_currentExpression))
		{
			_currentExpression = pAnimationEvent;
			SetFloatAnimator(_currentExpression, 0.5f, 0.5f);
			return;
		}

		if (_currentExpression == pAnimationEvent)
			SetFloatAnimator(pAnimationEvent, 1f, 0.5f);
		else
		{
			SetFloatAnimator(_currentExpression, 0.5f, 0.5f);
			SetFloatAnimator(pAnimationEvent, 0.5f, 0.5f);
			_currentExpression = pAnimationEvent;
		}

		Debug.Log($"Switching Facial Animation for {npcName} to {pAnimationEvent}.");
	}

	/// <summary>
	/// Sets a float parameter in the animator over time (in seconds).
	/// </summary>
	/// <param name="pParamName"></param>
	/// <param name="pValue"></param>
	/// <param name="pTime"></param>
	private void SetFloatAnimator(string pParamName, float pValue, float pTime = 0f)
	{
		StartCoroutine(SetOverTime());
		IEnumerator SetOverTime()
		{
			while (pTime > 0)
			{
				float deltaTime = Time.deltaTime;
				_animator.SetFloat(pParamName, pValue, pTime, deltaTime);
				pTime -= deltaTime;

				yield return new WaitForEndOfFrame();
			}

			_animator.SetFloat(pParamName, pValue);
		}
	}

	private void OnPhonemeGenerationFail(string pError)
	{
		Debug.LogError(pError);
	}

	private void OnPhonemeGenerationComplete(AudioClip pAudioClip, List<PhonemeMarker> pMarkers)
	{
		LipSyncData data = LipSyncRuntimeManager.Instance.lipSyncData;
		data.clip = pAudioClip;
		data.phonemeData = pMarkers.ToArray();

		Debug.Log("Playing LipSync data");
		_lipSync.ClearDataCache();
		_lipSync.Play(data);
		_lipSync.ProcessData();

		_audioSource.clip = pAudioClip;
		_audioSource.Play();
		_processingMessage = false;
	}
	#endregion

	public bool InInteractRange => Vector3.Distance(transform.position, _player.transform.position) < playerInteractionRange;
	public bool IsSpeaking => _audioSource.isPlaying;
}
