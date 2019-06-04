using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    #region Varibles
    [SerializeField]
    private Transform center;
    private float auxDis;
    public float time;
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
            //Vector 3 Lerp
            RaycastHit hit;
            Vector3 target = transform.position + transform.forward * speed;
            //Debug.DrawRay(target, -Vector3.up, Color.green);
            if (Physics.Linecast(transform.position, target, out hit)) {
                target = transform.position + transform.forward * (hit.distance - 1f);
            }
            transform.position = target;
        }
    }
    
}
