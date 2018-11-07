using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using UnityEngine.Events;
using Crosstales.RTVoice;
using RogoDigital.Lipsync;

public class ClientScript : MonoBehaviour
{
	public LipSyncData lipdata;
	public string serverURL = "http://localhost:5005";

	public bool usingRTVoice = false;
	public AudioSource audioSource;
	public string VoiceName;

	protected Socket socket = null;

	private List<string> _voiceQueue = new List<string>();
	public List<string> messageQueue = new List<string>();

	private int _audioFileIndex;

	enum MessageStatus
	{
		Undiscovered,
		Discovered,
		InProgress,
		Completed
	}

	void Destroy() {
		DoClose();
	}

	void Start() {
		//DoOpen();

	}

	public void TestMessageGeneration(string pMessage) {
		GenerateAudioFile("Assets/Resources/Audio/", "VoiceTest", pMessage);
	}

	private void GenerateAudioFile(string FileDirectory, string FileName, string TextToGenerateFrom) {
		if (!Directory.Exists(FileDirectory))
			Directory.CreateDirectory(FileDirectory);

		Debug.Log("Generating TTS audio file for text: " + TextToGenerateFrom);
		Speaker.Generate(TextToGenerateFrom, FileDirectory + FileName + _audioFileIndex, Speaker.VoiceForName(VoiceName));
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space))
			GenerateAudioFile("Assets/Resources/Audio/", "VoiceTest", "hello i am garfield and i love lasagne");


		//UpdateVoiceQueue();
	}

	void UpdateVoiceQueue() {
		lock (_voiceQueue) {
			if (_voiceQueue.Count > 0) {
				if (string.IsNullOrEmpty(_voiceQueue[0]))
					_voiceQueue.RemoveAt(0);

				else if (!audioSource.isPlaying) {
					//Speaker.Speak(_voiceQueue[0], audioSource, Speaker.VoiceForName(VoiceName), true, 1, 1, 1, "_Speeches/TestFile");
					Speaker.Generate(_voiceQueue[0], "_AudioCache/Voices/VoiceTest" + _audioFileIndex, Speaker.VoiceForName(VoiceName));
					_audioFileIndex++;

					_voiceQueue.RemoveAt(0);
				}
			}
		}
	}

	public void DoOpen() {
		if (socket == null) {
			socket = IO.Socket(serverURL);
			socket.On(Socket.EVENT_CONNECT, () => {
				ReadyMessage("Socket.IO connected.");
			});

			socket.On("bot_uttered", (data) => {

				var jsonString = JsonConvert.SerializeObject(data);
				var serverMessage = JsonConvert.DeserializeObject<ServerPackage>(jsonString);

				string strChatLog = "Server: " + serverMessage.text;

				ReadyMessage(serverMessage.text);
				ReadyVoiceMessage(serverMessage.text);
			});
		}
	}

	public string GetMessage {
		get {
			lock (messageQueue) {
				if (messageQueue.Count > 0) {
					string message = messageQueue[0];
					messageQueue.RemoveAt(0);
					return message;
				}

				return null;
			}
		}
	}

	public void SendUserMessage(string str) {
		if (socket != null) {
			JObject message = new JObject();
			message.Add("message", str);
			socket.Emit("user_uttered", message);
		}
	}

	private void ReadyMessage(string Message) {
		lock (messageQueue) {
			messageQueue.Add(Message);
		}
	}

	public void ReadyVoiceMessage(string Message) {
		lock (_voiceQueue) {
			if (usingRTVoice)
				_voiceQueue.Add(Message);
		}
	}

	public virtual void DoClose() {
		if (socket != null) {
			socket.Disconnect();
			socket = null;
		}
	}

	protected virtual void OnApplicationQuit() {
		DoClose();
	}
}