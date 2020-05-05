using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Programming;

public class EnemyCombat : MonoBehaviour {
    //To do: Add combos and when you are in the combo, the animations just play, the probability resets to 0, and you can't move
    #region Variables
    [Header("Set Basic Dmg")]
    public float basicMeleeDmg = 1f;
    public float basicRangedDmg = 1f;
    [Header("Set Mid Dmg")]
    public float midMeleeDmg = 1f;
    public float midRangedDmg = 1f;
    [Header("Set Heavy Dmg")]
    public float heavyMeleeDmg = 1f;
    public float heavyRangedDmg = 1f;
    public float activeDmg;
    [Header ("Probability for attacking MAX 1")]
    public float attackProbability;
    Behaviour behaviourRef;
    public bool inCombatRange;
    public bool inAttackRange;
    public float aggressiveness;
    GeneralBehaviours generalBehaviours;
    public Transform sword;
    private CapsuleCollider swordColl;
    public bool attacking;
    #endregion

    private void Start() {
        swordColl = sword.GetComponent<CapsuleCollider>();
        generalBehaviours = new GeneralBehaviours();
        behaviourRef = GetComponent<Behaviour>();

        inCombatRange = false;
        swordColl.enabled = false;
    }

    private void Update() {
        UpdateProbability();
        inAttackRange = generalBehaviours.ReachedPos(transform, behaviourRef.player.position, behaviourRef.range);
        ActivateAttack();
    }
    
    private void UpdateProbability () {
        if (inCombatRange && attackProbability < 1f) {
            attackProbability += (Time.deltaTime * aggressiveness) / 10f;
        }
        attackProbability = Mathf.Clamp01(attackProbability);
    }

    private void ActivateAttack () {
        if (attackProbability >= 1f) {
            if (!inAttackRange) {
                behaviourRef.stopped = false;
                behaviourRef.PathFindingMovement(behaviourRef.player);
            }
            else if (!attacking) {
                behaviourRef.Stop();
                attacking = true;
                activeDmg = basicMeleeDmg;
                behaviourRef.canMove = false;
                behaviourRef.anim.SetTrigger("Attack");
            }
        }
    }

    private void EnSlash () {
        swordColl.enabled = true;
    }

    private void EnFinishedStrike () {
        swordColl.enabled = false;
    }

    private void EnFinishedAnim () {
        behaviourRef.canMove = true;
        attacking = false;
        attackProbability = 0f;
    }

}
