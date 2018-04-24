using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGravAttraction : MonoBehaviour {

    public float gravity = -9.81f;
    public float bodyRotationalSpeed = 50f;

    public void Attract(Transform body, bool noOrient = false) {
        // Fall towars target
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        // Point towards target
        if (!noOrient) {
            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, bodyRotationalSpeed * Time.deltaTime);
        }
    }
}
