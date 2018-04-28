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
    static float health = 100f;
    static float hunger = 100f;

    // Negatives (per day)
    static float hungerRate = 1f;
    static float feedingMultiplier = 4f;
    static float starvationDmg = 10f;

    // Calculates at which stage should the alien evolve
    public void EvolutionIncrimentUpdate() {
        evolutionIncriment = maxGrowSize / MAX_EVOLUTIONS;
    }

    // Takes car of Alien Groth size
    public void Grow(float _foodIntake) {
        HungerUpdate(_foodIntake);
        growSize += ((_foodIntake * growRate) );
        if(growSize > maxGrowSize) {
            growSize = maxGrowSize;
        }
        ScaleUpdate(growSize);
    }

    // Re scale Alien model
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

    // Daily Update - Processed each day (BiomeContoller)
    public static void DailyUpdate() {
        HealthUpdate();
        HungerUpdate();
        UIController.StatsUpdate(health, hunger);
    }

    // Hunger Update - diminishes by time and increases by food
    static void HungerUpdate(float _foodIntake = 0) {
        // Decrease hunger daily
        if(_foodIntake == 0 && hunger > 0) {
            hunger -= hungerRate;
            if(hunger < 0) {
                hunger = 0;
            }
        }
        // Feed
        else if(_foodIntake > 0) {
            hunger += ((hungerRate * feedingMultiplier) * _foodIntake);
            if (hunger > 100f) {
                hunger = 100f;
            }
        }
    }

    // Health Update - by hunger or attacks
    static void HealthUpdate() {
        // Hunger
        if(hunger == 0) {
            health -= starvationDmg;
        }
        // Attack Dmg

        // Limit Health
        if(health < 0) {
            health = 0;
        }
        else if(health > 100f) {
            health = 100f;
        }
        // Check if player is dead
        if(health <= 0) {
            Debug.Log("Dead - Game Over");
        }
    }


}
