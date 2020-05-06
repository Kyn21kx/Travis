using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using XInputDotNetPure;

public class SmoothMovement : MonoBehaviour {

    #region General variables
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public bool moving;
    [HideInInspector]
    public float auxSpeed;
    [HideInInspector]
    public float auxRunningSpeed;
    public Vector2 input;
    float smoothVel = 5f;
    float turnTime = 0.04f;
    public bool grounded;
    public float maxJumpDistance;
    public float jumpForce = 20f;
    public GameObject movPivot;
    float distance;
    Vector3 rotation;
    private Transform movingTarget;
    float maxDis = float.MaxValue;
    public Transform closestFloor = null;
    public bool canMove;
    public LockOn lockOn;
    Rigidbody rig;
    #endregion

    #region Animation variables
    private Animator anim;
    #endregion
    /* TODO:
     * Decrease the movement speed when in the air
     * When in the air, disable movement if parrying
     */
    private void Start() {
        rig = GetComponent<Rigidbody>();
        moving = false;
        movingTarget = transform;
        canMove = true;
        auxRunningSpeed = runSpeed;
        anim = GetComponentInChildren<Animator>();
        lockOn = GetComponent<LockOn>();
        closestFloor = transform;
        auxSpeed = walkSpeed;
    }

    private void Update() {
        //Debug.Log(Input.GetAxis("LT"));
        _Input();
        if (canMove) {
            Jump();
        }
        MovePlayer();
        Identify();
    }

    private void _Input () {
        Vector2 inputDir = input.normalized;
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftShift) || (Input.GetButtonDown("L3"))) {
            GamePad.SetVibration(PlayerIndex.One, 0.2f, 0.2f);
            anim.SetBool("Run", true);
            walkSpeed = runSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || (Input.GetButtonUp("L3"))) {
            anim.SetBool("Run", false);
            walkSpeed = auxSpeed;
        }
        if (inputDir != Vector2.zero) {
            moving = true;
            float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            if (!lockOn.locking) {
                movingTarget = transform;
                rotation = Vector3.up * Mathf.SmoothDampAngle(movingTarget.eulerAngles.y, target, ref smoothVel, turnTime);
                movingTarget.eulerAngles = !GetComponent<Combat>().attacking ? rotation : movingTarget.eulerAngles;
            }
            else {
                movingTarget = movPivot.transform;
                rotation = Vector3.up * Mathf.SmoothDampAngle(movingTarget.eulerAngles.y, target, ref smoothVel, turnTime);
                movingTarget.eulerAngles = rotation;
            }
        }
        else {
            moving = false;
        }

    }

    private void MovePlayer () {
        if (moving) {
            //Change walk speed for speed and set the variable depending on the input
            if (canMove) {
                rig.MovePosition(transform.position + movingTarget.forward * walkSpeed * Time.deltaTime);
                anim.SetBool("Walk", true);
            }
        }
        else {
            anim.SetBool("Walk", false);
        }
    }

    private void Jump () {
        var rb = GetComponent<Rigidbody>();
        if ((Input.GetButtonDown("A") || Input.GetKeyDown(KeyCode.Space)) && grounded) {
            //anim.SetBool("Jump", true);
            rb.velocity += Vector3.up * jumpForce;
        }
        if (distance >= maxJumpDistance) {
            StartCoroutine(FrameJump());
            //anim.SetBool("Jump", false);
        }
    }

    IEnumerator FrameJump () {
        //Add camera behaviour
        var rb = GetComponent<Rigidbody>();
        rb.velocity += Vector3.down * 0f;
        yield return new WaitForFixedUpdate();
        rb.velocity += Vector3.down * (jumpForce / 15f);
    }

    private void Identify() {
        distance = transform.position.y - closestFloor.position.y;
    }

}
