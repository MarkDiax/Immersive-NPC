using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Quobject.SocketIoClientDotNet.Client;

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

	protected Socket socket = null;
	protected List<string> chatLog = new List<string>();

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
	}

	void DoOpen() {
		if (socket == null) {
			socket = IO.Socket(serverURL);
			socket.On(Socket.EVENT_CONNECT, () => {
				lock (chatLog) {
					// Access to Unity UI is not allowed in a background thread, so let's put into a shared variable
					chatLog.Add("Socket.IO connected.");
				}
			});
			socket.On("bot_uttered", (data) => {
				string str = data.ToString();

				ServerPackage package = JsonUtility.FromJson<ServerPackage>(str);
				string strChatLog = "Server: " + package.text;
				
				// Access to Unity UI is not allowed in a background thread, so let's put into a shared variable
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


	void SendChat(string str) {
		if (socket != null) {
			socket.Emit("user_uttered", str);
			chatLog.Add("Unity: " + str);
		}
	}

	public void OnApplicationQuit() {
		DoClose();
	}
}
