using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDir : MonoBehaviour {

    #region Variables
    [SerializeField]
    Vector2 xyInput;
    #endregion

    private void Update() {
        xyInput = GetComponent<SmoothMovement>().input;
        Identify();
    }

    private void Identify () {
        //Debug.Log("Forward: " + transform.forward.normalized);
        //Debug.Log("xyInput: " + xyInput);
    }

}
