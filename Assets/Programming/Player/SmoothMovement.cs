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
    public bool canMove;
    public LockOn lockOn;
    #endregion

    #region Animation variables
    Animator anim;
    #endregion
    /* TODO:
     * Decrease the movement speed when in the air
     * When in the air, disable movement if parrying
     */
    private void Start() {
        canMove = true;
        anim = GetComponentInChildren<Animator>();
        lockOn = GetComponent<LockOn>();
        closestFloor = transform;
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
        if (inputDir != Vector2.zero) {
            if (!lockOn.locking) {
                float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                rotation = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, target, ref smoothVel, turnTime);
                transform.eulerAngles = rotation;
                //Change walk speed for speed and set the variable depending on the input
                transform.Translate(transform.forward * walkSpeed * Time.deltaTime, Space.World);
                anim.SetBool("Walk", true);
            }
            else {
                float target = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                rotation = Vector3.up * Mathf.SmoothDampAngle(movPivot.transform.eulerAngles.y, target, ref smoothVel, turnTime);
                movPivot.transform.eulerAngles = rotation;
                //Change walk speed for speed and set the variable depending on the input
                transform.Translate(movPivot.transform.forward * walkSpeed * Time.deltaTime, Space.World);
                anim.SetBool("Walk", true);
            }
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
        if (distance >= 2.4f) {
            StartCoroutine(FrameJump());
            //anim.SetBool("Jump", false);
        }
    }

    IEnumerator FrameJump () {
        var rb = GetComponent<Rigidbody>();
        rb.velocity += Vector3.down * 0.000000001f;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        rb.velocity += Vector3.down * (jumpForce / 18);
    }

    private void Identify() {
        distance = transform.position.y - closestFloor.position.y;
    }

}
