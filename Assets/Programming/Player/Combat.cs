using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    #region Variables
    [SerializeField]
    private Transform sword;
    private Animator anim;
    private SmoothMovement movRef;
    [SerializeField]
    private bool attacking;
    public bool canMeleeAttack;
    private Rigidbody rig;
    public float attachMult;
    [SerializeField]
    float dis;
    #endregion

    private void Start() {
        canMeleeAttack = true;
        rig = GetComponent<Rigidbody>();
        attacking = false;
        anim = GetComponent<Animator>();
        sword = GameObject.FindGameObjectWithTag("Sword").transform;
        movRef = GetComponent<SmoothMovement>();
    }

    private void Update() {
        MeleeBehaviours();
        if (!attacking)
            _Input();
    }
    //Replace combo manager's input with this one, and also kinda rewrite that whole script, please
    private void _Input () {
        if (Input.GetButtonDown("X") && canMeleeAttack) {
            anim.SetTrigger("Attack");
            movRef.canMove = false;
            attacking = true;
            //Change for move towards
            rig.AddForce(transform.forward.normalized * attachMult, ForceMode.Force);
        }
    }

    private void MeleeBehaviours () {
        Debug.DrawRay(sword.position, sword.forward * dis);
    }

    private void SwordSlash() {
        //Raycast sword
        RaycastHit swordHit;
        //if(Physics.Raycast(sword.position, sword.forward)
    }

    private void Finished_Anim () {
        rig.velocity *= 0f;
        attacking = false;
        movRef.canMove = true;
    }

}
