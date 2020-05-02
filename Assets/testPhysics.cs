using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPhysics : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Sword")) {
            var rig = GetComponent<Rigidbody>();
            rig.AddForce(other.transform.forward * 5000f);
        }
    }
}
