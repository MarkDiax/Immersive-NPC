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
    
	public AudioSource audioSource;
	public string VoiceName;

	protected Socket socket = null;
	protected List<string> chatLog = new List<string>();

	private List<string> _voiceQueue = new List<string>();

	void Destroy() {
		DoClose();
	}

	void Start() {
		DoOpen();

		uiSend.onClick.AddListener(() => {
			SendChat(uiInput.text);
			uiInput.text = "";
			uiInput.ActivateInputField();
		});
	}


	void Update() {
		lock (chatLog) {
			if (chatLog.Count > 0) {
				string str = uiChatLog.text;
				foreach (var s in chatLog) {
					str = str + "\n" + s;
				}
				uiChatLog.text = str;
				chatLog.Clear();
			}
		}

		lock (_voiceQueue) {
			if (_voiceQueue.Count > 0) {
				if (!audioSource.isPlaying) {
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
				lock (chatLog) {
					chatLog.Add("Socket.IO connected.");
				}
			});
			socket.On("bot_uttered", (data) => {
				string str = data.ToString();

                var jsonString = JsonConvert.SerializeObject(data);
                var serverMessage = JsonConvert.DeserializeObject<ServerPackage>(jsonString);

                string strChatLog = "Server: " + serverMessage.text;

                lock (_voiceQueue) {
					_voiceQueue.Add(serverMessage.text);
				}
				lock (chatLog) {
					chatLog.Add(strChatLog);
				}
			});
		}
	}

	void DoClose() {
		if (socket != null) {
			socket.Disconnect();
			socket = null;
		}
	}

    void SendChat(string str)
    {
        if (socket != null)
        {
            JObject message = new JObject();
            message.Add("message", str);
            chatLog.Add("Unity: " + str);
            socket.Emit("user_uttered", message);
        }
    }

    public void OnApplicationQuit() {
		DoClose();
	}
}