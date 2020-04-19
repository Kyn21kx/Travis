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
    public float maxDis;
    private float auxDis;
    public float time;
    private Rigidbody rg;
    private bool dashed;
    Transform targetTransform;
    [SerializeField]
    LayerMask mask;
    public float dashTime;
    private float auxDashTime;
    #endregion
    #region statsVars
    public float speed = 50f;
    public float staminaCost = 10f;
    #endregion

    private void Start() {
        auxDis = maxDis;
        dashed = false;
        auxDashTime = dashTime;
        rg = GetComponent<Rigidbody>();
        targetTransform = transform;
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
        if (Input.GetButtonDown("B") && GetComponent<StaminaManager>().staminaAmount >= staminaCost) {
            dashed = true;
            targetTransform = transform;
            if (GetComponent<LockOn>().locking) {
                targetTransform = GetComponent<SmoothMovement>().movPivot.transform;
            }
            rg.AddForce(targetTransform.forward * speed, ForceMode.VelocityChange);
            GetComponent<SmoothMovement>().canMove = false;
            GetComponent<Combat>().canMeleeAttack = false;
            GetComponent<StaminaManager>().Reduce(staminaCost);
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
        GetComponent<SmoothMovement>().canMove = true;
    }

}
