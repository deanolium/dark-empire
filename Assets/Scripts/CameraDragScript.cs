using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDragScript : MonoBehaviour {
    private Vector3 oldPosition;
    private Vector3 panOrigin;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            oldPosition = transform.position;
            panOrigin = Input.mousePosition;
            Debug.Log(panOrigin);
        }

        if (Input.GetMouseButton(0))
        {

            Vector3 deltaPos = (Input.mousePosition - panOrigin) / 32.0f;
            Debug.Log(panOrigin + "--" + deltaPos);

            transform.position = oldPosition - deltaPos;
            Debug.Log(oldPosition + "::" + transform.position);
        }
    }
}
