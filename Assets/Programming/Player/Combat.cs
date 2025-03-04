﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Programming;
using System;

public class Combat : MonoBehaviour {

    #region Variables
    public float staminaCost;
    GeneralBehaviours generalBehaviours;
    [SerializeField]
    private float angleOfAssist;
    public Transform sword;
    public float swordDamage;
    public float boostDamage;
    private Animator anim;
    [SerializeField]
    private float turningDistance;
    private SmoothMovement movRef;
    public bool attacking;
    public bool canMeleeAttack;
    private Rigidbody rig;
    private Transform target;
    CapsuleCollider swordColl;
    Rigidbody rigSword;
    [SerializeField]
    private float magneticSwordSpeed;
    private bool oneTimeMagnetic;
    [SerializeField]
    private bool canMagneticMove;
    SpellShooting spellRef;
    [SerializeField]
    private Transform[] basicAttackPos;
    Transform placeForSword;
    [SerializeField]
    private Transform magneticCollider;
    Quaternion attackRot;
    private Dash dashRef;
    [Header("Number of attack animation that you are in")]
    public int attackIndex;
    [HideInInspector]
    public AudioSource source;
    [Header("Attack Sound")]
    public AudioClip clip;
    StaminaManager staminaRef;
    #endregion
    //To Do: Work on magnetic combat so we can have a functioning system
    //To Do: Correct the movement when doing the second Fire attack
    private void Start() {
        attackIndex = 0;
        staminaRef = GetComponent<StaminaManager>();
        dashRef = GetComponent<Dash>();
        source = GetComponent<AudioSource>();
        magneticCollider.gameObject.SetActive(false);
        generalBehaviours = new GeneralBehaviours();
        spellRef = GetComponent<SpellShooting>();
        canMagneticMove = true;
        canMeleeAttack = true;
        oneTimeMagnetic = false;
        rig = GetComponent<Rigidbody>();
        attacking = false;
        anim = GetComponent<Animator>();
        sword = GameObject.FindGameObjectWithTag("Sword").transform;
        swordColl = sword.GetComponent<CapsuleCollider>();
        movRef = GetComponent<SmoothMovement>();
        swordColl.enabled = false;
        rigSword = sword.GetComponent<Rigidbody>();
        rigSword.isKinematic = true;
    }

    private void Update() {
        anim.SetInteger("AttackIndex", attackIndex);
        TypeManager();
        MeleeBehaviours();
        _Input();
    }
    Vector3 pos1;
    //Replace combo manager's input with this one, and also kinda rewrite that whole script, please
    private void _Input () {
        if ((Input.GetButtonDown("X") || Input.GetMouseButtonDown(0)) && canMeleeAttack && movRef.grounded && staminaRef.staminaAmount >= staminaCost) {
            Start_Anim();
        }
    }

    private void GetClosestEnemy () {
        //Calculate the vector going forwards or backwards here
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDis = turningDistance;
        foreach (var enemy in enemies) {
            float newDis = Vector3.Distance(transform.position, enemy.transform.position);
            if (newDis < minDis) {
                minDis = newDis;
                target = enemy.transform;
            }
        }

    }

