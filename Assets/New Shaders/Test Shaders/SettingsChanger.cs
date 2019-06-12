using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsChanger : MonoBehaviour {

    #region
    public Material shader;
    public float ex = 0f;
    #endregion

    private void Update() {
        shader.SetFloat("_Intensity", ex);
        
    }
}
