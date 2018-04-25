using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public GameObject target;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;

	// Update is called once per frame
	void Update () {
        MoveWithTarget();
	}

    // Move With Target
    void MoveWithTarget() {
        Vector3 _pos = target.transform.position;
        _pos += positionOffset;
        gameObject.transform.position = _pos;
        gameObject.transform.eulerAngles = (target.transform.eulerAngles + rotationOffset);
    }
}
