using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Species/creatures", order = 1)]
public class SpeciesSpecs: ScriptableObject {

    public string speciesName = "";
    public GameObject modelPrefab;
    public Material matreial;
    public int maxHerdLimit;
    public int startHerdLimit;
    public int nestsNum;
    public float maxSpeed;
    public float maxJumpForce;
    public float worldZdepth;
    public bool limitMovment = true;
    public Mesh skeletonMesh;
    [HideInInspector]
    public GameObject[] species;
    [HideInInspector]
    public GameObject speicesContainer;

    // INITIALIZE ---------------------------------------------------------------
    public void Initialize(float _worldRadius, ref GameObject _attractor) {
        species = new GameObject[maxHerdLimit];
        speicesContainer = new GameObject(speciesName);
        for (int i = 0; i < species.Length; i++) {
            // Instantiate
            species[i] = MonoBehaviour.Instantiate(modelPrefab, speicesContainer.transform);
            // Name
            species[i].name = speciesName + " " + i;
            // Add Script and Attractor - World
            species[i].AddComponent<CreaturesBase>().InitializePositioning(_worldRadius, ref _attractor);
            // Add material
            species[i].GetComponent<Renderer>().material = matreial;
            // Set Index
            species[i].GetComponent<CreaturesBase>().indexId = i;
            // Move Setup
            species[i].GetComponent<CreaturesBase>().MoveSetup(maxSpeed, maxJumpForce);
            // Set World Radius
            species[i].GetComponent<CreaturesBase>().WorldRadius = _worldRadius;
            // Setup skeleton
            species[i].GetComponent<CreaturesBase>().SkeletonSetup(skeletonMesh);
            // Set Inactive
            species[i].GetComponent<CreaturesBase>().Terminate();
        }
    }
    // METHODS ---------------------------------------------------------------
    // Check if it is still Alive
    bool CheckIfAlive(int _index) {
        return species[_index].GetComponent<CreaturesBase>().isAlive;
    }
    // Spawn to world
    public void InitSpawn(int _index, float _angle, float _limitAngle = 0) {
        if (limitMovment) {
            species[_index].GetComponent<CreaturesBase>().PositionInWorld(_angle, worldZdepth, _limitAngle);
        }
        else {
            species[_index].GetComponent<CreaturesBase>().PositionInWorld(_angle, worldZdepth);
        }
        species[_index].GetComponent<CreaturesBase>().InitializeSpecies(speciesName, true);
    }
    // Auto Move
    public void AutoMove() {
        for (int i = 0; i < species.Length; i++) {
            if (CheckIfAlive(i)) {
                species[i].GetComponent<CreaturesBase>().AutoMove();
            }
        }
    }
    // Move Update - RigidBody (Fixed Update)
    public void MoveUpdate() {
        for (int i = 0; i < species.Length; i++) {
            if (CheckIfAlive(i)) {
                species[i].GetComponent<CreaturesBase>().MoveUpdate();
            }
        }
    }
    // Age
    public void AgeUpdate() {
        for (int i = 0; i < species.Length; i++) {
            if (CheckIfAlive(i)) {
                species[i].GetComponent<CreaturesBase>().AgeUpdate();
            }
        }
    }
}

[CreateAssetMenu(fileName = "Vegitation", menuName = "Species/vegitation", order = 1)]
public class VegitationSpecs: ScriptableObject {

    public string speciesName = "";
    public GameObject[] modelPrefab;
    public int maxPlantationLimit;
    public int startPlantationLimit;
    public int plantationsNum;
    public int maxAge;
    public float worldZdepth;
    public Mesh[] deadVegMesh;
    public bool isRandomRotY = false;
    public bool isSprites;
    public bool pivotIsBase;
    [HideInInspector]
    public GameObject[] species;
    [HideInInspector]
    public GameObject speicesContainer;

    // INITIALIZE ---------------------------------------------------------------
    public void Initialize(float _worldRadius, ref GameObject _attractor) {
        species = new GameObject[maxPlantationLimit];
        speicesContainer = new GameObject(speciesName);
        for (int i = 0; i < species.Length; i++) {
            // Instantiate
            int _modelSelect = 0;
            if(modelPrefab.Length > 1) {
                _modelSelect = UnityEngine.Random.Range(0, modelPrefab.Length);
            }
            species[i] = MonoBehaviour.Instantiate(modelPrefab[_modelSelect], speicesContainer.transform);
            // Name
            species[i].name = speciesName + " " + i;
            // Add Script and Attractor - World
            species[i].AddComponent<CreaturesBase>().InitializePositioning(_worldRadius, ref _attractor, pivotIsBase , isSprites);
            // Set Index
            species[i].GetComponent<CreaturesBase>().indexId = i;
            // Set World Radius
            species[i].GetComponent<CreaturesBase>().WorldRadius = _worldRadius;
            // Setup skeleton
            if(deadVegMesh.Length > 0) {
                species[i].GetComponent<CreaturesBase>().SkeletonSetup(deadVegMesh[_modelSelect]);
                // Set Inactive
                species[i].GetComponent<CreaturesBase>().Terminate();
            }
            else {
                species[i].SetActive(false);
            }
        }
    }
    // METHODS ---------------------------------------------------------------
    // Activate
    public void Active(int _index, bool _ans) {
        species[_index].SetActive(_ans);
    }
    // Spawn to world
    public void InitSpawn(int _index, float _angle) {
        species[_index].GetComponent<CreaturesBase>().PositionInWorld(_angle, worldZdepth, 0, isRandomRotY);
        species[_index].GetComponent<CreaturesBase>().ConditionsSetUp(maxAge);
        species[_index].GetComponent<CreaturesBase>().InitializeSpecies(speciesName, true);
    }
    // Check if it is still Alive
    bool CheckIfAlive(int _index) {
        return species[_index].GetComponent<CreaturesBase>().isAlive;
    }
    // Age
    public void AgeUpdate() {
        for (int i = 0; i < species.Length; i++) {
            if (CheckIfAlive(i)) {
                species[i].GetComponent<CreaturesBase>().AgeUpdate();
            }
        }
    }
}

