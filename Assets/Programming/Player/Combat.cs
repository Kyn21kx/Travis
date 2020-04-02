using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    #region Variables
    [SerializeField]
    private GameObject sword;
    private Animator anim;
    private SmoothMovement movRef;
    [SerializeField]
    private bool attacking;
    #endregion

    private void Start() {
        attacking = false;
        anim = GetComponent<Animator>();
        movRef = GetComponent<SmoothMovement>();
    }

    private void Update() {
        if (!attacking)
            _Input();
    }
    //Replace combo manager's input with this one, and also kinda rewrite that whole script, please
    private void _Input () {
        if (Input.GetButtonDown("X")) {
            anim.SetTrigger("Attack");
            movRef.canMove = false;
            attacking = true;
        }
    }

    private void SwordSlash () {
        Debug.Log("Attacked");
    }

    private void Finished_Anim () {
        attacking = false;
        movRef.canMove = true;
    }

}
