using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockOn : MonoBehaviour {

    #region Variables
    [SerializeField]
    Transform target, player;
    [SerializeField]
    float distanceToTarget;
    public bool locking;
    Vector2 viewportTarget, viewportPlayer;
    public CinemachineFreeLook LockCam;
    public CinemachineFreeLook MainCam;
    #endregion

    private void Start() {
        LockCam.gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.Locked;
        target = null;
    }

    private void Update() {
        if (Input.GetAxis("LT") > 0) {
            locking = true;
            Quaternion rotation = Quaternion.LookRotation(GetTarget());
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 15f * Time.fixedDeltaTime);
            LockCam.gameObject.SetActive(true);
            MainCam.gameObject.SetActive(false);
        }
        else {
            LockCam.gameObject.SetActive(false);
            MainCam.gameObject.SetActive(true);
            target = null;
            locking = false;
        }
    }

    //Find the nearest target
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
            }
        }
        return target.position - player.position;
    }

    //Adjust the camera
    
}
