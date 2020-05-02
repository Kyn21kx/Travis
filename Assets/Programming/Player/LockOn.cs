using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockOn : MonoBehaviour {

    #region Variables
    [SerializeField]
    Transform target, player, collisionAux;
    [SerializeField]
    float distanceToTarget;
    public bool locking;
    private bool onetime = false;
    private bool isCollided = true;
    Vector2 viewportTarget, viewportPlayer;
    public CinemachineFreeLook mainCam;
    public CinemachineFreeLook lockCam;
    #endregion

    private void Awake() {
        player = transform;
        Cursor.lockState = CursorLockMode.Locked;
        target = null;
        lockCam.gameObject.SetActive(false);
    }
    //Check if there is a collision in between the player and the enemy, if there is, do not turn around
    private void Update() {
        _Input();
        var _target = GetTarget();
        if (locking) {
            Quaternion rotation = Quaternion.LookRotation(_target);
            collisionAux.rotation = rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 15f * Time.deltaTime);
            onetime = false;
            LockCamera();
            Adjustments();
        }
        else {
            UnlockCamera();
            target = null;
        }
    }

    private void _Input () {
        if (Input.GetButtonDown("RS") && !locking) {
            locking = true;
        }
        else if (Input.GetButtonDown("RS") && locking) {
            locking = false;
        }
    }

    //Find the nearest target
    //Add condition if there are no enemies
    //Add a distance limit
    private Vector3 GetTarget () {
        Vector2 joyInput = new Vector2(Input.GetAxis("RightJoystickX"), Input.GetAxis("RightJoystickY"));
        if (target == null) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies != null) {
                float minDis = float.MaxValue;
                float dis;
                //Changing target to the right
                if (joyInput. x > 0) {
                    //Get the second nearest enemy based on +x
                    foreach (GameObject enemy in enemies) {
                        dis = Vector3.Distance(player.position, enemy.transform.position);
                        if (dis < minDis) {
                            minDis = dis;
                            target = enemy.transform;
                        }
                        else if (dis == minDis) {
                            target = enemy.transform;
                        }
                    }
                }
                //Chanigng target to the left
                else if (joyInput.x < 0) {

                }
                //Changing target forwards
                else if (joyInput.y > 0) {

                }
                //Changing target backwards
                else if (joyInput.y < 0) {

                }
                //Default nearest target
                else {
                    foreach (GameObject enemy in enemies) {
                        dis = Vector3.Distance(player.position, enemy.transform.position);
                        if (dis < minDis) {
                            minDis = dis;
                            target = enemy.transform;
                        }
                        else if (dis == minDis) {
                            target = enemy.transform;
                        }
                    }
                }
            }
        }
        return target.position - player.position;
    }

    private void Adjustments () {
        mainCam.m_XAxis.Value = lockCam.m_XAxis.Value;
        mainCam.m_YAxis.Value = lockCam.m_YAxis.Value;
    }

    private void LockCamera () {
        mainCam.gameObject.SetActive(false);
        lockCam.gameObject.SetActive(true);
    }

    private void UnlockCamera () {
        mainCam.gameObject.SetActive(true);
        lockCam.gameObject.SetActive(false);
    }
}
