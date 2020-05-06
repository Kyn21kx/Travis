using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    #region Variables
    public float lookSmoothing;
    public Transform target;
    public float clampAngle;
    public float sensitivity;
    public float cameraSpeed;
    [SerializeField]
    private Vector2 cameraInput;
    [SerializeField]
    public Vector2 _rotation;
    public bool canControl;
    #endregion

    private void Start() {
        canControl = true;
        _rotation = new Vector2(transform.rotation.eulerAngles.y, transform.rotation.x);
    }

    private void Update() {
        _Input();
        if (canControl)
            RotateCamera();
    }

    private void _Input () {
        cameraInput = canControl ? new Vector2(Input.GetAxis("RightJoystickX"), Input.GetAxis("RightJoystickY")) : Vector2.zero;
    }
    

    private void RotateCamera () {
        _rotation.x += cameraInput.x * sensitivity * Time.deltaTime;
        _rotation.y += cameraInput.y * sensitivity * Time.deltaTime;
        //Issue is caused by clamp angle
        _rotation.y = Mathf.Clamp(_rotation.y, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(_rotation.y, _rotation.x, 0f);
        transform.rotation = rot;
    }

    private void LateUpdate() {
        FollowPlayer();
    }

    private void FollowPlayer () {
        float step = cameraSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

}
