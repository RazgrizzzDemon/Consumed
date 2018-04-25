using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public GameObject target;
    public GameObject rotObj;
    public Vector3 positionOffset;
    //public Vector3 rotationOffset;

	// Update is called once per frame
	void Update () {
        MoveWithTarget();
	}

    // Move With Target
    void MoveWithTarget() {
        Vector3 _pos = positionOffset;
        Vector3 _angle = target.transform.eulerAngles;
        if(_angle[2] == 0) {
            _pos[1] += target.transform.position.y;
        }
        else { // Y is used instead of X because of the 90 degree offset. Therefor adjacent = Y, Opposite = X
            _pos[1] += target.transform.position.y / (Mathf.Cos(_angle[2] * Mathf.Deg2Rad));
        }
        
        gameObject.transform.localPosition = _pos;
        rotObj.transform.eulerAngles = (_angle);
    }
}
