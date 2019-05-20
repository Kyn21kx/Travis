using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    #region Varibles
    [SerializeField]
    private Transform center;
    private float auxDis;
    #endregion
    #region statsVars
    public float speed = 50f;
    public float staminaCost = 10f;
    #endregion

    private void Start() {
        auxDis = speed;
    }

    private void FixedUpdate() {
        _Dash();
    }

    private void _Dash () {
        if (Input.GetButtonDown("B") && GetComponent<StaminaManager>().staminaAmount >= staminaCost) {
            transform.DOMove(transform.forward, 0.2f);
        }
    }
}
