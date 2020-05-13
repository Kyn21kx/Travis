using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Environment : MonoBehaviour
{

    #region
    public bool detected;
    public NavMeshSurface surface;
    public List<GameObject> prioritizedEnemies = new List<GameObject>();
    public bool physicsUpdate;
    private bool setup;
    #endregion

    private void Start() {
        setup = false;
    }

    private void Update() {
        if (!setup)
            PriorityPicker();
    }
    //Loop this in update
    private void PriorityPicker() {
        //Set up a distance check
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        enemies.Remove(gameObject);
        for (int i = 0; i < enemies.Count; i++) {
            var behaviourRef = enemies[i].GetComponent<Behaviour>();
            var enCombatRef = enemies[i].GetComponent<EnemyCombat>();
            var nextEnCombatRef = enemies[i + 1].GetComponent<EnemyCombat>();
            enCombatRef.priority = (int)behaviourRef.masterType;
            if (enCombatRef.priority == nextEnCombatRef.priority) {
                enCombatRef.priority += new System.Random().Next(0, 1);
                i--;
            }
            prioritizedEnemies.Insert(enCombatRef.priority, enCombatRef.gameObject);
        }
        setup = true;
    }
}
