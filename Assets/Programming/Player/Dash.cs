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
    private float t;
    #endregion

    private void Start() {
        auxDis = speed;
        t = 0f;
        locker = GetComponent<LockOn>();
    }

    private void Update() {
        _Dash();
    }

    private void _Dash () {
        if (Input.GetButtonDown("B") && GetComponent<StaminaManager>().staminaAmount >= staminaCost) {
            t += Time.deltaTime;
            transform.Translate(GetComponent<SmoothMovement>().movPivot.transform.forward * speed * Time.deltaTime, Space.World);
        }
    }
    
}
