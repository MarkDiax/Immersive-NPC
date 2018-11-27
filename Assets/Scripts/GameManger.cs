using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

[Serializable]
public class ConfigFile {
    public bool Debug = false;
    public bool OverrideDatabase = false;
    public int LoadLevelOnStart = 0;
}


public class GameManger : MonoBehaviour {
	public ChatLog chatLog;

    [HideInInspector]
    public static GameManger Instance = null;

    private ConfigFile _configFile = null;
	private Player _player;
	private NPCManager _npcManager;

    private void Awake() {
        #region Instance And Dont Destroy On Load
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        #endregion
        //CreateConfigFile();
    }

	private void Start() {
		_player = Player.Instance;
		_npcManager = NPCManager.Instance;

		if (chatLog == null)
			chatLog = FindObjectOfType<ChatLog>();
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();
	}

	public void AddToChatlog(string pMessage) {
		if (chatLog == null) {
			Debug.LogError("Can't add message to ChatLog, ChatLog is NULL!");
			return;
		}

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
            if(_configFile == null) {
                _configFile = LoadConfigFile();
            }
            return _configFile;
        }
    }

    #endregion

}
