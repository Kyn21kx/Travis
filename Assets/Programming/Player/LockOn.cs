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
        if (Input.GetAxis("LT") > 0) {
            locking = true;
            var target = GetTarget();
            Quaternion rotation = Quaternion.LookRotation(target);
            collisionAux.rotation = rotation;
            Ray ray = new Ray(new Vector3(collisionAux.position.x, collisionAux.position.y + 0.5f, collisionAux.position.z), transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Vector3.Distance(new Vector3(collisionAux.position.x, 0, collisionAux.position.z), new Vector3(target.x, 0, target.z)))) {

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

    //Find the nearest target
    //Add condition if there are no enemies
    //Add a distance limit
    private Vector3 GetTarget () {
        if (target == null) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float minDis = float.MaxValue;
            foreach (GameObject enemy in enemies) {
                float dis = Vector3.Distance(player.position, enemy.transform.position);
                if (dis < minDis) {
                    minDis = dis;
                    target = enemy.transform;
                }
                else if (dis == minDis) {
                    target = enemy.transform;
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
