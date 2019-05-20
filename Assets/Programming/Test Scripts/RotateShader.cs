using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShader : MonoBehaviour {

    #region Variables

    #endregion

    private void Update() {
        transform.Rotate(axis: Vector3.right, 2f);
        transform.Rotate(axis: Vector3.up, 2f);
    }

}
