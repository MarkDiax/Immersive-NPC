using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;


public class ClientScript : MonoBehaviour
{
	public string serverURL = "http://localhost:5005";

	public bool usingLocalHost;
	private readonly string _localURL = "http://localhost:5005";

	public delegate void OnMessageReceived(ServerPackage serverPackage);
	public OnMessageReceived onMessageReceived;

	public delegate void OnAnimationEventReceived(string animationEvent);
	public OnAnimationEventReceived onAnimationEventReceived;

	public readonly object eventLocker = new object();

	private Socket _socket = null;
	private List<ServerPackage> _packageQueue = new List<ServerPackage>();
	private List<string> _animationEventQueue = new List<string>();

	public class Elements
	{
		public string animation { get; set; }
	}

	public class Payload
	{
		public string template_type { get; set; }
		public Elements elements { get; set; }
	}

	public class Attachment
	{
		public string type { get; set; }
		public Payload payload { get; set; }
	}

	public class RootObject
	{
		public Attachment attachment { get; set; }
	}

	void Destroy()
	{
		DoClose();
	}

	private void Start()
	{
		if (usingLocalHost)
			serverURL = _localURL;
	}

	private void Update()
	{
		if (_packageQueue.Count > 0)
		{
			//Send the event here, in the main thread, because every action 
			//after this has to be called in the main thread as well.
			lock (eventLocker)
			{
				for (int i = _packageQueue.Count; i-- > 0;)
				{
					onMessageReceived.Invoke(_packageQueue[i]);
					_packageQueue.RemoveAt(i);
				}
			}
		}

		if (_animationEventQueue.Count > 0)
		{
			lock (eventLocker)
			{
				for (int i = _animationEventQueue.Count; i-- > 0;)
				{
					onAnimationEventReceived.Invoke(_animationEventQueue[i]);
					_animationEventQueue.RemoveAt(i);
				}
			}
		}
	}

	public void OpenServerChannel()
	{
		if (_socket == null)
		{
			Debug.Log("Connecting to server..");

			_socket = IO.Socket(serverURL);
			_socket.On(Socket.EVENT_CONNECT, () =>
			{
				Debug.Log("Socket.IO connected.");
				IsConnected = true;
			});

			_socket.On("bot_uttered", (data) =>
			{
				var jsonString = JsonConvert.SerializeObject(data);
				var serverMessage = JsonConvert.DeserializeObject<ServerPackage>(jsonString);

				RootObject animationEvent = JsonConvert.DeserializeObject<RootObject>(jsonString);
				string animationToPlay = animationEvent.attachment.payload.elements.animation;

				if (!string.IsNullOrEmpty(animationToPlay) && !string.IsNullOrWhiteSpace(animationToPlay))
				{
					Debug.Log("Received Animation Event: " + animationToPlay);

					lock (eventLocker)
						_animationEventQueue.Add(animationToPlay);
				}


				if (string.IsNullOrEmpty(serverMessage.text) || string.IsNullOrWhiteSpace(serverMessage.text))
					Debug.LogError("Server text was null or whitespace! Ignoring server message.");
				else
				{
					string strChatLog = "Server: " + serverMessage.text;
					Debug.Log(strChatLog);

					/*
					RootObject command = JsonConvert.DeserializeObject<RootObject>(jsonString);
					string receivedAnimationEvent = command?.attachment.payload.elements.animation;
					Debug.Log("Received Animation Event: " + receivedAnimationEvent);
					*/

					//add message to list that is iterated on in the main thread
					lock (eventLocker)
						_packageQueue.Add(serverMessage);
				}
			});
		}
	}

	public void SendUserMessage(string str)
	{
		if (_socket != null)
		{
			JObject message = new JObject();
			message.Add("message", str);
			_socket.Emit("user_uttered", message);
		}
	}

	public virtual void DoClose()
	{
		if (_socket != null)
		{
			_socket.Disconnect();
			_socket = null;
			IsConnected = false;
		}
	}

	protected virtual void OnApplicationQuit()
	{
		DoClose();
	}

	public bool IsConnected { get; set; }
}