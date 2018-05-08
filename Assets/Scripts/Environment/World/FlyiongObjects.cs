using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyiongObjects : MonoBehaviour {

    float ground;
    public float height;
    public float flySpeed;
    public int zLayerNumber = -1; // 0 is the furthest away
    public bool isOscillate = false;
    public int flightAxis = 2;
    public int direction = 1;

    float currentFlightAngle = 0f;
    GameObject rotateObj;

    // CONSTRUCTOR ---------------------------------------------------------
    public void Constructor(float _height, float _speed, int _axis, int _dir) {
        height = _height;
        flySpeed = _speed;
        flightAxis = _axis;
        direction = _dir;
    }

    // MONOBEHAVIOUR --------------------------------------------------------
    private void Awake() {
        rotateObj = new GameObject(gameObject.name + "_flightRotator");
        gameObject.transform.parent = rotateObj.transform;
    }

    // Use this for initialization
    void Start () {
        if(zLayerNumber != -1) {
            ZlayerAndGroundSetup(zLayerNumber);
        }
    }
	
	// Update is called once per frame
	void Update () {
        Fly();
	}

    // METHODS ------------------------------------------------------------------
    public void ZlayerAndGroundSetup(int _zLayer) {
        ground = BiomeController.worldRadius;
        zLayerNumber = _zLayer;
        HeightUpdate();
    }

    public void OverrideAngle(float _angle) {
        currentFlightAngle = _angle;
    }

    void Fly() {
        // Update current angle
        currentFlightAngle += ((flySpeed * Time.deltaTime) * direction);
        // Limit
        if(currentFlightAngle > 359) {
            currentFlightAngle -= 360f;
        }
        else if(currentFlightAngle < 0) {
            currentFlightAngle += 360f;
        }
        // Set New Euler Angles
        Vector3 _angle = new Vector3();
        _angle[flightAxis] = currentFlightAngle;
        rotateObj.transform.eulerAngles = _angle;
    }

    void HeightUpdate() {
        gameObject.transform.localPosition = new Vector3(0, ground + height, BiomeController.zDepthLayers[zLayerNumber]);
    }

    public void SpinObject(int _axis, float _spinAngle) {
        Vector3 _angle = new Vector3();
        _angle[_axis] = _spinAngle;
        gameObject.transform.localEulerAngles = _angle;
    }

    public void ScaleObject(float _scale) {
        gameObject.transform.localScale = new Vector3(_scale, _scale, _scale);
    }
}
