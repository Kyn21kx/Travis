using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCollider : MonoBehaviour
{

    #region Variables
    public float minDistance;
    public float maxDistance;
    public float smoothness;
    private Vector3 dir;
    public float distance;
    public LayerMask collisionMask;
    #endregion
    //Initialize the variables of the direction
    private void Awake() {
        dir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    private void Update() {
        //Calculate a new position 
        Vector3 newPos = transform.parent.TransformPoint(dir * maxDistance);
        RaycastHit hit;
        if (Physics.Linecast(transform.parent.position, newPos, out hit, collisionMask)) {
            //Reduce the distance of the detection by 10%
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
        }
        else {
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dir * distance, Time.deltaTime * smoothness);
    }

}
