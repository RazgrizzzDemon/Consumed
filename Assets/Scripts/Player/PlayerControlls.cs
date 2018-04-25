using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : MonoBehaviour {

    public GameObject world;
    public float moveSpeed = 15f;
    public float jumpForce;
    bool canJump = true;
    bool isJump = false;
    Vector3 moveDir;
    Vector3 jumpDir;
    Rigidbody playerRigidBody;
    public float WorldRadius {
        get { return worldRadius; }
        set {// World radius + bounds of creature
            value += gameObject.GetComponent<Renderer>().bounds.size[1] / 2f;
            worldRadius = value;
        }
    }
    float worldRadius;
    float distanceFromCore; // The players distance from the plantes core
    float worldAngleDeg;
    float orientationOffset = -90f;
    GameObject modelTransformGrp;
    Animator animator;

    private void Awake() {
        playerRigidBody = GetComponent<Rigidbody>();
        modelTransformGrp = gameObject.transform.GetChild(0).gameObject;
        animator = modelTransformGrp.transform.GetChild(0).gameObject.GetComponent<Animator>();
        worldRadius = world.GetComponent<Renderer>().bounds.size[0] / 2f;
    }

    // Update is called once per frame
    void Update () {
        CalculateAngleToWorld();
        OrientToWolrdsSurface(2);
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        if(canJump && Input.GetKeyDown(KeyCode.Space)) {
            canJump = false;
            isJump = true;
            jumpDir = CalculateWorldPosition(worldAngleDeg, jumpForce);
        }
        // rotate model towards direction
        if(moveDir[0] > 0) {
            modelTransformGrp.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        }
        else if(moveDir[0] < 0) {
            modelTransformGrp.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
        }
        // Bite
        if(Input.GetAxisRaw("Fire1") > 0 && !animator.GetBool("BiteAnim")) {
            animator.SetBool("BiteAnim", true);
        }
        else if (Input.GetAxisRaw("Fire1") == 0) {
            animator.SetBool("BiteAnim", false);
        }
    }

    // POSITIONING AND MOVEMENT ---------------------------------------------------------------------------------
    Vector3 CalculateWorldPosition(float _angle, float _mulitplier = 1, int _xAxis = 0, int _yAxis = 1) {
        Vector3 _pos = new Vector3();
        // Cos angle will give us the x Axis
        _pos[_xAxis] = (Mathf.Cos(_angle * Mathf.Deg2Rad) * _mulitplier);
        // Sine angle will give us the Y axis
        _pos[_yAxis] = (Mathf.Sin(_angle * Mathf.Deg2Rad) * _mulitplier);
        return _pos;
    }

    // Calculates Angle position in refernace to the worlds position
    void CalculateAngleToWorld() {
        // Update World Pos
        Vector3 worldPos = transform.position;
        distanceFromCore = Mathf.Sqrt(((Mathf.Pow(worldPos[1], 2) + Mathf.Pow(worldPos[0], 2)) - (2 * worldPos[1] * worldPos[0] * (Mathf.Cos(90f * Mathf.Deg2Rad)))));
        // Normalize positions
        float posX = worldPos[0] / distanceFromCore;
        float posY = worldPos[1] / distanceFromCore;
        // Get new Angle
        worldAngleDeg = Mathf.Acos(posX) * Mathf.Rad2Deg;
        if (Mathf.Asin(posY) * Mathf.Rad2Deg < 0) {
            worldAngleDeg = 360f - worldAngleDeg;
        }
    }

    // Orient to wolrds surface
    void OrientToWolrdsSurface(int _axis) {
        Vector3 _rot = new Vector3(0f, 0f, 0f);
        float _angle = worldAngleDeg + orientationOffset;
        if (float.IsNaN(_angle)) {
            _angle = 0f;
        }
        else if (_angle >= 360f) {
            _angle -= 360f;
        }
        else if (_angle < 0f) {
            _angle = 360f + _angle;
        }
        _rot[_axis] = _angle;
        transform.localEulerAngles = _rot;
    }


    private void FixedUpdate() {
    playerRigidBody.MovePosition(playerRigidBody.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
        if (isJump) {
            playerRigidBody.AddForce(jumpDir);
            isJump = false;
        }
    }

    // TRIGGERS ----------------------------------------------------------
    private void OnTriggerStay(Collider other) {
        if (animator.GetBool("BiteAnim") && other.gameObject.tag.Equals("LifeForm")) {
            CreaturesBase creatureBase = other.gameObject.GetComponent<CreaturesBase>();
            if (creatureBase.isAlive) {
                GetComponent<PlayerStats>().Grow(creatureBase.Harvest());
                creatureBase.Die();
            }
        }
    }

    // COLIDERS ----------------------------------------------------------
    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.tag.Equals("World")) {
            canJump = true;
        }
    }
}
