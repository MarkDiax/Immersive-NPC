using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class CutSceneVideoplayer : MonoBehaviour
{
    private VideoPlayer _player;

    // Start is called before the first frame update
    void Start() {
        _player = GetComponent<VideoPlayer>();
        _player.loopPointReached += VideoEnded;
    }


    public void VideoEnded(VideoPlayer pPlayer) {
        SceneHandler.Instance.GoToSceneWIthFadeOut(SceneManager.GetActiveScene().buildIndex + 1);
    }


}
