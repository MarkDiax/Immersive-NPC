using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using Crosstales.RTVoice;
using UnityEngine.Events;

[System.Serializable]
public class ServerPackage
{
	public string text;
	//to be expanded...
}

public class SocketIOScript : MonoBehaviour
{
	public string serverURL = "http://localhost:5005";

	public InputField uiInput = null;
	public Button uiSend = null;
	public Text uiChatLog = null;

	public Text messagePlaceholder;
	public ScrollRect scrollRect;
	public float scrollSpeed = 2f;
	public int maxMessageCount = 16;

	public AudioSource audioSource;
	public string VoiceName;


	protected Socket socket = null;
	protected List<string> chatLog = new List<string>();

	private List<string> _voiceQueue = new List<string>();
	private List<string> _messageQueue = new List<string>();
	private Transform _scrollArea = null;

	void Destroy() {
		DoClose();
	}

	void Start() {
		_scrollArea = scrollRect.viewport.GetChild(0).transform;
		ClearScrollArea();

		DoOpen();

		uiSend.onClick.AddListener(() => {
			SendChat(uiInput.text);
			uiInput.text = "";
			uiInput.ActivateInputField();
		});
	}

	private void ClearScrollArea() {
		for (int i = 0; i < _scrollArea.childCount; i++)
			Destroy(_scrollArea.GetChild(0).gameObject);
	}

	private void UpdateScrollView() {
		if (scrollRect.verticalNormalizedPosition > 0)
			scrollRect.verticalNormalizedPosition -= Time.deltaTime * scrollSpeed;
	}

	void Update() {
		UpdateMessageLog();
		UpdateVoiceQueue();
		UpdateScrollView();
	}

	void UpdateMessageLog() {
		lock (_messageQueue) {
			if (_messageQueue.Count > 0) {
				ShowMessage(_messageQueue[0]);
				_messageQueue.RemoveAt(0);

				if (_scrollArea.childCount > maxMessageCount)
					Destroy(_scrollArea.GetChild(0).gameObject);
			}
		}
	}

	void UpdateVoiceQueue() {
		lock (_voiceQueue) {
			if (_voiceQueue.Count > 0) {
				if (string.IsNullOrEmpty(_voiceQueue[0]))
					_voiceQueue.RemoveAt(0);

				else if (!audioSource.isPlaying) {
					Speaker.Speak(_voiceQueue[0], audioSource, Speaker.VoiceForName(VoiceName), true, 1, 1, 1, "_Speeches/TestFile");
					_voiceQueue.RemoveAt(0);
				}
			}
		}
	}

	void DoOpen() {
		if (socket == null) {
			socket = IO.Socket(serverURL);
			socket.On(Socket.EVENT_CONNECT, () => {
				ReadyMessage("Socket.IO connected.");
			});

			socket.On("bot_uttered", (data) => {
				string str = data.ToString();

				var jsonString = JsonConvert.SerializeObject(data);
				var serverMessage = JsonConvert.DeserializeObject<ServerPackage>(jsonString);

				string strChatLog = "Server: " + serverMessage.text;
				ReadyMessage(strChatLog);

				lock (_voiceQueue) {
					_voiceQueue.Add(serverMessage.text);
				}
			});
		}
	}

	void SendChat(string str) {
		if (socket != null) {
			JObject message = new JObject();
			message.Add("message", str);

			lock (_scrollArea) {
				ReadyMessage("Unity: " + str);
			}

			socket.Emit("user_uttered", message);
		}
	}

	private void ReadyMessage(string Message) {
		lock (_messageQueue) {
			_messageQueue.Add(Message);
		}
	}

	private void ShowMessage(string Message) {
		Text txtMessage = Instantiate(messagePlaceholder, _scrollArea);
		txtMessage.text = Message;
	}

	private void DoClose() {
		if (socket != null) {
			socket.Disconnect();
			socket = null;
		}
	}

	public void OnApplicationQuit() {
		DoClose();
	}
}