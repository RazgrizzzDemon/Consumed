using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public GameObject target;
    public GameObject rotObj;
    public Vector3 positionOffset;
    public float manualSpeed;
    public int manualAxis = 2;
    float targetRadius;
    public static Vector3 currentAngle;

    public static bool isManual = false;

    private void Awake() {
        isManual = false;
    }

    // Update is called once per frame
    void LateUpdate () {
        if (isManual) {
            Manual();
            return;
        }
        MoveWithTarget();
	}

    // Move With Target
    void MoveWithTarget() {
        Vector3 _pos = positionOffset;
        targetRadius = TargetRadius();
        currentAngle = target.transform.eulerAngles;
        _pos[1] += targetRadius;
        gameObject.transform.localPosition = _pos;
        rotObj.transform.eulerAngles = currentAngle;
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

    // Manual Mode
    void Manual() {
        int _direction = 0;
        // Rotate Right
        if (Input.GetKey(KeyCode.D)) {
            _direction = -1;
        }
        // Rotate Left
        else if (Input.GetKey(KeyCode.A)) {
            _direction = 1;
        }
        // Update Position
        currentAngle[manualAxis] += ((_direction * manualSpeed) * Time.deltaTime);
        rotObj.transform.eulerAngles = currentAngle;
    }
}
