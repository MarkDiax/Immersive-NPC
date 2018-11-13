using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoSingleton<Player>
{
	public CustomInputField inputField;

	private bool _isInteracting;
	private FirstPersonController _controller;

	[HideInInspector]
	public UnityEvent onInteractRequest, onInteractStop;

	private void Awake() {
		onInteractRequest = new UnityEvent();
		onInteractStop = new UnityEvent();
	}

	private void Start() {
		_controller = GetComponent<FirstPersonController>();

		onInteractRequest.AddListener(OnInteractStart);
		onInteractStop.AddListener(OnInteractStop);
	}

	private void OnInteractStop() {
		_isInteracting = false;

		inputField.DeactivateInputField();
		inputField.gameObject.SetActive(false);
		_controller.enabled = true;
	}

	private void OnInteractStart() {
		_isInteracting = true;

		inputField.gameObject.SetActive(true);
		inputField.ActivateInputField();
		_controller.enabled = false;
	}

	public void SendUserMessage(string Message) {
		if (string.IsNullOrEmpty(Message))
			return;

		//NPCManager.Instance.onMessageSend.Invoke(Message);
	}

	private void Update() {
		if (_isInteracting) {
			if (Input.GetMouseButtonDown(1))
				onInteractStop.Invoke();
		}
		else {
			if (Input.GetMouseButtonDown(0)) 
				onInteractRequest.Invoke();
		}
	}
}