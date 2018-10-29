using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour {


    public string LevelToLoad = "";


    void OnTriggerEnter(Collider pColider) {
        if(pColider.gameObject.tag == "Player") {
            LevelManager.Instance.LoadLevel(LevelToLoad);
        }
    }

}
