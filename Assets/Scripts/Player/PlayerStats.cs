using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats: MonoBehaviour {

    // Physical
    float growSize = 1f;
    public  float maxGrowSize;
    public  float growRate;
    int evolutionStage = 0;
    const int MAX_EVOLUTIONS = 3;
    float evolutionIncriment;

    // Stats
    float health = 100f;
    float stamina = 100f;
    float hunger = 100f;

    public void EvolutionIncrimentUpdate() {
        evolutionIncriment = maxGrowSize / MAX_EVOLUTIONS;
    }

    public void Grow(float _foodIntake) {
        growSize += ((_foodIntake * growRate) );
        if(growSize > maxGrowSize) {
            growSize = maxGrowSize;
        }
        ScaleUpdate(growSize);
    }

    void ScaleUpdate(float _growSize) {
        if(gameObject.transform.localScale.x != growSize) {
            // Grow
            gameObject.transform.localScale = new Vector3(_growSize, _growSize, _growSize);
            // Check for evolution
            if(_growSize >= (evolutionIncriment * (evolutionStage + 1))) {
                // Evolve
                Debug.Log("EVOLVE!!!");
                evolutionStage++;
                ++UIController.avaialbleSkills;
                // Show Powers Panel
                if (!UIController.arePowersAvailable) {
                    UIController.powerPanel_isSwitchPos = true;
                }
            }
        }
    }

}
