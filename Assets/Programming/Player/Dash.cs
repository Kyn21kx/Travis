using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Dash : MonoBehaviour {
    /*
     * TODO:
     * Stamina cost
     * Cooldown
     */
    #region Varibles
    [SerializeField]
    private Transform center;
    private float auxDis;
    private LockOn locker;
    public float time;
    #endregion
    #region statsVars
    public float speed = 50f;
    public float staminaCost = 10f;
    #endregion

    private void Start() {
        auxDis = speed;
        locker = GetComponent<LockOn>();
    }

    private void Update() {
        _Dash();
    }

    private void _Dash () {
        if (Input.GetButtonDown("B") && GetComponent<StaminaManager>().staminaAmount >= staminaCost) {
            //Vector 3 Lerp
            RaycastHit hit;
            Vector3 target;
            if (!locker.locking) {
                target = transform.position + transform.forward * speed;
            }
            else {
                Transform piv = GetComponent<SmoothMovement>().movPivot.transform;
                target = transform.position + piv.forward * speed;
            }
            //Debug.DrawRay(target, -Vector3.up, Color.green);
            if (Physics.Linecast(transform.position, target, out hit)) {
                target = transform.position + transform.forward * (hit.distance - 1f);
            }
            transform.position = target;
            
        }
    }
    
}
