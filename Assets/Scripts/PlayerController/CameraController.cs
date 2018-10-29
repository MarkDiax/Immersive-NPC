using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Vector2 _mouseLook;
    private Vector2 _smoothV;
    public float sensitivity = 5.0f;
    public float smooting = 2.0f;

    public float maxAngle = 300f, minAngle = 25f;

    private GameObject _character;

	// Use this for initialization
	void Start () {
        _character = this.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smooting, sensitivity * smooting));
        _smoothV.x = Mathf.Lerp(_smoothV.x, mouseDelta.x, 1.0f / smooting);
        _smoothV.y = Mathf.Lerp(_smoothV.y, mouseDelta.y, 1.0f / smooting);
        _mouseLook += _smoothV;
        if(_mouseLook.y > maxAngle) {
            _mouseLook.y = maxAngle;
        }
        if(_mouseLook.y < minAngle) {
            _mouseLook.y = minAngle;
        }
        transform.localRotation = Quaternion.AngleAxis(-_mouseLook.y, Vector3.right);
        _character.transform.localRotation = Quaternion.AngleAxis(_mouseLook.x, _character.transform.up);
    }
}
