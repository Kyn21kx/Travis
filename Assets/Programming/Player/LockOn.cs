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
    public CinemachineFreeLook LockCam;
    public CinemachineFreeLook MainCam;
    #endregion

    private void Start() {
        LockCam.gameObject.SetActive(false);
        player = transform;
        Cursor.lockState = CursorLockMode.Locked;
        target = null;
    }
    //Check if there is a collision in between the player and the enemy, if there is, do not turn around
    private void Update() {
        _Input();
        var _target = GetTarget();
        if (locking) {
            Quaternion rotation = Quaternion.LookRotation(_target);
            collisionAux.rotation = rotation;
            Ray ray = new Ray(new Vector3(collisionAux.position.x, collisionAux.position.y + 0.5f, collisionAux.position.z), transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Vector3.Distance(new Vector3(collisionAux.position.x, 0, collisionAux.position.z), new Vector3(_target.x, 0, _target.z)))) {

            }
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 15f * Time.deltaTime);
            onetime = false;
            LockCam.gameObject.SetActive(true);
            MainCam.gameObject.SetActive(false);
        }
        else {
            LockCam.gameObject.SetActive(false);
            MainCam.gameObject.SetActive(true);
            target = null;
            locking = false;
        }
        Adjustments();
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

    //Adjust the camera
    private void Adjustments () {
        if (!locking && !onetime) {
            MainCam.m_XAxis.Value = LockCam.m_XAxis.Value;
            onetime = true;
        }

    }
}
