using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseTracker : MonoBehaviour {

    public float minWith, maxWith, minHeight, maxHeight;
    public float withBorderPercentage = 25, heightBoderPercentage = 25;
 
    private Vector2 mousePosition;

    float speed = 10.0f;
    int boundary = 1;
    int width;
    int height;

    void Start() {
        //float sliceWith = Screen.width / withBorderPercentage;
        //minWith = sliceWith;
        //maxWith = Screen.width - sliceWith;
        //float sliceHeight = Screen.height / heightBoderPercentage;
        //minHeight = sliceHeight;
        //maxHeight = Screen.height - sliceHeight;
        width = Screen.width;
        height = Screen.height;
    }
	
	// Update is called once per frame
	void Update () {
        //  mousePosition = Input.mousePosition;
        if(Input.mousePosition.x > width - boundary) {
            transform.position -= new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed,0,0);
        }

        if(Input.mousePosition.x < 0 + boundary) {
            transform.position -= new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, 0, 0);
        }

        if(Input.mousePosition.x > height - boundary) {
            transform.position -= new Vector3(0, 0, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed);
        }

        if(Input.mousePosition.x < 0 - boundary) {
            transform.position -= new Vector3(0, 0, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed);
        }


    }
}
