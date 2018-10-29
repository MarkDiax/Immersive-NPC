using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public delegate void SceneUnloadEvent();
    public static event SceneUnloadEvent OnUnloadLevel;

    public delegate void SceneloadEvent();
    public static event SceneloadEvent OnLoadLevel;

    //[SerializeField]
    //private string[] _levelNames;
    [SerializeField]//temp
    private string _nextLevel;

    public static LevelManager Instance;   

	void Awake () {
        if(Instance== null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
     
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            _nextLevel = "MainMenu";
            StartCoroutine("LoadRequestedLevel");
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            _nextLevel = "Level1";
            StartCoroutine("LoadRequestedLevel");
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            _nextLevel = "Level2";
            StartCoroutine("LoadRequestedLevel");
        }
    }

    public void LoadLevel(string pLevelName) {
        _nextLevel = pLevelName;
        StartCoroutine("LoadRequestedLevel");
    }



    IEnumerator LoadRequestedLevel() {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_nextLevel);
        while(!operation.isDone) {
            if(OnUnloadLevel != null) {
                OnUnloadLevel();
            }
            yield return null;
        }
        if(OnLoadLevel !=null) {
            OnLoadLevel();
        }
    }
}
