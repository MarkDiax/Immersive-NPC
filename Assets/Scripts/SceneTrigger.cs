using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SceneTrigger : MonoBehaviour
{
    PlayableDirector director;
    //public CinemachineBrain cineMachine;

    // Start is called before the first frame update
    void Start()
    {
        director = FindObjectOfType<PlayableDirector>();
        director.stopped += TriggerEndCutScene;
        
    }

    public void TriggerEndCutScene(PlayableDirector pDirector) {
        SceneHandler.Instance.GoToSceneWIthFadeOut(2);

    }



}
