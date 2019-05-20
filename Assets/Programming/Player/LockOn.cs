using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour {

    #region Variables
    [SerializeField]
    Transform target, player;
    [SerializeField]
    float distanceToTarget;
    public bool locking;
    Vector2 viewportTarget, viewportPlayer;

    #endregion

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        if (Input.GetAxis("LT") > 0) {
            locking = true;
            Quaternion rotation = Quaternion.LookRotation(GetTarget());
            transform.rotation = rotation;
        }
        else {
            locking = false;
        }
    }

    //Find the nearest target
    private Vector3 GetTarget () {
        Adjustments();
        return target.position - player.position;
    }

    //Adjust the camera
    private void Adjustments () {
        viewportPlayer = Camera.main.WorldToViewportPoint(player.position);
        viewportTarget = Camera.main.WorldToViewportPoint(target.position);
        Debug.Log(Vector2.Distance(viewportPlayer, viewportTarget));
    }

}
