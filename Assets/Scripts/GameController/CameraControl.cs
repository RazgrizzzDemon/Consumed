using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public GameObject target;
    public GameObject rotObj;
    public Vector3 positionOffset;
    float targetRadius;
    //public Vector3 rotationOffset;

	// Update is called once per frame
	void LateUpdate () {
        MoveWithTarget();
	}

    // Move With Target
    void MoveWithTarget() {
        Vector3 _pos = positionOffset;
        targetRadius = TargetRadius();
        Vector3 _angle = target.transform.eulerAngles;
        _pos[1] += targetRadius;
        gameObject.transform.localPosition = _pos;
        rotObj.transform.eulerAngles = (_angle);
    }

    // Find target Hypotenius
    float TargetRadius() {
        Vector3 _targetPos = target.transform.position;
        float _radius = 0f;
        if(_targetPos[0] == 0) {
            _radius = _targetPos[1];
        }
        else if(_targetPos[1] == 0) {
            _radius = _targetPos[0];
        }
        // Pytagoras
        else {
            _radius = Mathf.Sqrt(Mathf.Pow(_targetPos[0], 2f) + Mathf.Pow(_targetPos[1], 2f));
        }
        return _radius;
    }
}
