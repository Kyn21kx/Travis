﻿using System.Collections;
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
    public bool dashed;
    Transform targetTransform;
    Vector3 dashDir;
    [SerializeField]
    LayerMask mask;
    public float dashTime;
    private float auxDashTime;
    private SmoothMovement movRef;
    private StaminaManager staminaRef;
    public bool canDash;
    private LockOn lockRef;
    private Combat combatRef;
    #endregion
    #region statsVars
    public float speed = 50f;
    public float staminaCost = 10f;
    #endregion

    private void Start() {
        canDash = true;
        lockRef = GetComponent<LockOn>();
        staminaRef = GetComponent<StaminaManager>();
        movRef = GetComponent<SmoothMovement>();
        combatRef = GetComponent<Combat>();
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
            if (lockRef.locking || combatRef.attacking) {
                dashDir = movRef.input != Vector2.zero ? movRef.movPivot.transform.forward : lockRef.closest_enemy.forward;
            }
            else {
                dashDir = movRef.input != Vector2.zero ? transform.forward : -transform.forward;
            }
            //Condition for the dash direction to go forward or backwards
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
