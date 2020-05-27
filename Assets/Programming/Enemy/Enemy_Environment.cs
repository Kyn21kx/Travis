using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class Enemy_Environment : MonoBehaviour
{

    #region
    public bool detected;
    public List<Behaviour>  prioritizedEnemies;
    public bool physicsUpdate;
    public bool inCombat;
    public bool setup;
    public bool oneTimeArr;
    public List<Behaviour> enemiesOnCombat;
    public int enemyCntr;
    #endregion

    private void Start() {
        oneTimeArr = false;
        enemyCntr = 0;
    }
    private void Update() {
        PriorityPicker();
    }
    private void PriorityPicker() {
        //Issue lies within the enemiesOnCombat.Count -1 line
        if (enemiesOnCombat.Count > 1 && enemyCntr < enemiesOnCombat.Count - 1) {
            var enCombatRef = enemiesOnCombat[enemyCntr].GetComponent<EnemyCombat>();
            var nextCombatRef = enemiesOnCombat[enemyCntr + 1].GetComponent<EnemyCombat>();
            CheckAgain:
            if (enCombatRef.priority == nextCombatRef.priority || (enemyCntr != 0 && enCombatRef.priority == enemiesOnCombat[enemyCntr - 1].GetComponent<EnemyCombat>().priority)) {
                enCombatRef.priority++;
                goto CheckAgain;
            }
            //Rearrange Array
            prioritizedEnemies[enCombatRef.priority] = enemiesOnCombat[enemyCntr];
            prioritizedEnemies[nextCombatRef.priority] = enemiesOnCombat[enemyCntr + 1];
            enemyCntr++;
        }
    }
}
