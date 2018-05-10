using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flying Types", menuName = "Flying Types / Clouds", order = 1)]
public class FlyingTypes : ScriptableObject {

    [Header("Amount")]
    public int amountToSpawn = 1;

    [Header ("Properties")]
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.5f;
    public float minHeight = 10f;
    public float maxHeight = 25f;
    public float minScale = 1f;
    public float maxScale = 2f;
    int zLayerMaxNumber = 1;
    [Range(0,2)]
    public int flightAxis;

    [Space]
    [Header ("Meshes and Material")]
    public Mesh[] meshtypes;
    public Material objsMaterial;

    [Space]
    [Header("Other options")]
    public bool randomInitRot = false;
    public bool disperse = false;

    GameObject[] flyingObjs;

    public void Initialize() {
        flyingObjs = new GameObject[amountToSpawn];
        for (int i = 0; i < flyingObjs.Length; i++) {
            // 3D Model Creation
            flyingObjs[i] = new GameObject("cloud_" + i);
            flyingObjs[i].AddComponent<MeshFilter>().mesh = meshtypes[Random.Range(0, meshtypes.Length)];
            flyingObjs[i].AddComponent<MeshRenderer>().material = objsMaterial;
            flyingObjs[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            flyingObjs[i].GetComponent<MeshRenderer>().motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            // Height
            float _hieght = Random.Range(minHeight, maxHeight);
            // Speed
            float _speed = Random.Range(minSpeed, maxSpeed);
            // Random Direction
            int _dir = RandomDirection();
            // Add FlyingObject component
            flyingObjs[i].AddComponent<FlyiongObjects>();
            flyingObjs[i].GetComponent<FlyiongObjects>().Constructor(_hieght, _speed, flightAxis, _dir);
            // Spin
            if (randomInitRot) {
                int _rot = Random.Range(0, 360);
                flyingObjs[i].GetComponent<FlyiongObjects>().SpinObject(1, _rot);
            }
            // Disperse
            if (disperse) {
                int _disp = Random.Range(0, 360);
                flyingObjs[i].GetComponent<FlyiongObjects>().OverrideAngle(_disp);
            }
            // Scale
            flyingObjs[i].GetComponent<FlyiongObjects>().ScaleObject(Random.Range(minScale, maxScale));
        }
    }

    // Use this for initialization
    public void Start () {
        // Add Z layer after world has fully generated
        for (int i = 0; i < flyingObjs.Length; i++) {
            // Random Z layer
            int _zLayer = Random.Range(0, zLayerMaxNumber);
            // Update Layer
            flyingObjs[i].GetComponent<FlyiongObjects>().ZlayerAndGroundSetup(_zLayer);
        }
	}

    // METHODS ---------------------------------------------------------------------------------------------
    int RandomDirection() {
        int _dir = Random.Range(0, 2);
        if (_dir == 0) {
            _dir = -1;
        }
        else {
            _dir = 1;
        }
        return _dir;
    }
}