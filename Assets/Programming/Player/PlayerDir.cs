using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDir : MonoBehaviour {

    #region Variables
    [SerializeField]
    Vector2 xyInput;
    [SerializeField]
    List<Vector3> data;
    #endregion

    private void Update() {
        xyInput = GetComponent<SmoothMovement>().input;
        Identify();
    }

    private void Identify() {
        if (xyInput != Vector2.zero && Input.GetKeyDown(KeyCode.M)) {
            data.Add(new Vector3(transform.forward.x, transform.eulerAngles.y, transform.forward.z));
        }
    }

}
