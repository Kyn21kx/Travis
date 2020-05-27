using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class PatrolNode : MonoBehaviour {

    #region Variables
    public Behaviour agent;
    [HideInInspector]
    public PatrolCreator creator;
    public int index;
    #endregion

    private void Start() {

    }

    private void OnDestroy() {
        creator.targets.Remove(transform);
        creator.changedOrder = true;
    }

    public void ChangeOrder() {
        index = creator.targets.FindIndex(t => t.transform == transform);
        transform.name = agent.transform.name + "_Patrol_Pos_#" + index;
    }

}
