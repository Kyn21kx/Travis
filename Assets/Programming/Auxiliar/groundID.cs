using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundID : MonoBehaviour {
    //TODO: Add behaviours for the enemy jump
    private void OnCollisionStay(Collision collision) {
        if (collision.transform.CompareTag("Player")) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<SmoothMovement>().grounded = true;
            GameObject.FindGameObjectWithTag("Player").GetComponent<SmoothMovement>().closestFloor = transform;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.transform.CompareTag("Player")) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<SmoothMovement>().grounded = false;
        }
    }
}
