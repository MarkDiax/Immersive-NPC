using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
	[Header("UI")]
	public InteractionPrompt interactionPrompt;
	public TextMesh dialogueText;

	[Header("AI")]
	public float rotationSpeed = 20;
	public readonly float playerInteractionRange = 2f;

	private Player _player;
	private ClientScript _client;
	private string _currentMessage;
	private bool _isInteracting = false;

	private Coroutine InteractRoutine;

	private void Start() {

		NPCManager.Instance.RegisterNPC(this);
		NPCManager.Instance.onMessageSend.AddListener(SendUserMessage);
		_client = GetComponent<ClientScript>();

		_player = Player.Instance;
		_player.onInteractStop.AddListener(StopInteraction);

		//set random rotation at start
		//transform.eulerAngles = new Vector3(GetRandomFloat(-45, 45), GetRandomFloat(-45, 45), GetRandomFloat(-45, 45));
	}

	void SendUserMessage(object pMessage) {
		Debug.Log("Sending message: " + pMessage);
		_client.SendUserMessage(pMessage.ToString());
	}

	void StopInteraction() {
		StopCoroutine(InteractRoutine);
		dialogueText.gameObject.SetActive(false);

		_isInteracting = false;
	}

	public void Interact() {
		InteractRoutine = StartCoroutine(Interacting());
	}

	IEnumerator Interacting() {
		_isInteracting = true;
		dialogueText.gameObject.SetActive(true);
		interactionPrompt.EnableText(true);

		while (true) {
			lock (_client.messageQueue) {
				if (_client.messageQueue.Count > 0)
				{
					dialogueText.text = _client.GetMessage;
					
				}
			}

			transform.LookAt(_player.transform);

			yield return new WaitForEndOfFrame();
		}
	}

	void Update() {
		if (_isInteracting) {
			interactionPrompt.SetText("Press MB2 to stop interaction");
		}
		else {
			interactionPrompt.SetText("Press MB1 to interact");
			//Rotate();
		}

		if (!InInteractRange) {
			interactionPrompt.EnableText(false);
		}
		else interactionPrompt.EnableText(true);
	}

	void Rotate() {
		Vector3 normalizedRotation = transform.eulerAngles.normalized;
		Vector3 signedVector = new Vector3(Mathf.Sin(normalizedRotation.x), Mathf.Sin(normalizedRotation.y), Mathf.Sin(normalizedRotation.y));
		transform.Rotate(signedVector, rotationSpeed * Time.deltaTime);
	}

	private float GetRandomFloat(int MinValue, int MaxValue) {
		return ((float)new System.Random().Next(MinValue, MaxValue));
	}

	public bool InInteractRange {
		get {
			return Vector3.Distance(transform.position, _player.transform.position) < playerInteractionRange;
		}
	}
}