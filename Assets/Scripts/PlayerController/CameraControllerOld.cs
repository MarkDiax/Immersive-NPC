using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerOld : MonoBehaviour {


    public Transform target;

    [System.Serializable]
    public class PositionSettings {
        public Vector3 targetOffset = new Vector3(0, 3.4f, 0);
        public float lookSmooth = 100f;
        public float distanceFromTarget = -8;
        public float zoomSmooth = 10;
        public float maxZoom = -2;
        public float minZoom = -15;
    }
    [System.Serializable]
    public class OrbitSettings {
        public float xRotation = -20;
        public float yRotation = -180;
        public float maxXRotation = 25;
        public float minxRotation = -85;
        public float vOrbitSmooth = 150;
        public float hOrbitSmooth = 150;

    }
    [System.Serializable]
    public class InputSettings {
        public string orbit_Horizontal_Snap = "OrbitHorizontalSnap";
        public string orbit_Horizontal = "OrbitHorizontal";
        public string orbit_Vertical = "OrbitVertical";
        public string zoom = "Mouse ScrollWheel";
    }

    public PositionSettings postionSettings = new PositionSettings();
    public OrbitSettings orbitSettings = new OrbitSettings();
    public InputSettings inputSettings = new InputSettings();

    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _destination;
    private PlayerController _playerController;
    float vOrbitInput, hOrbitInput, zoomInput, hOrbitSnapInput;

	// Use this for initialization
	void Start () {
        SetCameraTarget(target);

        _targetPosition = target.position + postionSettings.targetOffset;
        _destination = Quaternion.Euler(orbitSettings.xRotation, orbitSettings.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * postionSettings.distanceFromTarget;
        _destination += _targetPosition;
        transform.position = _destination;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        MoveToTargt();
        LootToTarget();
	}

    void Update() {
        GetInput();
        OrbitTarget();
        ZoomInOnTarget();
    }

    private void MoveToTargt() {
        _targetPosition = target.position + postionSettings.targetOffset;
        _destination = Quaternion.Euler(orbitSettings.xRotation, orbitSettings.yRotation + target.eulerAngles.y, 0) * - Vector3.forward *postionSettings.distanceFromTarget;
        _destination += _targetPosition;
        transform.position = _destination;
    }

    private void LootToTarget() {
        //float eulerYAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref _rotateVelocity, lookSmooth);
        //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, eulerYAngle, transform.eulerAngles.z);
        Quaternion targetRotation = Quaternion.LookRotation(_targetPosition - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, postionSettings.lookSmooth * Time.deltaTime);
    }

    private void GetInput() {
        vOrbitInput = Input.GetAxisRaw(inputSettings.orbit_Vertical);
        hOrbitInput = Input.GetAxisRaw(inputSettings.orbit_Horizontal);
        hOrbitSnapInput = Input.GetAxisRaw(inputSettings.orbit_Horizontal_Snap);
        zoomInput = Input.GetAxisRaw(inputSettings.zoom);
    }

    private void OrbitTarget() {
        if(hOrbitSnapInput > 0) {
            orbitSettings.yRotation = -180;
        }

        orbitSettings.xRotation += -vOrbitInput * orbitSettings.vOrbitSmooth * Time.deltaTime;
        orbitSettings.yRotation += -hOrbitInput * orbitSettings.hOrbitSmooth * Time.deltaTime;

        if(orbitSettings.xRotation > orbitSettings.maxXRotation) {
            orbitSettings.xRotation = orbitSettings.maxXRotation; 
        }
        if(orbitSettings.xRotation < orbitSettings.minxRotation) {
            orbitSettings.xRotation = orbitSettings.minxRotation;
        }
    }

    private void ZoomInOnTarget() {
        postionSettings.distanceFromTarget += zoomInput * postionSettings.zoomSmooth * Time.deltaTime;
        if(postionSettings.distanceFromTarget > postionSettings.maxZoom ) {
            postionSettings.distanceFromTarget = postionSettings.maxZoom;
        }
        if(postionSettings.distanceFromTarget < postionSettings.minZoom) {
            postionSettings.distanceFromTarget = postionSettings.minZoom;
        }
    }


    public void SetCameraTarget(Transform pTransform) {
        target = pTransform;
        if(target != null) {
            if(target.GetComponent<PlayerController>() != null) {
                _playerController = target.GetComponent<PlayerController>();
            } else {
                Debug.LogError("The Target for the CameraContoller needs a playercontoller!");
            }

        } else {
            Debug.LogError("The CameraController needs a target to at!");
        }
    }
}
