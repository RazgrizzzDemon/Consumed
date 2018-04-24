using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesBase: CreatureWorldPositioning {

    // Others
    Mesh skeletonMesh;
    Mesh creatureMesh;

    // Stats
    public int indexId;
    public string type; // What kinf of creature is it.. Herbivor or carnivor
    public int age = 1; // Age determins if he is able to hunt, or riproduce
    int maxAge = 100; // The maximum age a creature can live, at max age creatures die
    public bool isMale = false; // Finds a partner to reproduce
    public float size; // the actual size of the creature (scale of object)
    float maxSize = 2f; // The maximum size a creature can grow
    float minSize = 0.5f; // The minimum size a creature can be born

    // Needs
    public bool isAlive = false;
    float health = 100f;
    float hunger = 100f;

    // CONDITIONS -------------------------------------------------------
    public void ConditionsSetUp(int _maxAge, float _maxSize = 2f, float _minSize = 0.5f) {
        maxAge = _maxAge;
        maxSize = _maxSize;
        minSize = _minSize;
    }
    // INITIALIZE --------------------------------------------------------
    // Species type and behavior - on Awake is used when game is started to have rendom ages
    public void InitializeSpecies(string _type, bool onAwake = false) {
        // Live
        isAlive = true;
        SkeletonMeshUpdate();
        gameObject.SetActive(true);
        // Age
        if (onAwake) {
            // Set random Age
            age = Random.Range(1, maxAge); // max age is exclded since certure dies at max age
        }
        else {
            age = 1;
        }
        // Set random Sex
        int _sex = Random.Range(0, 2);
        if(_sex == 1) {
            isMale = true;
        }
        // Set Size to Age
        UpdateSize();
    }

    // TERMINATE ---------------------------------------------------------
    // no longer active, generaly after decomposition
    public void Terminate() {
        isAlive = false;
        SkeletonMeshUpdate();
        gameObject.SetActive(false);
    }

    // METHODS -----------------------------------------------------------
    // Calculate Size
    void UpdateSize() {
        size = minSize + (((float)age / maxAge) * (maxSize - minSize));
        transform.localScale = new Vector3(size, size, size);
    }

    // Age Update
    public void AgeUpdate() {
        age++;
        // Die
        if(age == maxAge) {
            isAlive = false;
            // Spawn skeleton
            SkeletonMeshUpdate();
        }
        // grow
        else {
            UpdateSize();
        }
    }

    // Search for partner and reproduce
    public void Reproduce() {
    }

    // Skeleton Mesh setup, Setup Meshes
    public void SkeletonSetup(Mesh _skeleton) {
        skeletonMesh = _skeleton;
        creatureMesh = gameObject.GetComponent<MeshFilter>().mesh;
    }

    // Sekelton Update
    void SkeletonMeshUpdate() {
        if(skeletonMesh != null) {
            Mesh _mesh = skeletonMesh;
            if (isAlive) {
                _mesh = creatureMesh;
            }
            gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            // Deactivate rigidbody
            if (gameObject.GetComponent<Rigidbody>()) {
                gameObject.GetComponent<Rigidbody>().detectCollisions = isAlive;
                gameObject.GetComponent<Rigidbody>().isKinematic = !isAlive;
            }
            // If Dead place on ground
            if (!isAlive) {
                PositionInWorld(worldAngleDeg, worldZdepth);
            }
            // Deactivate gravitational Pull
            isMovable = isAlive;
            
        }
        else {
            Debug.LogWarning("Need to setup Skeleton before");
        }
    }
}

public class CreatureWorldPositioning: MonoBehaviour {

    enum Angles { small, large};

    // World
    public WorldGravAttraction attractor;
    float orientationOffset = -90f;
    public float worldZdepth;
    int clockwise = 1;
    int antiClockwise = -1;
    Vector3 bounds;
    float worldRadius;
    public Vector3 worldPos; // z remians always to the ceratures depth in the world
    public float worldAngleDeg = 0f; // The angle (degrees) in referance to the worlds center
    public float fullOccupationAngle; // The angle occupided by the creature
    float distanceFromCore; // Distance of creature from the core of the planet. Note there is no offset set!
    public float WorldRadius {
        get { return worldRadius; }
        set {// World radius + bounds of creature
            if (!pivotIsBase) {
                value += bounds[1] / 2f;
            }
            worldRadius = value;
        }
    }

    // Creature
    Rigidbody creatureRigid;
    Transform creatureTransform;
    public bool isMovable = false; // Wheter creature is movable or static on the world. ex: plants = static, pig = movable.
    public float moveSpeed;
    bool isSprite = false;
    bool pivotIsBase = false;

