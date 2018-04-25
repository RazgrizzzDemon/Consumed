using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats: MonoBehaviour {

    float growSize = 1f;
    public  float maxGrowSize;
    public  float growRate;

    float health = 100f;
    float stamina = 100f;
    float hunger = 100f;

    public void Grow(float _foodIntake) {
        growSize += ((_foodIntake * growRate) );
        ScaleUpdate(growSize);
    }

    void ScaleUpdate(float _growSize) {
        gameObject.transform.localScale = new Vector3(_growSize, _growSize, _growSize);
    }

}
