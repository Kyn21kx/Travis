using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Environment : MonoBehaviour
{

    #region
    public bool detected;
    public NavMeshSurface surface;
    private GameObject[] enemies;
    public bool physicsUpdate;
    #endregion

    private void Start() {
        InvokeRepeating("UpdateNavMesh", 1f, 0.5f);
    }

    private void Update() {
        
    }
}