    // Auto Move
    bool isGrounded = false;
    bool isGoingBack = false;
    Vector2 moveLimits = new Vector2(0f, 0f); // angle limits from its spawn position
    public bool isHop = false; // Herbivors hop, canrnivors strall
    bool isHoping = false; // true if it during a hop
    bool shouldHop = false; // Fixed update teel it if should hop or not
    public float jumpForce = 0f;
    Vector3 moveDir = new Vector3(0f, 0f, 1f);
    Vector3 jumpDir;
    float currentDirection = 0f;
    float curTime = 0f;
    float waitingTime = 0f;
    float moveTime = 0f;
    float lastColided = 0f;
    

    // INITIALIZER ----------------------------------------------------------
    public void InitializePositioning(float _radius, ref GameObject _attractor, bool _pivotIsBase = false ,bool _isSprite = false) {
        // World attractor
        attractor = _attractor.GetComponent<WorldGravAttraction>();
        // Is pivot located at base
        pivotIsBase = _pivotIsBase;
        // Bounds of creature
        if (_isSprite) {
            isSprite = _isSprite;
            bounds = GetComponent<SpriteRenderer>().bounds.size;
        }
        else {
            bounds = GetComponent<Renderer>().bounds.size;
        }
        // Transform
        creatureTransform = transform;
        // World radius + bounds of creature
        WorldRadius = _radius;
        // World Ocupation
        float _sideC = Mathf.Sqrt(Mathf.Pow(_radius, 2f) + Mathf.Pow(bounds[0] / 2f, 2));
        float _halfAngle = Mathf.Asin((Mathf.Sin(90f * Mathf.Deg2Rad) / _sideC) * (bounds[0] / 2f)) * Mathf.Rad2Deg;
        fullOccupationAngle = (_halfAngle * 2f);
    }

    // SETUP --------------------------------------------------------------
    public void MoveSetup(float _moveSpeed, float _jumpForce = 0f) {
        moveSpeed = _moveSpeed;
        if(moveSpeed != 0) {
            isMovable = true;
            // Rigidbody assignment
            creatureRigid = GetComponent<Rigidbody>();
            creatureRigid = GetComponent<Rigidbody>();
            creatureRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            creatureRigid.useGravity = false;
        }
        if (_jumpForce > 0f) {
            isHop = true;
            jumpForce = _jumpForce;
        }
    }

    // METHODS -----------------------------------------------------------
    // Position in world - 3D object with rigid body
    public void PositionInWorld(float _angle, float _zDepth, float _limitAngle = 0f, bool randomRotY = false) {
        // Update Z Depth
        worldZdepth = _zDepth;
        // Update Angle
        worldAngleDeg = _angle;
        // Calculate Position
        worldPos = CalculateWorldPosition(_angle, WorldRadius);
        // Upodate z on world pos
        worldPos[2] = worldZdepth;
        // Update position
        creatureTransform.position = worldPos;
        // If Sprite Orient to world
        if (!isMovable) {
            OrientToWolrdsSurface(2, randomRotY);
        }
        // Set Limits
        if (_limitAngle > 0) {
            moveLimits[(int)Angles.small] = worldAngleDeg - (_limitAngle / 2f);
            moveLimits[(int)Angles.large] = worldAngleDeg + (_limitAngle / 2f);
        }
    }

    // Calculates position in word relative to the angle
    Vector3 CalculateWorldPosition(float _angle, float _mulitplier = 1, int _xAxis = 0, int _yAxis = 1) {
        Vector3 _pos = new Vector3();
        // Cos angle will give us the x Axis
        _pos[_xAxis] = (Mathf.Cos(_angle * Mathf.Deg2Rad) * _mulitplier);
        // Sine angle will give us the Y axis
        _pos[_yAxis] = (Mathf.Sin(_angle * Mathf.Deg2Rad) * _mulitplier);
        return _pos;
    }

