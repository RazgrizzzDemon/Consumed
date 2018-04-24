using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldGravityBody : MonoBehaviour {

    public WorldGravAttraction attractor;
    Transform bodyTransforms;
    Rigidbody bodyRigidbody;

    private void Awake() {
        bodyRigidbody = GetComponent<Rigidbody>();
        bodyRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        bodyRigidbody.useGravity = false;
        bodyTransforms = transform;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        attractor.Attract(bodyTransforms, true);
	}
}
