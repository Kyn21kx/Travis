using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    #region Variables
    public Transform sword;
    public float swordDamage;
    private float auxDamage;
    public float boostDamage;
    private Animator anim;
    [SerializeField]
    private float turningDistance;
    private SmoothMovement movRef;
    [SerializeField]
    private bool attacking;
    public bool canMeleeAttack;
    private Rigidbody rig;
    public float attachMult;
    private Transform target;
    Vector3 movingPos;
    CapsuleCollider swordColl;
    #endregion

    private void Start() {
        canMeleeAttack = true;
        rig = GetComponent<Rigidbody>();
        auxDamage = swordDamage;
        attacking = false;
        anim = GetComponent<Animator>();
        sword = GameObject.FindGameObjectWithTag("Sword").transform;
        swordColl = sword.GetComponent<CapsuleCollider>();
        movRef = GetComponent<SmoothMovement>();
        swordColl.enabled = false;
    }

    private void Update() {
        MeleeBehaviours();
        if (!attacking)
            _Input();
    }
    Vector3 pos1;
    //Replace combo manager's input with this one, and also kinda rewrite that whole script, please
    private void _Input () {
        if (Input.GetButtonDown("X") && canMeleeAttack) {
            anim.SetTrigger("Attack");
            movRef.canMove = false;
            attacking = true;
            //Update the transform.forward for another value if you are rotating
            pos1 = transform.position;
            movingPos = rig.position + transform.forward.normalized;
        }
    }

    private void GetClosestEnemy () {
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
            if (target != null) {
                Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
            if (target == null || Vector3.Distance(target.position, transform.position) <= 1) {
                rig.position = Vector3.Lerp(rig.position, movingPos, Time.deltaTime * 4f);  
            }
            else if (Vector3.Distance(target.position, transform.position) <= 0.5f) {
                //Go backwards
            }
        }
        else {
            GetClosestEnemy();
        }
    }


    private void SwordSlash() {
        swordColl.enabled = true;
    }

    private void FinishedStrike () {
        swordColl.enabled = false;
    }
    Vector3 pos2;
    private void Finished_Anim () {
        pos2 = transform.position;
        Debug.Log(Vector3.Distance(pos1, pos2));
        rig.velocity *= 0f;
        attacking = false;
        target = null;
        movRef.canMove = true;
    }

}
