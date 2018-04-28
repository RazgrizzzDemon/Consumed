using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vegitation", menuName = "Species/vegitation", order = 1)]
public class VegitationSpecs : ScriptableObject {

    public string speciesName = "";
    public GameObject[] modelPrefab;
    public int maxPlantationLimit;
    public int startPlantationLimit;
    public int plantationsNum;
    public int maxAge;
    public int zLayerNumber; // 0 is the furthest away
    [HideInInspector]
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
            if (modelPrefab.Length > 1) {
                _modelSelect = UnityEngine.Random.Range(0, modelPrefab.Length);
            }
            species[i] = MonoBehaviour.Instantiate(modelPrefab[_modelSelect], speicesContainer.transform);
            // Name
            species[i].name = speciesName + " " + i;
            // Add Script and Attractor - World
            species[i].AddComponent<CreaturesBase>().InitializePositioning(_worldRadius, ref _attractor, species[i].GetComponent<CreaturesBase>().maxSize, pivotIsBase, isSprites);
            // Set Index
            species[i].GetComponent<CreaturesBase>().indexId = i;
            // Set World Radius
            species[i].GetComponent<CreaturesBase>().WorldRadius = _worldRadius;
            // Setup skeleton
            if (deadVegMesh.Length > 0) {
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
