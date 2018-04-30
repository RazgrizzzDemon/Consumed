using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats: MonoBehaviour {

    // Physical
    [Header("Physical")]
    public static float growSize = 1f; // equivalent to age
    public float eatingUpperEdge = 0.1f; // Allows you to eat slightly larger ceratures
    public  static float maxGrowSize = 5f;
    public  float growRate;
    int evolutionStage = 0;
    const int MAX_EVOLUTIONS = 3;
    float evolutionIncriment;

    [Space]
    // Damage
    [Header("Damage")]
    public Color damageColor;
    public int glowingTimes;
    public float glowDuration;
    Color defaultDamageColor = new Color(0f, 0f, 0f, 1f);
    Color currentGlowColor = new Color(0f, 0f, 0f, 1f);
    public Material alienMat;
    int currentGlowTimes = 0;
    float currentGlowDuration = 0f;
    bool isDamageGlow = false;
    int glowDirection = 1;
    

    // Stats
    static float health = 100f;
    static float hunger = 100f;

    // Negatives (per day)
    static float hungerRate = 1f;
    static float feedingMultiplier = 4f;
    static float starvationDmg = 10f;

    // MONOBEHAVIOUR -------------------------------------------------------
    private void Update() {
        DamageGlow();
    }

    // METHODS -------------------------------------------------------------
    // Calculates at which stage should the alien evolve
    public void EvolutionIncrimentUpdate() {
        evolutionIncriment = (maxGrowSize - growSize) / MAX_EVOLUTIONS;
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
            if(_growSize >= 1 + (evolutionIncriment * (evolutionStage + 1)) || _growSize >= maxGrowSize) {
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
            // Replanish hunger
            hunger += ((hungerRate * feedingMultiplier) * _foodIntake);
            // Limnit hunger to 100
            if (hunger > 100f) {
                hunger = 100f;
                // Replanish life
                if(health < 100f) {
                    HealthUpdate(0, ((hungerRate * feedingMultiplier) * _foodIntake));
                }
            }
        }
    }

    // Get Damage
    public void GetDamage(float _dmg) {
        HealthUpdate(_dmg);
        isDamageGlow = true;
    }

    // Damage Glow
    void DamageGlow() {
        // Check if should glow
        if (!isDamageGlow) {
            return;
        }
        // Update Time
        currentGlowDuration += Time.deltaTime;
        // Switch glow
        if(currentGlowDuration >= glowDuration) {
            currentGlowDuration = 0;
            // Reverse direction
            glowDirection *= -1;
            // Count
            if(glowDirection > 0) {
                currentGlowTimes++;
                // End Glow
                if(currentGlowTimes == glowingTimes) {
                    currentGlowTimes = 0;
                    currentGlowDuration = 0;
                    isDamageGlow = false;
                    return;
                }
            }
            
        }
        // Check percentage of glow
        float _normalized = currentGlowDuration / glowDuration;
        if(glowDirection == -1) {
            _normalized = 1 - _normalized;
        }
        currentGlowColor = Color.Lerp(defaultDamageColor, damageColor, _normalized);
        // Colour Update
        alienMat.SetColor("_EmissionColor", currentGlowColor);
    }

    // Health Update - by hunger or attacks
    static void HealthUpdate(float _dmg = 0, float _foodIntake = 0) {
        // Hunger
        if(hunger == 0) {
            health -= starvationDmg;
        }
        // Attack Dmg
        if(_dmg > 0) {
            health -= _dmg;
        }
        // health Replanish
        if(_foodIntake > 0) {
            health += _foodIntake;
            if (health > 100f) {
                health = 100f;
            }
        }
        // Limit Health
        if(health < 0) {
            health = 0;
        }
        // Check if player is dead
        if(health <= 0) {
            Debug.Log("Dead - Game Over");
            PlayerControlls.controlLock = true;
        }
    }

    public float GetSize() {
        return growSize;
    }

}