public class CreaturesController : MonoBehaviour {

    enum CreatureType { herbivour, carnivour}
    enum VegitationType { grass, trees}

    // World
    [Header("World")]
    public GameObject world;
    float worldRadius;
    static float worldClock = 0f;
    static float oneYearTime = 10f;
    static int worldYear = 0;

    [Space]
    // Creatures
    [Header("Creatures")]
    public SpeciesSpecs[] speciesSpecs;
    [Space]
    [Header("Vegitation")]
    public VegitationSpecs[] vegitationSpecs;
    private void Awake() {
        // Get World Radius
        worldRadius = (world.GetComponent<Renderer>().bounds.size.x / 2f);
        // Create Species
        for (int i = 0; i < speciesSpecs.Length; i++) {
            speciesSpecs[i].Initialize(worldRadius, ref world);
            // Posiion
            NestDistributor(i);
        }
        // Vegitation
        for (int i = 0; i < vegitationSpecs.Length; i++) {
            vegitationSpecs[i].Initialize(worldRadius, ref world);
            // Position
            PlantationDistributor(i);
        }
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        WorldTimeUpdate();
        // Auto MOve
        for (int i = 0; i < speciesSpecs.Length; i++) {
            speciesSpecs[i].AutoMove();
        }
    }

    private void FixedUpdate() {
        // Update Rigid body
        for (int i = 0; i < speciesSpecs.Length; i++) {
            speciesSpecs[i].MoveUpdate();
        }
    }

    // Wolrd Time Update
    void WorldTimeUpdate() {
        // Time
        worldClock += Time.deltaTime;
        if(worldClock >= oneYearTime) {
            Debug.Log("Year ----------------------------------------------");
            worldYear++;
            worldClock = 0f;
            // Creatures Age
            for (int i = 0; i < speciesSpecs.Length; i++) {
                speciesSpecs[i].AgeUpdate();
            }
            // Vegitation Age
            for (int i = 0; i < speciesSpecs.Length; i++) {
                vegitationSpecs[i].AgeUpdate();
            }
        }
    }

    // Nest Distributor
    void NestDistributor(int _type) {
        float _speciesSize = speciesSpecs[_type].species[0].GetComponent<CreaturesBase>().fullOccupationAngle;
        float _NestSize =  _speciesSize * speciesSpecs[_type].startHerdLimit;
        float[] _NestsPos = new float[speciesSpecs[_type].nestsNum];
        float _NestAngle = 360f / speciesSpecs[_type].nestsNum;
        float _nextPos = 0;
        int j = 0;
        for (int i = 0; i < _NestsPos.Length; i++) {
            // Set nest Angle
            _NestsPos[i] = _NestAngle * (i + 1);
            // Instantiate and position
            for (; j < speciesSpecs[_type].startHerdLimit * (i + 1); j++) {
                // Check if first on Nest
                if(j + 1 == ((i + 1) * speciesSpecs[_type].startHerdLimit)) {
                    // if yes its possition is on angle limit
                    _nextPos = (_NestsPos[i] + (_NestSize / 2f)) - _speciesSize;
                }
                else {
                    _nextPos -= (_speciesSize + (_speciesSize / 2f));
                }
                speciesSpecs[_type].InitSpawn(j, _nextPos, _NestSize);
            }
        }
    }

    // Plantation Distributor
    void PlantationDistributor(int _type) {
        float _speciesSize = vegitationSpecs[_type].species[0].GetComponent<CreaturesBase>().fullOccupationAngle;
        float _NestSize;
        // Fill Planet
        if (vegitationSpecs[_type].startPlantationLimit == 0) {
            vegitationSpecs[_type].startPlantationLimit = vegitationSpecs[_type].maxPlantationLimit;
            vegitationSpecs[_type].plantationsNum = 1;
            _NestSize = 360f;
        }
        // Plantation
        else {
            _NestSize = _speciesSize * vegitationSpecs[_type].startPlantationLimit;
        }
        float[] _NestsPos = new float[vegitationSpecs[_type].plantationsNum];
        float _NestAngle = 360f / vegitationSpecs[_type].plantationsNum;
        float _nextPos = 0;
        int j = 0;
        for (int i = 0; i < _NestsPos.Length; i++) {
            // Set nest Angle
            _NestsPos[i] = _NestAngle * (i + 1);
            // Instantiate and position
            for (; j < vegitationSpecs[_type].startPlantationLimit * (i + 1); j++) {
                // Check if first on Nest
                if (j + 1 == ((i + 1) * vegitationSpecs[_type].startPlantationLimit)) {
                    // if yes its possition is on angle limit
                    _nextPos = (_NestsPos[i] + (_NestSize / 2f)) - _speciesSize;
                }
                else {
                    _nextPos -= _speciesSize;
                }
                vegitationSpecs[_type].Active(j, true);
                vegitationSpecs[_type].InitSpawn(j, _nextPos);
            }
        }
    }
}
