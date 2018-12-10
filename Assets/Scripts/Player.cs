using Crosstales.RTVoice;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoSingleton<Player>
{
	[Header("Voice Input")]
	public bool usingVoiceInput;
	private bool voiceIsOn;

	public CustomInputField inputField;

	private bool _isInteracting;
	private FirstPersonController _controller;
	private SpeechRecognizer _speechRecognizer;

	public delegate void OnInteractWithNPCRequest(bool pIteracting);
	public OnInteractWithNPCRequest onInteractWithNPCRequest;

	private void Start() {
		_controller = GetComponent<FirstPersonController>();
		_speechRecognizer = SpeechRecognizer.Instance;

		inputField.onEndEdit.AddListener(SendUserMessage);

		NPCManager.Instance.onInteractWithNPC += OnInteract;
		NPCManager.Instance.onPlayerOutOfRange += StopInteractWithNPC;

		SpeechRecognizer.Instance.onSpeechRecognized += (pText) => {
			if (_isInteracting) {
				SendUserMessage(pText);
				inputField.text = "";
			}
		};
	}

	private void StartInteractWithNPC() {
		_isInteracting = true;
		_controller.enabled = false;
		inputField.gameObject.SetActive(true);

		inputField.ActivateInputField();

		if (usingVoiceInput) {
			_speechRecognizer.StartListen();
			voiceIsOn = true;
		}
	}

	private void StopInteractWithNPC() {
		_isInteracting = false;
		_controller.enabled = true;
		inputField.DeactivateInputField();
		inputField.gameObject.SetActive(false);

		if (usingVoiceInput) {
			_speechRecognizer.StopListen();
			voiceIsOn = false;
		}
	}

	private void OnInteract(NPC pNPC) {
		if (pNPC == null)
			StopInteractWithNPC();
		else
			StartInteractWithNPC();
	}

	public void SendUserMessage(string pMessage) {
		if (string.IsNullOrEmpty(pMessage) || string.IsNullOrWhiteSpace(pMessage))
			return;

		NPC npc = NPCManager.Instance.currentInteractingNPC;
		if (npc != null)
			npc.SendUserMessage(pMessage);

		GameManger.Instance.AddToChatlog("Player: " + pMessage);
	}

	private void Update() {
		if (_isInteracting) {
			if (Input.GetMouseButtonDown(1))
				onInteractWithNPCRequest.Invoke(false);
		}
		else {
			if (Input.GetMouseButtonDown(0))
				onInteractWithNPCRequest.Invoke(true);
		}

		if (Input.GetKeyDown(KeyCode.F1)) {
			_speechRecognizer.ToggleSpeechRecognizer();
		}
	}
}