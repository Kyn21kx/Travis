using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsChanger : MonoBehaviour {

    #region
    public Material shader;
    float ex = 0f;
    #endregion

    private void Update() {
        ex += 1f * Time.deltaTime;
        shader.SetFloat("_Intensity", ex);
        
    }
}
