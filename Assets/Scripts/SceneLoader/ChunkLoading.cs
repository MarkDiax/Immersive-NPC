using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ChunkLoading : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string path = Application.dataPath + "/AssetBundle/stationselection";

        //StartCoroutine(loadWWWNonCaching(url));
        StartCoroutine(LoadFromMemoryAsync(path));
    }


    IEnumerator LoadFromMemoryAsync(string path) {
        AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
        yield return createRequest;
        AssetBundle bundle = createRequest.assetBundle;
        var prefab = bundle.LoadAsset<GameObject>("SectionsTest");
        Instantiate(prefab);
        //bundle.Unload(true);
    }   

    // Update is called once per frame
    void Update () {
		
	}
}
