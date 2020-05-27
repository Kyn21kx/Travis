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
    [HideInInspector]
    public Enemy_Environment environment;
    [HideInInspector]
    public Transform player;
    [Header("Posciciones a los que se va a mover el agente, en orden")]
    public List<Transform> targets;
    private bool shot;
    public States state;
    public float speed;
    private float dmgOverTime, time;
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
    [SerializeField]
    float cntr = 0f;
    int patrolIndex;
    float disToPos;
    int numberOfAttacks = 0;
    [SerializeField]
    float positionOffset;
    float detectionP = 0f;
    #endregion

    #region Stats
    [Header("Stats")]
    public float health = 1f;
    public float shield;
    private float auxShield;
    public float shieldRecoveryCooldown;
    private float auxShieldRecoveryCooldown;
    public GameObject[] spells;
    [Header("The highest, the fastest... (Recommended value: 2)")]
    [Header("The time it takes the enemy to face the player")]
    public float rotateSpeed = 1f;
    public bool collided;
    public float angle, radius, range;
    #endregion

    //Initialization
    private void Start() {
        auxShield = shield;
        auxShieldRecoveryCooldown = shieldRecoveryCooldown;
        enCombatRef = GetComponent<EnemyCombat>();
        unitPathfinder = GetComponent<UnitPathfinder>();
        generalBehaviours = new GeneralBehaviours();
        pathFindingSys = GetComponent<CorvoPathFinder>();
        rig = GetComponent<Rigidbody>();
        environment = GameObject.FindGameObjectWithTag("GlobalParameters").GetComponent<Enemy_Environment>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        patrolIndex = 0;
        stopped = true;
        detected = false;
        shot = false;
        rig.isKinematic = false;
        canMove = true;
        state = States.Patrol;
    }

    private void Update() {
        HealthBehaviour();
        RecoverShield();
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
                anim.SetBool("Patrol", true);
                anim.SetBool("Combat", false);
                //agent.speed = auxSpeed;
                stopped = false;
                if (targets.Count != 0) {
                    //Replace with rigidbody motion
                    PathFindingMovement(targets[patrolIndex], 0.5f);
                    if (generalBehaviours.ReachedPos(transform.position, targets[patrolIndex].position, 0.2f)) {
                        patrolIndex++;
                    }
                    if (patrolIndex >= targets.Count) {
                        patrolIndex = 0;
                    }
                }
                break;
            case States.Combat:
                if (!environment.enemiesOnCombat.Contains(this)) {
                    environment.enemiesOnCombat.Add(this);
                    environment.prioritizedEnemies.Add(this);
                }
                anim.SetBool("Patrol", false);
                anim.SetBool("Combat", true);
                RotateTowards(player, rotateSpeed);
                enCombatRef.Dueling();
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
    //Make different overloads for this function instead
    #region Damage Overloads
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
        player.GetComponent<CallOfBeyond>().wrath += (dmg * 0.1f);
        //Will process the shield amount, and return the health
        float newHealth = ShieldDamager(dmg);
        if (newHealth != health)
            anim.SetTrigger("Hit");
        health = newHealth;
        shieldRecoveryCooldown = auxShieldRecoveryCooldown;
        enCombatRef.swordColl.enabled = false;
        enCombatRef.attacking = false;
        //Substract a bit from the probability to allow movement to be restablished
        enCombatRef.attackProbability -= UnityEngine.Random.Range(0.2f, 1f);
        canMove = true;
        environment.detected = true;
    }
    #endregion
    public void HealthBehaviour () {
        text.text = health.ToString();
        if (health <= 0) {
            if (environment.enemiesOnCombat.Contains(this)) {
                environment.enemiesOnCombat.Remove(this);
                environment.prioritizedEnemies.Remove(this);
                if (environment.enemiesOnCombat.Count <= 0) {
                    environment.enemyCntr = 0;
                }
            }
            Destroy(gameObject);
        }
        if (burn) {
            //DamageOverTime();
        }
    }
    private void StealthBehaviour () {
        if (!environment.detected && detected) {
            environment.detected = true;
        }
        if (environment.detected && generalBehaviours.ReachedPos(transform.position, player.position, radius * 1.5f)) {
            state = States.Combat;
        }
        //If the player is too close, then the enemy will turn slowly
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= radius) {
            //ƒ(x) = |-(distance - threshold_to_hit_1)|
            detectionP += Time.deltaTime / Mathf.Abs(-(distance - 3.5f));
            detectionP = Mathf.Clamp(detectionP, float.MinValue, 1f);
        }
        else {
            if (detectionP > 0f)
                detectionP -= Time.deltaTime;
            detectionP = Mathf.Clamp(detectionP, 0f, 1f);
        }
        if (detectionP >= 1) {
            state = States.Combat;
        }
    }

    private void OnTriggerEnter(Collider other) {
        var combatController = player.GetComponent<Combat>();
        //Detects if the enemy is being hit by the player's sword
        if (other.transform.CompareTag("Sword")) {
            this.Damage(combatController.swordDamage, 0f, 0f);
            Debug.Log(other.transform.forward);
            hit = true;
            combatController.source.PlayOneShot(combatController.clip);
            //Apply velocity change;
            MoveOnHit(other.transform);
        }
    }
    private void MoveOnHit (Transform damagingObj) {
        //Maybe set a time limit just like the dash
        rig.AddForce(damagingObj.forward * forceMult, ForceMode.Impulse);
    }

    bool followingPlayer = false;

    public void PathFindingMovement (Transform target, float minDistance) {
        //Destination - source
        //Move this to variable and function UpdateAnimatorParameters()
        if (generalBehaviours.ReachedPos(transform.position, target.position, minDistance)) {
            Stop();
        }
        if (canMove) {
            if (!stopped) {
                anim.SetBool("Walking", true);
                followingPlayer = CollisionCheck(target);
                if (followingPlayer) {
                    unitPathfinder.enabled = false;
                    //Replace this method to control speed
                    rig.MovePosition(Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime));
                }
                else {
                    unitPathfinder.enabled = true;
                    unitPathfinder.speed = speed;
                    //Extract the path
                    unitPathfinder.goTo(target.position);
                }
            }
        }
    }
    public void PathFindingMovement(Vector3 target, string targetTag, float minDistance) {
        //Destination - source
        //Move this to variable and function UpdateAnimatorParameters()
        if (generalBehaviours.ReachedPos(transform.position, target, minDistance)) {
            Stop();
        }
        if (canMove) {
            if (!stopped) {
                anim.SetBool("Walking", true);
                followingPlayer = CollisionCheck(target, targetTag);
                if (followingPlayer) {
                    unitPathfinder.enabled = false;
                    //Replace this method to control speed
                    rig.MovePosition(Vector3.Lerp(transform.position, target, speed * Time.deltaTime));
                }
                else {
                    unitPathfinder.enabled = true;
                    unitPathfinder.speed = speed;
                    //Extract the path
                    unitPathfinder.goTo(target);
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

    private bool CollisionCheck(Vector3 _target, string tag) {
        bool allHit = true;
        //Setup pivot to calculate the raycast
        for (int i = 0; i < rayPivot.Length; i++) {
            rayPivot[i].LookAt(_target);
            RaycastHit obstacleHit;
            if (Physics.Raycast(rayPivot[i].position, rayPivot[i].forward, out obstacleHit)) {
                if (obstacleHit.transform.CompareTag(tag)) {
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
        rig.velocity *= 0f;
        anim.SetBool("Walking", false);
        movingTimer = 0f;
        stopped = true;
    }

    private float ShieldDamager (float _dmg) {
        if (shield > 0) {
            shield -= _dmg;
            //Check again after shield has changed to get the health
            if (shield > 0) {
                _dmg -= shield;
            }
            else {
                _dmg = Mathf.Abs(shield);
                shield = 0;
            }
        }
        return shield > 0 ? health : health - _dmg;
    }

    private void RecoverShield () {
        if (shield != auxShield) {
            shieldRecoveryCooldown -= Time.deltaTime;
            if (shieldRecoveryCooldown <= 0) {
                shield = shield <= auxShield ? shield + Time.deltaTime : auxShield;
                shieldRecoveryCooldown = 0;
            }
        }
    }

    //Function for when the enum changes

}
