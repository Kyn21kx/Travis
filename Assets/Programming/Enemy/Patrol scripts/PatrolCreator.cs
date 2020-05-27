using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Behaviour))]

public class PatrolCreator : MonoBehaviour {

    #region Variables
    public GameObject patrolPoint;
    public bool placePatrolPoint;
    [HideInInspector]
    public bool changedOrder;
    Behaviour aiRef;
    public List<Transform> targets = new List<Transform>();
    #endregion
    private void Awake() {
        aiRef = GetComponent<Behaviour>();
        placePatrolPoint = false;
    }

    private void Start() {
        //Add the list here
    }

    private void Update() {
        if (placePatrolPoint) {
            placePatrolPoint = false;
            var instance = Instantiate(patrolPoint, transform.position, Quaternion.identity);
            instance.transform.SetParent(CreateParent());
            targets.Add(instance.transform);
            var nodeRef = instance.GetComponent<PatrolNode>();
            nodeRef.agent = aiRef;
            nodeRef.creator = this;
            changedOrder = true;
        }
        try {
            if (changedOrder) {
                foreach (var target in targets) {
                    target.GetComponent<PatrolNode>().ChangeOrder();
                }
                changedOrder = false;
            }
        }
        catch (System.Exception) {
            targets.Clear();
        }
           
    }

    private Transform CreateParent() {
        GameObject insParent;
        if (!GameObject.Find(aiRef.transform.name + "_Patrol_Positions")) {
            insParent = new GameObject();
            insParent.transform.name = aiRef.transform.name + "_Patrol_Positions";
            insParent.transform.SetParent(GameObject.Find("PatrolPointsMaster").transform);
        }
        else {
            insParent = GameObject.Find(aiRef.transform.name + "_Patrol_Positions");
        }
        return insParent.transform;
    }

}
