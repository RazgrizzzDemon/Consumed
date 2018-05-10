using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDirection { None, Left, Right, Up, Down}

public class SwipeManager : MonoBehaviour {

    static SwipeManager instance;
    public static SwipeManager Instance { get { return instance; } }

    public SwipeDirection swipeDirection { set; get; }

    Vector3 touchPosition;
    float swipeResistanceRadius = 50f;
    float radius = 0f;
    Vector2 normalizedVector = new Vector2();
    Vector3 rangePos = new Vector3();
    Vector2 xyRange = new Vector2();
    float swipeAngle = 0f;

    private void Awake() {
        instance = this;
        touchPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
    }

    // Update is called once per frame
    void Update () {
        //Swipe();
        Pan();
    }

    void Swipe() {
        swipeDirection = SwipeDirection.None;

        // Check foir mouse down
        if (Input.GetMouseButtonDown(0)) {
            touchPosition = Input.mousePosition;
        }
        // While mouse button is held down.. Track Movement
        else if (Input.GetMouseButton(0)) {
            // Calculate Distance
            GetSwipeData();
            // limit initial movment with a rqadius.
            // If the distance is smaller do nothing
            if(radius >= swipeResistanceRadius) {
                // Check direction of swipe by angle
                CheckAngle();
            }
        }
    }

    void Pan() {
        swipeDirection = SwipeDirection.None;
        // Calculate Distance
        GetPanData();
        // limit initial movment with a rqadius.
        // If the distance is smaller do nothing
        if (radius >= swipeResistanceRadius) {
            // Check direction of swipe by angle
            CheckAngle();
        }
    }

    void GetSwipeData() {
        // Actual Range
        rangePos = Input.mousePosition - touchPosition;
        // Positive range
        Vector2 _xyRange = rangePos;
        _xyRange[0] = MakePositive(_xyRange[0]);
        _xyRange[1] = MakePositive(_xyRange[1]);
        // Radius
        radius = Mathf.Sqrt(Mathf.Pow(_xyRange[0], 2f) + Mathf.Pow(_xyRange[1], 2f));
        normalizedVector = new Vector2(rangePos[0] / radius, rangePos[1] / radius);
        // Get Angle
        swipeAngle = Mathf.Acos(normalizedVector[0]) * Mathf.Rad2Deg;
        if(rangePos[1] < 0) {
            swipeAngle = 360 - swipeAngle;
        }
    }

    void GetPanData() {
        // Actual Range
        rangePos = Input.mousePosition - touchPosition;
        // Positive range
        xyRange = rangePos;
        // Radius
        radius = Mathf.Sqrt(Mathf.Pow(xyRange[0], 2f) + Mathf.Pow(xyRange[1], 2f));
        normalizedVector = new Vector2(rangePos[0] / radius, rangePos[1] / radius);
        // Get Angle
        swipeAngle = Mathf.Acos(normalizedVector[0]) * Mathf.Rad2Deg;
        if (rangePos[1] < 0) {
            swipeAngle = 360 - swipeAngle;
        }
    }

    void CheckAngle() {
        // Going Left
        if((swipeAngle <= 45f && swipeAngle >= 0) || (swipeAngle > 315f) && swipeAngle < 360f) {
            swipeDirection = SwipeDirection.Right;
        }
        else if(swipeAngle > 45f && swipeAngle <= 135f) {
            swipeDirection = SwipeDirection.Up;
        }
        else if(swipeAngle > 135f && swipeAngle <= 225f) {
            swipeDirection = SwipeDirection.Left;
        }
        else if(swipeAngle > 225f && swipeAngle <= 315f) {
            swipeDirection = SwipeDirection.Down;
        }
    }

    float MakePositive(float _num) {
        if(_num < 0) {
            _num *= -1;
        }
        return _num;
    }

    public bool IsSwiping(SwipeDirection dir) {
        if (dir == swipeDirection) {
            return true;
        }
        else return false;
    }

    public float GetSwipeForce() {
        return radius;
    }

    public float GetSwipeAngle() {
        return swipeAngle;
    }

    public Vector2 GetNormalizedVector() {
        return normalizedVector;
    }

    public Vector2 GetNormailizedXYrange(){
        return xyRange;
    }

}
