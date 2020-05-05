using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class Dash : MonoBehaviour {
    /*
     * TODO:
     */
    #region Varibles
    public float time;
    private Rigidbody rg;
    private bool dashed;
    Transform targetTransform;
    Vector3 dashDir;
    [SerializeField]
    LayerMask mask;
    public float dashTime;
    private float auxDashTime;
    private SmoothMovement movRef;
    private StaminaManager staminaRef;
    public bool canDash;
    #endregion
    #region statsVars
    public float speed = 50f;
    public float staminaCost = 10f;
    #endregion

    private void Start() {
        canDash = true;
        staminaRef = GetComponent<StaminaManager>();
        movRef = GetComponent<SmoothMovement>();
        dashed = false;
        auxDashTime = dashTime;
        rg = GetComponent<Rigidbody>();
        targetTransform = transform;
        dashDir = -targetTransform.forward;
    }

    private void Update() {
        if (!dashed) {
            _Input();
        }
        _Dash();
    }

    private bool TimingDownDash() {
        dashTime -= Time.deltaTime;
        if (dashTime <= 0f) {
            return true;
        }
        else {
            return false;
        }
    }

    private void _Input () {
        //Raycast
        if (Input.GetButtonDown("B") && staminaRef.staminaAmount >= staminaCost && canDash) {
            rg.isKinematic = false;
            rg.velocity *= 0f;
            dashed = true;
            targetTransform = transform;
            if (GetComponent<LockOn>().locking) {
                targetTransform = movRef.movPivot.transform;
            }
            //Condition for the dash direction to go forward or backwards
            dashDir = movRef.input != Vector2.zero ? targetTransform.forward : -targetTransform.forward;
            rg.AddForce(dashDir * speed, ForceMode.VelocityChange);
            movRef.canMove = false;
            GetComponent<Combat>().canMeleeAttack = false;
            staminaRef.Reduce(staminaCost);
        }
    }
    private void _Dash () {
        if (dashed) {
            if (TimingDownDash()) {
                StopDash();
            }
        }
    }

    public void StopDash () {
        dashTime = auxDashTime;
        dashed = false;
        rg.velocity *= 0f;
        GetComponent<Combat>().canMeleeAttack = true;
        movRef.canMove = true;
    }

}
