using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Programming;
using System;

public class Behaviour : MonoBehaviour {

    /*
     * TODO:
     * Health Manager
     * Levels
     * Shooting
     * Avoiding
     * Go for the back of the player if there is another enemy in the front part of you
     */
    #region Types of enemies described
    /*
     * Enemy types {
     *     
*Melee
  *Lígeros
  *Pesados
  *Player-like enemy

*Híbridos

*Buffer
   *Healer
   *Shielder (+ Shield)
   *Damage booster

*Bosses
   *
*Distancia
   *Básicos
   *Casters (hechizos pesados)
*Minions (disposable enemies)
     * }
     */
    #endregion
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
    Transform player;
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
    public bool hit;
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
        detected = false;
        unitPathfinder = GetComponent<UnitPathfinder>();
        generalBehaviours = new GeneralBehaviours();
        pathFindingSys = GetComponent<CorvoPathFinder>();
        shot = false;
        rig = GetComponent<Rigidbody>();
        rig.isKinematic = false;
        canMove = true;
        state = States.Patrol;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        environment = GameObject.FindGameObjectWithTag("GlobalParameters").GetComponent<Enemy_Environment>();
        //auxSpeed = agent.speed;
    }

    private void Update() {
        //agent.speed = speed;
        HealthBehaviour();
        Shoot();
        StealthBehaviour();
        detected = generalBehaviours.Detect(player, transform, radius, angle);
        StateControl();
        if (hit) {
            MoveOnHit();
        }
    }

    private void StateControl() {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
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
                if (canMove) {
                    RotateTowards(player, rotateSpeed);
                    if (distanceToPlayer <= range) {
                        //agent.isStopped = true;
                    }
                    else {
                        //agent.isStopped = false;
                        Vector3 pos = player.position;
                        PathFindingMovement();
                        //agent.SetDestination(pos);
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
        }
    }
    #endregion

    private void MoveOnHit () {
        canMove = false;
        Vector3 newPos = (rig.position + player.forward).normalized * forceMult;
        rig.MovePosition(Vector3.Lerp(rig.position, newPos, Time.deltaTime));
    }

    bool followingPlayer = false;

    private void PathFindingMovement () {
        //Destination - source
        Vector3 dir = (player.position - transform.position).normalized;
        followingPlayer = CollisionCheck();
        if (followingPlayer) {
            unitPathfinder.enabled = false;
            rig.AddForce(dir * speed, ForceMode.Impulse);
        }
        else {
            unitPathfinder.enabled = true;
            //Extract the path
            unitPathfinder.goTo(player.position);
        }
    }

    private void RotateTowards (Transform target, float speed) {
        Vector3 _target = target.position - transform.position;
        var rotation = Quaternion.LookRotation(_target);
        rotation.x = 0f;
        rotation.z = 0f;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }

    private bool CollisionCheck () {
        bool allHit = true;
        //Setup pivot to calculate the raycast
        for (int i = 0; i < rayPivot.Length; i++) {
            rayPivot[i].LookAt(player);
            RaycastHit obstacleHit;
            if (Physics.Raycast(rayPivot[i].position, rayPivot[i].forward, out obstacleHit)) {
                if (obstacleHit.transform.CompareTag("Player")) {
                    continue;
                }
                else {
                    if (Vector3.Distance(rayPivot[i].position, obstacleHit.point) <= obstacleDistanceDetection) {
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
    //Function for when the enum changes

}
