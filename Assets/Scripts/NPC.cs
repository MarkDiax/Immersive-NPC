using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
	[Header("UI")]
	public Text interactPrompt;
	public TextMesh dialogueText;

	[Header("AI")]
	public float rotationSpeed = 20;
	public readonly float playerInteractionRange = 2f;

	private bool _isTalking = false;
	private Player _player;
	private string _currentMessage;

	private void Start() {
		NPCManager.Instance.RegisterNPC(this);

		_player = Player.Instance;
		_player.StopInteract.AddListener(InteractWithPlayer);

		//set random rotation at start
		transform.eulerAngles = new Vector3(GetRandomFloat(-45, 45), GetRandomFloat(-45, 45), GetRandomFloat(-45, 45));
	}

	void StopInteraction() {
		_isTalking = false;
		_player.SetInteraction(_isTalking);
		dialogueText.gameObject.SetActive(_isTalking);
	}

	void StartInteraction() {
		if (InInteractRange) {
			_isTalking = true;
			_player.SetInteraction(_isTalking);
			dialogueText.gameObject.SetActive(_isTalking);
		}
	}

	public void InteractWithPlayer() {
		if (!_isTalking && InInteractRange) {
			_isTalking = true;
			_player.SetInteraction(true);
			dialogueText.gameObject.SetActive(true);
			return;
		}
		_isTalking = !_isTalking;
		dialogueText.gameObject.SetActive(_isTalking);
		_player.SetInteraction(_isTalking);
	}

	void Update() {
		if (_isTalking) {
			lock (_player.client.messageQueue) {
				if (_player.client.messageQueue.Count > 0)
					dialogueText.text = _player.client.GetMessage;
			}

			transform.LookAt(_player.transform);

			interactPrompt.text = "Press MB2 to stop interaction";

			if (Input.GetMouseButtonDown(1)) {
				_isTalking = false;
			}
		}
		else {
			interactPrompt.text = "Press MB1 to interact";
			Rotate();
		}

		if (InInteractRange) {
			interactPrompt.enabled = true;
		}
		else {
			interactPrompt.enabled = false;
			if (_isTalking) {
				_isTalking = false;
				_player.SetInteraction(false);
				dialogueText.gameObject.SetActive(false);
			}
		}
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