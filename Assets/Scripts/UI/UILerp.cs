using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum LerpState {
    AtStartPostion = 0,
    AtEndpositon =1,
    ToEnd = 2,
    ToStart = 3
};

public class UILerp : MonoBehaviour
{
    private Collider _collider;
    
    public Transform EndTransform;
    private GameObject _startTransformGameObject;

    public float time;

    [SerializeField]//temp
    private LerpState _state = LerpState.AtStartPostion;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<Collider>();
        GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prim.transform.position = transform.position;
        prim.transform.rotation = transform.rotation;
        prim.transform.localScale = transform.localScale;
        Destroy(prim.GetComponent<Renderer>());
        Destroy(prim.GetComponent<Collider>());
        Destroy(prim.GetComponent<Mesh>());
        prim.name = gameObject.name + "Startposition";
        _startTransformGameObject = prim;

		//if (SearchEngine.Instance == null)
		//{
			DontDestroyOnLoad(_startTransformGameObject);
			DontDestroyOnLoad(EndTransform.gameObject);

		//}
      
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Space)) {
        //    StartLerp();
        //}

        switch(_state) {
            case LerpState.ToEnd:
                if(LerpToPosition(EndTransform, Time.deltaTime * 10)) {
                    _collider.enabled = false;
                    _state = LerpState.AtEndpositon;
                }
                break;
            case LerpState.ToStart:
                if(LerpToPosition(_startTransformGameObject.transform, Time.deltaTime * 10)) {
                    _collider.enabled = true;
                    _state = LerpState.AtStartPostion;
                }
                break;
        }    

    }
    void OnMouseDown() {
        StartLerp();
    }


    public void StartLerp() {
        switch(_state) {
            case LerpState.AtStartPostion:
                _state = LerpState.ToEnd;
                break;
            case LerpState.AtEndpositon:
                _state = LerpState.ToStart;
                break;
        }
    }


    private bool LerpToPosition(Transform pTargetTransform,float pTime) {
        transform.position = Vector3.Lerp(transform.position, pTargetTransform.position, pTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, pTargetTransform.rotation, pTime);
        if(Vector3.Distance(transform.position,pTargetTransform.position) <=0.1f) {
            return true;
        }
        return false;
    }

}
