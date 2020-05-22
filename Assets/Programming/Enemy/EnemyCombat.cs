using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Programming;
using System.Linq;

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
    public int priority;
    GeneralBehaviours generalBehaviours;
    public Transform sword;
    private Animator anim;
    public CapsuleCollider swordColl;
    private Enemy_Environment environmentRef;
    public Behaviour prevEnemy;
    public bool attacking;
    public bool prioritySetup;
    #endregion
    //To do: when the enemies detect you, they have a priority system in place, and if they are the same priority, it is assigned randomly
    private void Start() {
        swordColl = sword.GetComponent<CapsuleCollider>();
        generalBehaviours = new GeneralBehaviours();
        behaviourRef = GetComponent<Behaviour>();
        anim = GetComponent<Animator>();
        environmentRef = behaviourRef.environment;
        inCombatRange = false;
        swordColl.enabled = false;
    }

    private void Update() {
        UpdateProbability();
        inAttackRange = generalBehaviours.ReachedPos(transform.position, behaviourRef.player.position, behaviourRef.range);
        ActivateAttack();
    }
    
    public void Dueling () {
        //Set this up only when the function has finished rearranging
        if (priority != 0)
            prevEnemy = environmentRef.prioritizedEnemies[priority - 1];
        inCombatRange = true;
        behaviourRef.stopped = false;
        behaviourRef.PathFindingMovement(behaviourRef.player, behaviourRef.radius);
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
                behaviourRef.PathFindingMovement(behaviourRef.player, behaviourRef.range / 2f);
            }
            else if (!attacking ) {
                if (prevEnemy != null) {
                    if (!prevEnemy.GetComponent<EnemyCombat>().attacking) {
                        behaviourRef.Stop();
                        attacking = true;
                        activeDmg = basicMeleeDmg;
                        behaviourRef.canMove = false;
                        behaviourRef.anim.SetTrigger("Attack");
                    }
                }
                else {
                    behaviourRef.Stop();
                    attacking = true;
                    activeDmg = basicMeleeDmg;
                    behaviourRef.canMove = false;
                    behaviourRef.anim.SetTrigger("Attack");
                }
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
