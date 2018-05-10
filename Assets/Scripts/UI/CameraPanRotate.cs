using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanRotate : MonoBehaviour {

    [Header("Camera Limits")]
    public float transLimitX;
    public float transLimitY;
    public float rotLimitX;
    public float rotLimitY;
    public float sensitivity = 1f;

    GameObject cameraTransformObj;
    Vector3 initialPos;
    Vector2 mouseMaxDistace = new Vector2();

    private void Awake() {
        SetUp();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        MoveCamera();
	}

    // Setup Camera rotator
    void SetUp() {
        // Create Transform Object
        cameraTransformObj = new GameObject("Camera_Transform");
        initialPos = Camera.main.transform.position;
        cameraTransformObj.transform.position = initialPos;
        Camera.main.transform.parent = cameraTransformObj.transform;

        mouseMaxDistace[0] = (Screen.width / 2f);
        mouseMaxDistace[1] = (Screen.height / 2f);

        Cursor.visible = false;
    }

    // Move Camera
    void MoveCamera() {
        // Get X and Y of mouse pos from initial pos
        Vector2 _xyRange = SwipeManager.Instance.GetNormailizedXYrange();
        // Normalize
        Vector2 _normalizedXYrange = new Vector2();
        _normalizedXYrange[0] = _xyRange[0] / mouseMaxDistace[0];
        _normalizedXYrange[1] = _xyRange[1] / mouseMaxDistace[1];
        // If greater Limit
        if(_normalizedXYrange[0] > 1) {
            _normalizedXYrange[0] = 1;
        }
        else if(_normalizedXYrange[0] < -1) {
            _normalizedXYrange[0] = -1;
        }
        if (_normalizedXYrange[1] > 1) {
            _normalizedXYrange[1] = 1;
        }
        else if (_normalizedXYrange[1] < -1) {
            _normalizedXYrange[1] = -1;
        }
        // Calculate Rotation
        Vector3 _cameraRot = new Vector3(0f, 0f, 0f);
        _cameraRot[0] = (_normalizedXYrange[1] / 1f) * rotLimitX;
        _cameraRot[1] = (_normalizedXYrange[0] / 1f) * rotLimitY;
        //Debug.Log("Mouse Pos: " + Input.mousePosition);
        //Debug.Log("Limits XY: " + rotLimitX + " , " + rotLimitY);
        //Debug.Log("Camera Rot: " + _cameraRot);
        //Debug.Log("Normilized: " + _normalizedXYrange);
        //Debug.Log("---------------------------------------");
        // Update Camera Rotation
        cameraTransformObj.transform.localEulerAngles = _cameraRot;
        // Calculate Transformation
        Vector3 _cameraPos = initialPos;
        _cameraPos[0] += (_normalizedXYrange[0] / 1f) * transLimitX;
        _cameraPos[1] -= (_normalizedXYrange[1] / 1f) * transLimitY;
        // Update Transformation
        cameraTransformObj.transform.position = _cameraPos;
    }
}
