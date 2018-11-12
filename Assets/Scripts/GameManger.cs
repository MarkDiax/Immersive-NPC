using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class ConfigFile {
    public bool Debug = false;
    public bool OverrideDatabase = false;
    public int LoadLevelOnStart = 0;
}


public class GameManger : MonoBehaviour {
    [HideInInspector]
    public static GameManger Instance = null;

    private ConfigFile _configFile = null;

    public void Awake() {
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
