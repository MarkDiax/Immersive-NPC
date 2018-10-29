using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float speed = 10f;
    public CharacterController Instance;

    [SerializeField]
    private float translation, straffe;


    void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        //LevelManager.OnLoadLevel += OnLevelLoaded;
        LevelManager.OnUnloadLevel += OnLevelUnload;
    }

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}

    public void OnLevelLoaded() {
        translation = Input.GetAxis("Vertical") * speed;
        straffe = Input.GetAxis("Horizontal") * speed;
    }

    public void OnLevelUnload() {
       StartCoroutine(UpdateInputAfterLevelLoaded(translation, straffe, 0.2f));
    }

    private IEnumerator UpdateInputAfterLevelLoaded(float pTranslation, float pStraffe,float pDelayTime) {
        yield return new WaitForSeconds(pDelayTime);
        translation = pTranslation;
        straffe = pStraffe;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        transform.Translate(straffe, 0, translation);
    }
	
	// Update is called once per frame
	void Update () {
        translation = Input.GetAxis("Vertical") * speed;
        straffe = Input.GetAxis("Horizontal") * speed;

        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        transform.Translate(straffe, 0, translation);

        if(Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            Cursor.lockState = CursorLockMode.Locked;
        }

	}
}
