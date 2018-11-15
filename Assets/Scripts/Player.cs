using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoSingleton<Player>
{
	public CustomInputField inputField;

	private bool _isInteracting;
	private FirstPersonController _controller;

	public delegate void OnInteractWithNPCRequest(bool pIteracting);
	public OnInteractWithNPCRequest onInteractWithNPCRequest;

	private void Start() {
		_controller = GetComponent<FirstPersonController>();

		inputField.onEndEdit.AddListener(SendUserMessage);

		NPCManager.Instance.onInteractWithNPC += OnInteract;
		NPCManager.Instance.onPlayerOutOfRange += () => ToggleInteract(false);
	}

	private void ToggleInteract(bool pInteracting) {
		_isInteracting = pInteracting;
		_controller.enabled = !pInteracting;
		inputField.gameObject.SetActive(pInteracting);

		if (_isInteracting)
			inputField.ActivateInputField();
		else
			inputField.DeactivateInputField();
	}

	private void OnInteract(NPC pNPC) {
		ToggleInteract(pNPC == null ? false : true);
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
}