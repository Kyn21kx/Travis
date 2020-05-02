using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using XInputDotNetPure;

public class SmoothMovement : MonoBehaviour {

    #region General variables
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
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
    float maxDis = float.MaxValue;
    public Transform closestFloor = null;
    public bool canMove;
    public LockOn lockOn;
    private CinemachineFreeLook mainCam;
    #endregion

    #region Animation variables
    private Animator anim;
    #endregion
    /* TODO:
     * Decrease the movement speed when in the air
     * When in the air, disable movement if parrying
     */
    private void Start() {
        canMove = true;
        auxRunningSpeed = runSpeed;
        anim = GetComponentInChildren<Animator>();
        lockOn = GetComponent<LockOn>();
        closestFloor = transform;
        auxSpeed = walkSpeed;
    }

    private void Update() {
        //Debug.Log(Input.GetAxis("LT"));
        if (canMove) {
            Jump();
            MovePlayer();
        }
        Identify();
    }

    private void MovePlayer () {
        //Input management
        //if controller input, then change
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 inputDir = input.normalized;
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
            var rig = GetComponent<Rigidbody>();
            if (!lockOn.locking) {
                float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                rotation = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref smoothVel, turnTime);
                transform.eulerAngles = rotation;
                //Change walk speed for speed and set the variable depending on the input
                rig.MovePosition(transform.position + transform.forward * walkSpeed * Time.deltaTime);
                anim.SetBool("Walk", true);
            }
            else {
                float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                rotation = Vector3.up * Mathf.SmoothDampAngle(movPivot.transform.eulerAngles.y, target, ref smoothVel, turnTime);
                movPivot.transform.eulerAngles = rotation;
                //Change walk speed for speed and set the variable depending on the input
                rig.MovePosition(transform.position + movPivot.transform.forward * walkSpeed * Time.deltaTime);
                anim.SetBool("Walk", true);
            }
        }
        else {
            anim.SetBool("Walk", false);
        }
        
    }

    private void Jump () {
        var rb = GetComponent<Rigidbody>();
        mainCam = lockOn.mainCam;
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