    // Calculates Angle position in refernace to the worlds core
    void CalculateAngleToWorld() {
        // Update World Pos
        worldPos = creatureTransform.position;
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

    // AutoMove creature in world
    public void AutoMove() {
        if (!isMovable) {
            return;
        }
        // Time update
        curTime += Time.deltaTime;
        // Check if creature should Stop
        if (currentDirection != 0 && curTime >= moveTime) {
            // This will reset movement
            if (isHop) {
                isHoping = false;
                lastColided = 0f;
            }
            currentDirection = 0; 
            curTime = waitingTime;
        }
        // Keep in Limits
        GoBack();
        // Move or Stop
        if(currentDirection != 0) {
            // Hop
            if (isHop && !isHoping) {
                // Generate a direction
                isGrounded = false;
                isHoping = true;
                shouldHop = true;
                jumpDir = CalculateWorldPosition(worldAngleDeg, jumpForce);
            }
            else if(isHop && isHoping && isGrounded) {
                isHoping = false;
            }
        }
        // Wait
        else {
            // If times up, check what to do next
            if(curTime >= waitingTime) {
                curTime = 0f;
                GenerateDirection();
                isGoingBack = false;
                if (currentDirection == 0) {
                    // for how long should it stay idle?
                    waitingTime = Random.Range(1f, 5f);
                }
                else {
                    // for how long should it move
                    moveTime = Random.Range(2f, 5f);
                }
            }
        }
        attractor.Attract(creatureTransform, true);
    }

    // Gebnerate a random direction to go to
    void GenerateDirection() {
        currentDirection = Random.Range(antiClockwise, clockwise + 1);
    }

    // Go Back to Limits
    void GoBack() {
        // Check if creature is limited or not
        if(moveLimits[(int)Angles.small] == 0 && moveLimits[(int)Angles.large] == 0) {
            return;
        }
        // Check conditions to go back
        // Abnormal
        if(moveLimits[(int)Angles.small] < 0f || moveLimits[(int)Angles.large] >= 360f) {
            float _sml = moveLimits[(int)Angles.small];
            float _lrg = moveLimits[(int)Angles.large];
            // Negative Small Angle
            if (_sml < 0f) {
                // Change to inrange angle
                _sml = 360f + _sml;
            }
            // Greater Then 360 Large Angke
            if(_lrg > 360f) {
                // Change to inrange angle
                _lrg = _lrg - 360f;
            }
            // Codition to go back
            if(!isGoingBack && (worldAngleDeg > _lrg && worldAngleDeg < _sml)) {
                // Check direction by subtraction - the smaller the number the closser
                float _lrgSub = worldAngleDeg - _lrg;
                float _smlSub = _sml - worldAngleDeg;
                // Clockwise
                if(_lrgSub < _smlSub) {
                    currentDirection = clockwise;
                }
                // Anti clockwise
                else if(_smlSub < _lrgSub) {
                    currentDirection = antiClockwise;
                }
                OrientToWolrdsSurface(0);
                isGoingBack = true;
            }
            // It is inside limits turn off go back
            else {
                isGoingBack = false;
            }
        }
        // Normal
        else {
            if (!isGoingBack && (worldAngleDeg > moveLimits[(int)Angles.large] || worldAngleDeg < moveLimits[(int)Angles.small])) {
                if (worldAngleDeg > moveLimits[(int)Angles.large]) {
                    currentDirection = clockwise;
                }
                else {
                    currentDirection = antiClockwise;
                }
                OrientToWolrdsSurface(0);
                isGoingBack = true;
            }
            else if (isGoingBack && (worldAngleDeg < (moveLimits[(int)Angles.large] - fullOccupationAngle) && worldAngleDeg > (moveLimits[(int)Angles.small] + fullOccupationAngle))) { // Stop going back
                isGoingBack = false;
            }
        }        
    }

    // Move in world using riggid body
    public void MoveUpdate() {
        if (!isMovable) {
            return;
        }
        OrientToWolrdsSurface(0);
        // jump
        if (isHop && shouldHop) {
            if(currentDirection < 0) {
                jumpDir *= -1;
            }
            creatureRigid.AddForce(jumpDir);
            shouldHop = false;
        }
        // move
        creatureRigid.MovePosition(creatureRigid.position + creatureTransform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
        // Calculate new Angle
        CalculateAngleToWorld();
    }

    // Orient to wolrds surface (Tangential)
    void OrientToWolrdsSurface(int _axis, bool _randomRotY = false) {
        Vector3 _rot = new Vector3(0f, 0f, 0f);
        int _selectRotY = 0;
        int _newRot = 0;
        // Static Objects - random rotation on Y
        if (_randomRotY) {
            _selectRotY = Random.Range(0, 4);
            switch (_selectRotY) {
                case 1:
                    _newRot = 90;
                    _axis = 0;
                    break;
                case 2:
                    _newRot = 180;
                    break;
                case 3:
                    _newRot = 270;
                    _axis = 0;
                    break;
            }
        }
        float _angle = worldAngleDeg + orientationOffset;
        if (float.IsNaN(_angle)) {
            _angle = 0f;
        }
        else if (_angle >= 360f) {
            _angle -= 360f;
        }
        else if(_angle < 0f) {
            _angle = 360f + _angle;
        }
        _rot[_axis] = _angle;

        if (isMovable) {
            if(currentDirection == clockwise) {
                _rot[1] = 90f;
                _rot[_axis] *= -1;
            }
            else if( currentDirection == antiClockwise) {
                _rot[1] = 270f;
            }
            else {
                _rot[1] = 270f;
            }
        }
        // Static Objects - Upadte New rotation
        if (_randomRotY) {
            if(_selectRotY == 1 || _selectRotY == 2) {
                _rot *= -1;
            }
            _rot[1] = _newRot;
        }
        creatureTransform.localEulerAngles = _rot;
    }

    // COLIDERS -----------------------------------------
    private void OnCollisionStay(Collision collision) {
        if (isHop && curTime >= lastColided +  0.1f && collision.gameObject.tag.Equals("World")) {
            lastColided = curTime;
            isGrounded = true;
        }
    }
}
