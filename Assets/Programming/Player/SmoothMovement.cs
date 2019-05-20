using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour {

    #region General variables
    public float stealthSpeed = 4f;
    public float stealthRun = 5.5f;
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public Vector2 input;
    float smoothVel = 5f;
    float turnTime = 0.04f;
    public bool grounded;
    public float jumpForce = 20f;
    public GameObject movPivot;
    float distance;
    Vector3 rotation;
    float maxDis = float.MaxValue;
    public Transform closestFloor = null;
    #endregion

    #region Animation variables
    Animator anim;
    #endregion
    /* TODO:
     * Decrease the movement speed when in the air
     * When in the air, disable movement if parrying
     */
    private void Start() {
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate() {
        //Debug.Log(Input.GetAxis("LT"));
        MovePlayer(gameObject);
        Jump();
        Identify();
    }

    private void MovePlayer (GameObject moveTarget) {
        //Input management
        //if controller input, then change
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 inputDir = input.normalized;
        if (inputDir != Vector2.zero) {
            float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            rotation = Vector3.up * Mathf.SmoothDampAngle(moveTarget.transform.eulerAngles.y, target, ref smoothVel, turnTime);
            moveTarget.transform.eulerAngles = rotation;
            //Change walk speed for speed and set the variable depending on the input
            transform.Translate(movPivot.transform.forward * walkSpeed * Time.fixedDeltaTime, Space.World);
            anim.SetBool("Walk", true);
        }
        else {
            anim.SetBool("Walk", false);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || (Input.GetButtonDown("L3"))) {
            anim.SetBool("Run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || (Input.GetButtonUp("L3"))) {
            anim.SetBool("Run", false);
        }
    }

    private void Jump () {
        var rb = GetComponent<Rigidbody>();
        if ((Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Space)) && grounded) {
            //anim.SetBool("Jump", true);
            rb.velocity += Vector3.up * jumpForce;
        }
        if (distance >= 3) {
            //anim.SetBool("Jump", false);
            rb.velocity += Vector3.down;
        }
    }

    private void Identify() {
        distance = transform.position.y - closestFloor.position.y;
    }

}
