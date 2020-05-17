using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Programming;
using UnityEngine.Rendering.HighDefinition;

public class LockOn : MonoBehaviour {

    #region Variables
    public Vector3 offset;
    public Transform closest_enemy;
    public float distanceThreshold;
    public bool locking;
    private bool onetime = false;
    Vector3 dir = Vector3.zero;
    public Vector3 rotOffset;
    private Vector3 auxOffset;
    CameraControl camControlRef;
    private bool enemiesAround;
    GeneralBehaviours generalBehaviours;
    #endregion

    private void Awake() {
        enemiesAround = false;
        auxOffset = offset;
        generalBehaviours = new GeneralBehaviours();
        camControlRef = Camera.main.transform.parent.GetComponent<CameraControl>();
        Cursor.lockState = CursorLockMode.Locked;
        dir = Vector3.zero;
    }
    //Check if there is a collision in between the player and the enemy, if there is, do not turn around
    private void Update() {
        Adjustments();
        _Input();
        TargetChecking();
    }
    private void TargetChecking () {
        var _target = GetTarget();
        if (locking) {
            if (_target != Vector3.zero) {
                LockCamera(_target);
            }
            else {
                locking = false;
            }
        }
        else {
            if (!onetime)
                UnlockCamera();
            dir = Vector3.zero;
        }
    }

    private void _Input () {
        if (Input.GetButtonDown("RS") && !locking || (Input.GetKeyDown(KeyCode.Tab) && !locking)) {
            locking = true;
        }
        else if ((Input.GetButtonDown("RS") && locking) || closest_enemy == null && !enemiesAround && locking ||(Input.GetKeyDown(KeyCode.Tab) && locking)) {
            locking = false;
            onetime = false;
        }
    }

    //Find the nearest target
    //Make this a general behaviours method
    private Vector3 GetTarget () {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null) {
            if (dir == Vector3.zero && !locking) {
                GetNearestEnemy(enemies);
            }
            else {
                dir = closest_enemy != null ? closest_enemy.position - transform.position : Vector3.zero;
            }
        }
        else {
            dir = Vector3.zero;
            enemiesAround = false;
        }
        return dir;
    }

    private void GetNearestEnemy (GameObject [] _enemies) {
        //Separate this into another function
        float minDis = distanceThreshold;
        foreach (var enemy in _enemies) {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance <= minDis) {
                minDis = distance;
                dir = enemy.transform.position - transform.position;
                closest_enemy = enemy.transform;
            }
        }
    }

    private void Adjustments () {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool getEnemies = false;
        if (closest_enemy == null && locking) {
            if (enemies != null)
                GetNearestEnemy(enemies);
            getEnemies = true;
        }
        if (getEnemies) {
            if (closest_enemy != null) {
                Debug.Log("Found");
                enemiesAround = true;
            }
            else {
                enemiesAround = false;
            }
        }
    }


    private void LockCamera(Vector3 rot) {
        Quaternion rotation = Quaternion.LookRotation(rot);
        rotation.x = 0f;
        rotation.z = 0f;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 15f * Time.deltaTime);
        offset = auxOffset;
        camControlRef.canControl = false;
        //Use lerp and then hard lock when you are close to the rotation
        var camRot = Quaternion.LookRotation(-rot + rotOffset);
        camControlRef.transform.rotation = camRot;
    }

    private void UnlockCamera () {
        offset = Vector3.zero;
        onetime = true;
        var prevRotation = new Vector2(camControlRef.transform.eulerAngles.y, 0f);
        camControlRef._rotation = prevRotation;
        camControlRef.transform.rotation = Quaternion.LookRotation(-dir);
        camControlRef.canControl = true;
        closest_enemy = null;
    }
}
