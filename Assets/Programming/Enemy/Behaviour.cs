using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Programming;
using System;

public class Behaviour : MonoBehaviour {

    public enum MasterType {Melee, Ranged, Buffer, Hybrid, Boss};
    public enum SubType {V1, V2, V3};
    public enum States {Patrol, Combat};
    #region Enemy Selector
    [Header("Enemy type selector")]
    public MasterType masterType;
    [Header("Variants of the enemy type")]
    public SubType subType;
    #endregion
    #region Variables
    public bool detected;
    public float forceMult;
    public bool burn;
    public bool canMove;
    public TextMeshPro text;
    private Enemy_Environment environment;
    [HideInInspector]
    public Transform player;
    [Header("Posciciones a los que se va a mover el agente, en orden")]
    public Transform[] targets;
    private bool shot;
    private States state;
    public float speed;
    private float dmgOverTime, time;
    float playerRadius, playerAngleX, playerAngleZ;
    float auxSpeed;
    GeneralBehaviours generalBehaviours;
    public Rigidbody rig;
    UnitPathfinder unitPathfinder;
    CorvoPathFinder pathFindingSys;
    [SerializeField]
    private float obstacleDistanceDetection;
    [SerializeField]
    private Transform[] rayPivot;
    [SerializeField]
    private LayerMask radiusAvoidanceMask;
    public Animator anim;
    public bool hit;
    [SerializeField]
    private float movingTimer;
    public bool stopped;
    EnemyCombat enCombatRef;
    #endregion

    #region Stats
    [Header("Stats")]
    public float health = 1f;
    public GameObject[] spells;
    [Header("The highest, the fastest... (Recommended value: 2)")]
    [Header("The time it takes the enemy to face the player")]
    public float rotateSpeed = 1f;
    public bool collided;
    public float angle, radius, range;
    #endregion

    //Initialization
    private void Start() {
        enCombatRef = GetComponent<EnemyCombat>();
        unitPathfinder = GetComponent<UnitPathfinder>();
        generalBehaviours = new GeneralBehaviours();
        pathFindingSys = GetComponent<CorvoPathFinder>();
        rig = GetComponent<Rigidbody>();
        environment = GameObject.FindGameObjectWithTag("GlobalParameters").GetComponent<Enemy_Environment>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        stopped = true;
        detected = false;
        shot = false;
        rig.isKinematic = true;
        canMove = true;
        state = States.Patrol;
    }

    private void Update() {
        HealthBehaviour();
        Shoot();
        StealthBehaviour();
        detected = generalBehaviours.Detect(player, transform, radius, angle);
        StateControl();
    }

    private void StateControl() {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (!canMove) {
            //Stop the player
            unitPathfinder.stop();
            rig.velocity = Vector3.zero;
        }
        switch (state) {
            case States.Patrol:
                //agent.speed = auxSpeed;
                //agent.isStopped = false;
                if (targets.Length != 0) {
                    disToPos = Vector3.Distance(transform.position, targets[patrolIndex].position);
                    //Replace with rigidbody motion
                    //agent.SetDestination(targets[patrolIndex].position);
                    /*if (generalBehaviours.ReachedPos()) {
                        patrolIndex++;
                    }
                    if (patrolIndex >= targets.Length) {
                        patrolIndex = 0;
                    }*/
                }
                break;
            case States.Combat:
                RotateTowards(player, rotateSpeed);
                if (enCombatRef.attackProbability < 1f) {
                    if (distanceToPlayer <= radius / 2f && detected) {
                        //The agent is ready to attack
                        Stop();
                        enCombatRef.inCombatRange = true;
                    }
                    else {
                        enCombatRef.inCombatRange = false;
                        movingTimer += Time.deltaTime;
                        float rnd = UnityEngine.Random.Range(0.5f, 1.5f);
                        stopped = false;
                        if (movingTimer >= rnd)
                            PathFindingMovement(player);
                    }
                }
                break;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        Vector3 fov1 = Quaternion.AngleAxis(angle, transform.up) * transform.forward * radius;
        Vector3 fov2 = Quaternion.AngleAxis(-angle, transform.up) * transform.forward * radius;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fov1);
        Gizmos.DrawRay(transform.position, fov2);
        if (!detected) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * radius);
        }
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * radius);
        }
    }
    float time_down;
    public void Damage (float dmg, float t, float dot) {
        //Apply force in the direction of the attack
        if (t != 0) {
            time_down = 0f;
            time = t;
            dmgOverTime = dot / t;
            burn = true;
        }
        else {
            burn = false;
        }
        health -= dmg;
        player.GetComponent<CallOfBeyond>().wrath += (dmg * 0.1f);
        environment.detected = true;
    }
    public void HealthBehaviour () {
        text.text = health.ToString();
        if (health <= 0) {
            Destroy(gameObject);
        }
        if (burn) {
            //DamageOverTime();
        }
    }

    #region global aux variables
    [SerializeField]
    float cntr = 0f;
    int patrolIndex = 0;
    float disToPos;
    int numberOfAttacks = 0;
    [SerializeField]
    float positionOffset;
    #endregion
    private void StealthBehaviour () {
        if (!environment.detected && detected) {
            environment.detected = true;
        }
        if (environment.detected) {
            state = States.Combat;
        }
    }

    #region MeleeCombat
    private void OnTriggerEnter(Collider other) {
        var combatController = player.GetComponent<Combat>();
        //Detects if the enemy is being hit by the player's sword
        if (other.transform.CompareTag("Sword")) {
            this.Damage(combatController.swordDamage, 0f, 0f);
            Debug.Log(other.transform.forward);
            hit = true;
            //Apply velocity change;
            MoveOnHit(other.transform);
        }
    }
    #endregion

    private void MoveOnHit (Transform damagingObj) {
        //Maybe set a time limit just like the dash
        rig.AddForce(damagingObj.forward * forceMult, ForceMode.Impulse);
    }

    bool followingPlayer = false;

    public void PathFindingMovement (Transform target) {
        //Destination - source
        //Move this to variable and function UpdateAnimatorParameters()
        if (canMove) {
            if (!stopped) {
                anim.SetBool("Walking", true);
                followingPlayer = CollisionCheck(target);
                if (followingPlayer) {
                    unitPathfinder.enabled = false;
                    //Replace this method to control speed
                    transform.position = Vector3.MoveTowards(transform.position, target.position, 0.1f);
                }
                else {
                    unitPathfinder.enabled = true;
                    //Extract the path
                    unitPathfinder.goTo(target.position);
                }
            }
        }
    }

    private void RotateTowards (Transform target, float speed) {
        Vector3 _target = target.position - transform.position;
        var rotation = Quaternion.LookRotation(_target);
        rotation.x = 0f;
        rotation.z = 0f;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }

    private bool CollisionCheck (Transform _target) {
        bool allHit = true;
        //Setup pivot to calculate the raycast
        for (int i = 0; i < rayPivot.Length; i++) {
            rayPivot[i].LookAt(_target);
            RaycastHit obstacleHit;
            if (Physics.Raycast(rayPivot[i].position, rayPivot[i].forward, out obstacleHit)) {
                if (obstacleHit.transform.CompareTag(_target.tag)) {
                    continue;
                }
                else {
                    if (Vector3.Distance(rayPivot[i].position, obstacleHit.point) <= obstacleDistanceDetection && obstacleHit.transform != transform) {
                        allHit = false;
                        break;
                    }
                }
            }
        }
        return allHit;
    }

    private void Shoot () {

    }

    public void Stop () {
        anim.SetBool("Walking", false);
        movingTimer = 0f;
        stopped = true;
    }

    //Function for when the enum changes

}
