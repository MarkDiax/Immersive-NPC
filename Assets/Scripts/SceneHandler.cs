using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public Image fadeImage;
    public Button ButtonRinnol,ButtonAkemaz, ButtonEndGame;
    public Text TextEndgame;

    public static SceneHandler Instance = null;

    private bool _RestartGame = false;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += SceneChange;
            StartCoroutine(FadeIn());
        }
        else {
            GameObject.Destroy(gameObject);
        }

    
    }

    private void SceneChange(Scene pCurrent, Scene pNext) {
        StartCoroutine(FadeIn());
    }

    public void GoToSceneWIthFadeOut(int pSceneIndex) {
        StartCoroutine(FadeOut(pSceneIndex));
    }

    public void Update() {
        if(Input.GetKeyDown(KeyCode.N)) {
			if (SceneManager.GetActiveScene().buildIndex <=1)
			{
				GoToSceneWIthFadeOut(SceneManager.GetActiveScene().buildIndex + 1);
			}
            
        }
    }


    IEnumerator FadeIn() {
        for(float f = 1f; f >= 0; f -= 0.1f) {
            Color c = fadeImage.color;
            c.a = f;
            fadeImage.color = c;
            yield return null;
        }

        if(SceneManager.GetActiveScene().buildIndex == 2) {
            ButtonRinnol.gameObject.SetActive(true);
            ButtonRinnol.interactable = false;
            ButtonAkemaz.gameObject.SetActive(true);
            ButtonAkemaz.interactable = true;
                ;
        } else if(SceneManager.GetActiveScene().buildIndex ==3) {
            ButtonRinnol.gameObject.SetActive(true);
            ButtonRinnol.interactable = true;
            ButtonAkemaz.gameObject.SetActive(true);
            ButtonAkemaz.interactable = false;
        }
    }


    IEnumerator FadeOut(int pSceneToLoad = -1) {
        for(float f = 0; f <= 1; f += 0.1f) {
            Color c = fadeImage.color;
            c.a = f;
            fadeImage.color = c;
            yield return null;
        }
        if(pSceneToLoad != -1) {
            SceneManager.LoadScene(pSceneToLoad);
        }
        
    }

    public void StartFadeIn() {
        if(_RestartGame) {
            SceneManager.LoadScene(0);
        }
        StartCoroutine(FadeIn());
        ButtonEndGame.gameObject.SetActive(false);
        TextEndgame.gameObject.SetActive(false);
    }

    public void ShowEndScreen(string pTextToDisplay, int pCaptureTryes) {
        if((pCaptureTryes >= 3) || (pCaptureTryes == -1)) {
            ButtonEndGame.GetComponentInChildren<Text>().text = "Restart";
            _RestartGame = true;
        }
        ButtonEndGame.gameObject.SetActive(true);
        TextEndgame.gameObject.SetActive(true);

        TextEndgame.text = pTextToDisplay;
        StartCoroutine(FadeOut());

    }

}
