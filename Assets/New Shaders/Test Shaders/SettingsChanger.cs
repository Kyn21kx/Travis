using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsChanger : MonoBehaviour {

    #region
    public Material shader;
    public float ex = 0f;
    #endregion

    private void Update() {
        shader.SetFloat("_Opacity", ex);
        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 180, 0, 0), 5f * Time.deltaTime);
    }
}
