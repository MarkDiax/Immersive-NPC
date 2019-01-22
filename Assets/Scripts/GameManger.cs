using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using RogoDigital.Lipsync;

[Serializable]
public class ConfigFile
{
	public bool Debug = false;
	public bool OverrideDatabase = false;
	public int LoadLevelOnStart = 0;
}


public class GameManger : MonoBehaviour
{
	public ChatLog chatLog;
	public Text npcServerMessageDisplay;
	public string soxInstallPath;

	[HideInInspector]
	public static GameManger Instance = null;

	private ConfigFile _configFile = null;
	private Player _player;
	private NPCManager _npcManager;
	private SpeechRecognizer _speechRecognizer;
	private Coroutine _speechRecognizerToggleRoutine;

	private void Awake() {
		#region Instance And Dont Destroy On Load
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
		}
		#endregion

		//CreateConfigFile();
	}

	private void Start() {
		_player = Player.Instance;
		_npcManager = NPCManager.Instance;
		_speechRecognizer = SpeechRecognizer.Instance;

		InitiateMiscSoftware();

		if (chatLog == null)
			chatLog = FindObjectOfType<ChatLog>();
	}

	private void InitiateMiscSoftware()
	{
		LipSyncRuntimeGenerator.SetSoxFilePath(soxInstallPath);
	}

	private void Update() {

		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		CheckForNPCAudio();
	}

	/// <summary>
	/// Prevents an audio feedback loop where the SpeechRecognizer is listening to its own input.
	/// </summary>
	private void CheckForNPCAudio() {
		if (_npcManager.currentInteractingNPC != null) {
			NPC npc = _npcManager.currentInteractingNPC;

			if (npc.IsSpeaking && _speechRecognizer.IsListening) {
				if (_speechRecognizerToggleRoutine == null)
					_speechRecognizerToggleRoutine = StartCoroutine(SpeechRecognizerToggle(npc));
			}
		}
	}

	private IEnumerator SpeechRecognizerToggle(NPC pNPC) {
		Debug.Log("GM: Toggling SpeechRecognizer to prevent audio feedback loop!");
		_speechRecognizer.StopListen();
		yield return new WaitWhile(() => pNPC.IsSpeaking);

		if (_npcManager.currentInteractingNPC != null)
			_speechRecognizer.StartListen();

		_speechRecognizerToggleRoutine = null;
	}



	public void AddToChatlog(string pMessage) {
		if (chatLog == null || !chatLog.gameObject.activeSelf) 
			Debug.LogError("GM: Can't add message to ChatLog, ChatLog is NULL!");	
		else
			chatLog.AddChatMessage(pMessage);
	}

	#region Config File Settings
	public void CreateConfigFile() {
		ConfigFile configFile = new ConfigFile();
		configFile.Debug = true;
		configFile.OverrideDatabase = true;
		configFile.LoadLevelOnStart = 0;
		string configObj = JsonUtility.ToJson(configFile);

		File.WriteAllText(Application.dataPath + "/StreamingAssets/Config.txt", configObj);

	}
	private ConfigFile LoadConfigFile() {
		string text = File.ReadAllText(Application.dataPath + "/StreamingAssets/Config.txt");

		return JsonUtility.FromJson<ConfigFile>(text);
	}

	public ConfigFile ConfigFile {
		get {
			if (_configFile == null) {
				_configFile = LoadConfigFile();
			}
			return _configFile;
		}
	}

	#endregion

}
