using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockOn : MonoBehaviour {

    #region Variables
    private Vector3 target;
    [SerializeField]
    Transform collisionAux;
    public Transform closest_enemy;
    public float distanceThreshold;
    public bool locking;
    private bool onetime = false;
    private bool isCollided = true;
    Vector3 dir = Vector3.zero;
    CameraControl camControlRef;
    Vector2 viewportTarget, viewportPlayer;
    #endregion

    private void Awake() {
        camControlRef = Camera.main.transform.parent.GetComponent<CameraControl>();
        Cursor.lockState = CursorLockMode.Locked;
        target = Vector3.zero;
    }
    //Check if there is a collision in between the player and the enemy, if there is, do not turn around
    private void Update() {
        _Input();
        var _target = GetTarget();
        if (locking) {
            if (_target != Vector3.zero) {
                LockCamera(_target);
                Quaternion rotation = Quaternion.LookRotation(_target);
                rotation.x = 0f;
                rotation.z = 0f;
                collisionAux.rotation = rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 15f * Time.deltaTime);
            }
            else {
                locking = false;
            }
        }
        else {
            UnlockCamera();
            target = Vector3.zero;
        }
    }

    private void _Input () {
        if (Input.GetButtonDown("RS") && !locking) {
            locking = true;
        }
        else if (Input.GetButtonDown("RS") && locking) {
            locking = false;
        }
    }

    //Find the nearest target
    //Make this a general behaviours method
    private Vector3 GetTarget () {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDis = distanceThreshold;
        if (target == Vector3.zero && !locking) {
            foreach (var enemy in enemies) {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance <= minDis) {
                    minDis = distance;
                    dir = enemy.transform.position - transform.position;
                    closest_enemy = enemy.transform;
                }
            }
        }
        else {
            dir = closest_enemy != null ? closest_enemy.position - transform.position : Vector3.zero;
        }
        return dir;
    }

    private void LockCamera(Vector3 rot) {
        camControlRef.canControl = false;
        camControlRef.transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
    }

    private void UnlockCamera () {
        camControlRef.canControl = true;
    }
}
