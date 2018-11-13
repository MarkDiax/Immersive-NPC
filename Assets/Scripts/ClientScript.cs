using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;


public class ClientScript : MonoBehaviour
{
	public string serverURL = "http://localhost:5005";

	public delegate void OnMessageReceived(ServerPackage serverPackage);
	public OnMessageReceived onMessageReceived;

	public readonly object eventLocker = new object();

	private Socket _socket = null;
	private List<ServerPackage> _packageQueue = new List<ServerPackage>();

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

	private void Update() {
		if (_packageQueue.Count > 0) {
			//Send the event here, in the main thread, because every action 
			//after this has to be called in the main thread as well.
			lock (eventLocker) {
				for (int i = _packageQueue.Count; i-- > 0;) {
					onMessageReceived.Invoke(_packageQueue[i]);
					_packageQueue.RemoveAt(i);
				}
			}
		}
	}

	public void OpenServerChannel() {
		if (_socket == null) {
			Debug.Log("Connecting to server..");

			_socket = IO.Socket(serverURL);
			_socket.On(Socket.EVENT_CONNECT, () => {
				Debug.Log("Socket.IO connected.");
			});

			_socket.On("bot_uttered", (data) => {

				var jsonString = JsonConvert.SerializeObject(data);
				var serverMessage = JsonConvert.DeserializeObject<ServerPackage>(jsonString);

				if (string.IsNullOrEmpty(serverMessage.text) || string.IsNullOrWhiteSpace(serverMessage.text))
					Debug.LogError("Server text was null or whitespace! Ignoring server message.");
				else {
					string strChatLog = "Server: " + serverMessage.text;
					Debug.Log(strChatLog);

					//add message to list that is iterated on in the main thread
					lock (eventLocker)
						_packageQueue.Add(serverMessage);
				}
			});
		}
	}

	public void SendUserMessage(string str) {
		if (_socket != null) {
			JObject message = new JObject();
			message.Add("message", str);
			_socket.Emit("user_uttered", message);
		}
	}

	public virtual void DoClose() {
		if (_socket != null) {
			_socket.Disconnect();
			_socket = null;
		}
	}

	protected virtual void OnApplicationQuit() {
		DoClose();
	}
}