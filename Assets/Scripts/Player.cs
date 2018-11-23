using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoSingleton<Player>
{
	[Header("Voice Input")]
	public bool usingVoiceInput;
	public float speechTimeoutSeconds = 0.7f;


	public CustomInputField inputField;

	private bool _isInteracting;
	private FirstPersonController _controller;

	public delegate void OnInteractWithNPCRequest(bool pIteracting);
	public OnInteractWithNPCRequest onInteractWithNPCRequest;

	private void Start() {
		_controller = GetComponent<FirstPersonController>();

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

		if (usingVoiceInput)
			SpeechRecognizer.Instance.StartListen(speechTimeoutSeconds);
	}

	private void StopInteractWithNPC() {
		_isInteracting = false;
		_controller.enabled = true;
		inputField.DeactivateInputField();
		inputField.gameObject.SetActive(false);

		if (usingVoiceInput)
			SpeechRecognizer.Instance.StopListen();
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
	}


	private void ListenForVoice() {

	}
}