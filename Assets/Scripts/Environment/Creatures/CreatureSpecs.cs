using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Species/creatures", order = 1)]
public class CreatureSpecs : ScriptableObject {

    public string speciesType = "";
    public string speciesName = "";
    public GameObject modelPrefab;
    public Material matreial;
    int totalCreatures;
    public int startHerdLimit;
    public int nestsNum;
    public float maxSpeed;
    public float maxJumpForce;
    public float hitPoints;
    public int gestationDays;
    public int zLayerNumber; // 0 is the furthest away
    [HideInInspector]
    public float worldZdepth;
    public bool limitMovment = true;
    public Mesh skeletonMesh;
    [HideInInspector]
    public GameObject[] species;

    // INITIALIZE ---------------------------------------------------------------
    public void Initialize(float _worldRadius, ref GameObject _attractor) {
        TotalCreatures(true);
        species = new GameObject[totalCreatures];
        // Name container
        BiomeController.LifeContainers[BiomeController.currentLifeContainer] = new GameObject(speciesName);
        for (int i = 0; i < species.Length; i++) {
            // Instantiate
            species[i] = MonoBehaviour.Instantiate(modelPrefab, BiomeController.LifeContainers[BiomeController.currentLifeContainer].transform);
            // Name
            species[i].name = speciesName + " " + i;
            // Add Script and Attractor - World
            species[i].AddComponent<CreaturesBase>().InitializePositioning(_worldRadius, ref _attractor, species[i].GetComponent<CreaturesBase>().maxSize);
            // Add material
            species[i].GetComponent<Renderer>().material = matreial;
            // Set Index
            species[i].GetComponent<CreaturesBase>().indexId = i;
            // Move Setup
            species[i].GetComponent<CreaturesBase>().MoveSetup(maxSpeed, maxJumpForce);
            // Hit points
            species[i].GetComponent<CreaturesBase>().hitPoints = hitPoints;
            // Gestation Days
            species[i].GetComponent<CreaturesBase>().GestationDays(gestationDays);
            // Set World Radius
            species[i].GetComponent<CreaturesBase>().WorldRadius = _worldRadius;
            // Setup skeleton
            species[i].GetComponent<CreaturesBase>().SkeletonSetup(skeletonMesh);
            // Set Inactive
            species[i].GetComponent<CreaturesBase>().Terminate();
        }
        BiomeController.currentLifeContainer++;
    }
    // METHODS ---------------------------------------------------------------
    // Get total Amount of cretaures (All alive)
    public int TotalCreatures(bool _SetUp = false) {
        if (_SetUp) {
            totalCreatures = startHerdLimit * nestsNum;
        }
        return totalCreatures;
    }

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
        species[_index].GetComponent<CreaturesBase>().InitializeSpecies(speciesType, speciesName, false, false, true);
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

    // Daily Update
    public void DailyUpdate() {
        for (int i = 0; i < species.Length; i++) {
            if (CheckIfAlive(i)) {
                species[i].GetComponent<CreaturesBase>().DailyUpdate();
            }
        }
    }

    // Regenerate
    public void Regenerate(string _name = "") {
        if(_name.Equals("") || _name.Equals(speciesName)) {
            species[0].GetComponent<CreaturesBase>().InitializeSpecies("", "", true, true);
            species[1].GetComponent<CreaturesBase>().InitializeSpecies("", "", true, false);
            BiomeController.HealthUpdate(true);
            BiomeController.HealthUpdate(true);
        }
    }
}