    private void MeleeBehaviours () {
        if (attacking) {
            //Rotate Player to closest enemy
            if (target != null && generalBehaviours.Detect(target, transform, turningDistance, angleOfAssist)) {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                //Increase accuracy
                rot = new Quaternion(0f, rot.y, 0f, rot.w);
                rig.MoveRotation(Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10f));
            }
            //Activate slash motion
        }
        else {
            GetClosestEnemy();
        }
        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, turningDistance);
        Gizmos.color = Color.red;
        Vector3 fov1 = Quaternion.AngleAxis(angleOfAssist, transform.up) * transform.forward * turningDistance;
        Vector3 fov2 = Quaternion.AngleAxis(-angleOfAssist, transform.up) * transform.forward * turningDistance;
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, fov1);
        Gizmos.DrawRay(transform.position, fov2);
    }

    private void Start_Anim () {
        anim.SetTrigger(spellRef.type.ToString());
        MoveOnAttack();
        movRef.canMove = false;
        dashRef.canDash = false;
        attacking = true;
        canMeleeAttack = false;
        //Update the transform.forward for another value if you are rotating
        pos1 = transform.position;
        spellRef.canCastSpells = false;
        if (spellRef.type.Equals(SpellShooting.Type.Magnetism)) {
            magneticCollider.position = basicAttackPos[0].position;
            attackRot = sword.rotation;
            sword.position = basicAttackPos[0].position;
            swordColl.enabled = true;
        }
    }

    private void SwordSlash() {
        if (attacking) {
            attackIndex++;
            staminaRef.Reduce(staminaCost);
            rig.velocity *= 0f;
            if (spellRef.type.Equals(SpellShooting.Type.Magnetism)) {
                magneticCollider.gameObject.SetActive(true);
            }
            else {
                swordColl.enabled = true;
            }
            movRef.canMove = false;
            canMagneticMove = false;
        }
    }

    private void FinishedStrike () {
        swordColl.enabled = false;
        canMeleeAttack = true;
        dashRef.canDash = true;
    }
    private void HitAnimStart () {
        attacking = false;
        movRef.canMove = false;
        //Change this so you can move with the velocity of the enemy like an impact
        rig.velocity *= 0f;
        canMeleeAttack = false;
        dashRef.canDash = false;
        spellRef.canCastSpells = false;
    }
    Vector3 pos2;
    //Overload for combos
    private void Finished_Anim (int final) {
        //canMagneticMove = true;
        pos2 = transform.position;
        Debug.Log(Vector3.Distance(pos1, pos2));
        rig.velocity *= 0f;
        attacking = false;
        target = null;
        movRef.canMove = true;
        GetComponent<SpellShooting>().canCastSpells = true;
        FinishedStrike();
        canMagneticMove = true;
        magneticCollider.gameObject.SetActive(false);
        StartCoroutine(ResetAttackIndex(0.5f, Convert.ToBoolean(final)));
    }

    private void TypeManager () {

        switch (spellRef.type) {
            case SpellShooting.Type.Fire:
                RestoreSwordPos();
                break;
            case SpellShooting.Type.Electric:
                RestoreSwordPos();
                break;
            case SpellShooting.Type.Ice:
                RestoreSwordPos();
                break;
            case SpellShooting.Type.Magnetism:
                MagneticCombat();
                break;
        }
    }

    private void MagneticCombat () {
        if (!oneTimeMagnetic) {
            anim.SetTrigger("MagneticDrop");
            oneTimeMagnetic = true;
        }
        if (canMagneticMove) {
            MagneticSwordMovement();
        }
        if (attacking) {
            Magnetic_Slashes();
        }
    }

    private void MagneticSwordMovement () {
        //Expression for force multiplier comes from ƒ(h) = -10.9h + 18.53, taking max height as 1.7m
        Transform backRay = GameObject.Find("BackRay").transform, frontRay = GameObject.Find("FrontRay").transform;
        rigSword.isKinematic = false;
        sword.SetParent(null);
        #region Raycast Forces
        RaycastHit backHit, frontHit;
        if (Physics.Raycast(backRay.position, Vector3.down, out backHit)) {
            float dis = Vector3.Distance(backRay.position, backHit.point);
            float forceMult = (-10.9f * dis) + 18.53f;
            Debug.Log("Back distance: " + dis);
            rigSword.AddForceAtPosition(Vector3.up * forceMult, backHit.point);
        }
        if (Physics.Raycast(frontRay.position, Vector3.down, out frontHit)) {
            float dis = Vector3.Distance(frontRay.position, frontHit.point);
            float forceMult = (-10.9f * dis) + 18.53f;
            Debug.Log("Back distance: " + dis);
            rigSword.AddForceAtPosition(Vector3.up * forceMult, frontHit.point);
        }
        #endregion
        placeForSword = GameObject.Find("SwordPlace").transform;
        rigSword.MovePosition(Vector3.Slerp(sword.position, placeForSword.position, Time.deltaTime * magneticSwordSpeed));
        sword.LookAt(transform, -transform.forward);
    }

    private void RestoreSwordPos () {
        var hand = GameObject.Find("RightHand").transform;
        anim = GetComponent<Animator>();
        oneTimeMagnetic = false;
        sword.position = Vector3.Slerp(sword.position, hand.position, Time.deltaTime * magneticSwordSpeed);
        rigSword.isKinematic = true;
        //Set parent to the right hand of the character
        sword.SetParent(hand);
    }

    private void Magnetic_Slashes () {
        sword.localRotation = attackRot;
        magneticCollider.position = Vector3.Lerp(magneticCollider.position, basicAttackPos[1].position, Time.deltaTime * magneticSwordSpeed * 4f);
        rigSword.MovePosition(Vector3.Lerp(sword.position, basicAttackPos[1].position, Time.deltaTime * magneticSwordSpeed * 4f));
    }

    private void MoveOnAttack () {
        if ((target == null || Vector3.Distance(target.position, transform.position) >= 1.5f) && !spellRef.type.Equals(SpellShooting.Type.Magnetism) && dashRef.dashTime > 0f) {
            rig.AddForce(transform.forward * 8f, ForceMode.VelocityChange);
        }
    }

    private IEnumerator ResetAttackIndex (float t, bool _final) {
        if (!_final && !movRef.moving) {
            yield return new WaitForSeconds(t);
        }
        attackIndex = 0;
    }

}
