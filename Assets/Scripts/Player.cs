using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoSingleton<Player>
{
	[HideInInspector]
	public UnityEvent InteractEvent, StopInteract;
	public InputField inputField;

	private FirstPersonController _controller;
	public ClientScript client;
	private bool _isInteracting;

	private void Awake() {
		InteractEvent = new UnityEvent();
		StopInteract = new UnityEvent();
	}

	private void Start() {
		client = GetComponent<ClientScript>();
		_controller = GetComponent<FirstPersonController>();
	}

	public void SendUserMessage(string Message) {
		Debug.Log("Sending: " + Message);
		client.SendUserMessage(Message);
	}

	private void Update() {
		if (_isInteracting) {
			if (Input.GetMouseButtonDown(1))
				StopInteract.Invoke();
		}
		else {
			if (Input.GetMouseButtonDown(0)) {
				InteractEvent.Invoke();
			}
		}
	}

	public void SetInteraction(bool Interacting) {
		_isInteracting = Interacting;

		if (_isInteracting) {
			inputField.gameObject.SetActive(true);
			inputField.ActivateInputField();
			inputField.onEndEdit.AddListener(SendUserMessage);
			_controller.enabled = false;
		}
		else {
			inputField.DeactivateInputField();
			inputField.gameObject.SetActive(false);
			inputField.onEndEdit.RemoveListener(SendUserMessage);
			_controller.enabled = true;
		}
	}
}