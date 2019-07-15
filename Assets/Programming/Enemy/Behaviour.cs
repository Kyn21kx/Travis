using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class Behaviour : MonoBehaviour {

    /*
     * TODO:
     * Health Manager
     * Detection (different rates)
     * Levels
     * Shooting
     * Avoiding
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
    public bool burn;
    public TextMeshPro text;
    private Enemy_Environment environment;
    Transform player;
    [Header("Posciciones a los que se va a mover el agente, en orden")]
    public Transform[] targets;
    private bool shot;
    private States state;
    private NavMeshAgent agent;
    private float dmgOverTime, time;
    Vector3 toPlayerPos;
    float playerRadius, playerAngleX, playerAngleZ;
    float auxSpeed;
    #endregion

    #region Stats
    [Header("Stats")]
    public float health = 1f;
    public GameObject[] spells;
    [Header("The highest, the fastest... (Recommended value: 2)")]
    [Header("The time it takes the enemy to face the player")]
    public float rotateSpeed = 1f;
    public float angle, radius, range;
    #endregion

    //Initialization
    private void Start() {
        detected = false;
        shot = false;
        state = States.Patrol;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        environment = GameObject.FindGameObjectWithTag("GlobalParameters").GetComponent<Enemy_Environment>();
        toPlayerPos = Vector3.zero;
        auxSpeed = agent.speed;
    }

    private void Update() {
        HealthBehaviour();
        Shoot();
        StealthBehaviour();
        detected = Detect(player);
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
        }
        else {
            Gizmos.color = Color.green;
        }
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * radius);
    }
    float time_down;
    public void Damage (float dmg, float t, float dot) {
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
    }
    public void HealthBehaviour () {
        text.text = health.ToString();
        if (health <= 0) {
            Destroy(gameObject);
        }
        if (burn) {
            DamageOverTime();
        }
    }

    private void DamageOverTime () {
        time_down += Time.deltaTime;
        if (time > 0) {
            if (time_down >= 1f) {
                health -= dmgOverTime;
                time--;
                time_down = 0f;
            }
        }
    }

    private bool Detect (Transform target) {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, overlaps);
        for (int i = 0; i < count + 1; i++) {
            if(overlaps[i] != null) {
                if (overlaps[i].transform == target) {
                    Vector3 dir = (target.position - transform.position).normalized;
                    //Increase accuracy
                    dir.y *= 0f;
                    float _angle = Vector3.Angle(transform.forward, dir);
                    //Check if the player is in the field of view by comparing the angle with a max angle value
                    if (_angle <= angle) {
                        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), target.position - transform.position);
                        RaycastHit hit;
                         if (Physics.Raycast(ray, out hit, radius)) {
                             if (!hit.transform.CompareTag("Player")) {
                                 return false;
                             }
                             else {
                                return true;
                            }
                         }
                        return true;
                    }
                }
            }
        }
        return false;
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
        switch (state) {
            case States.Patrol:
                agent.speed = auxSpeed;
                agent.isStopped = false;
                if (targets.Length != 0) {
                    disToPos = Vector3.Distance(transform.position, targets[patrolIndex].position);
                    agent.SetDestination(targets[patrolIndex].position);
                    if (disToPos <= agent.stoppingDistance) {
                        patrolIndex++;
                    }
                    if (patrolIndex >= targets.Length) {
                        patrolIndex = 0;
                    }
                }
                break;
            case States.Combat:
                Vector3 target = player.position - transform.position;
                var rotation = Quaternion.LookRotation(target);
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
                agent.speed = 6;
                switch (masterType) {
                    case MasterType.Melee:
                        //TODO: Make the enemy go around the player in a pseudo random distance
                        cntr += Time.deltaTime;
                        //Getting the point x,z on the circumference.
                        float dZ = Mathf.Abs(player.position.z - transform.position.z);
                        float dX = Mathf.Abs(player.position.x - transform.position.x);
                        if (cntr <= 0.2f) {
                            playerAngleX = Mathf.Atan((dZ / dX)) * Mathf.Rad2Deg;
                            Debug.Log(playerAngleX);
                        }
                        float x = range * Mathf.Cos(playerAngleX) + player.position.x;
                        float z = range * Mathf.Sin(playerAngleX) + player.position.z;
                        toPlayerPos = new Vector3(x, player.position.y, z);
                        #region Conditions to attack
                        if (distanceToPlayer <= range && detected) {
                            agent.isStopped = true;
                            cntr = 0f;
                            //numberOfAttacks = 1;
                            //Play animation, rotate, and attack the player
                            //If after the animation is completed, the player is still in range, set the number of attacks to 2.
                        }
                        else {
                            agent.isStopped = false;
                            agent.SetDestination(toPlayerPos);
                        }

                        #endregion
                        switch (subType) {
                            case SubType.V1:
                                break;
                            case SubType.V2:
                                break;
                            case SubType.V3:
                                break;
                        }
                        break;
                    case MasterType.Ranged:
                        //TODO: Make the enemy take an aiming position around his allies, or a cover in the terrain
                        switch (subType) {
                            case SubType.V1:
                                break;
                            case SubType.V2:
                                break;
                            case SubType.V3:
                                break;
                        }
                        break;
                    case MasterType.Buffer:
                        switch (subType) {
                            case SubType.V1:
                                break;
                            case SubType.V2:
                                break;
                            case SubType.V3:
                                break;
                        }
                        break;
                    case MasterType.Hybrid:
                        switch (subType) {
                            case SubType.V1:
                                break;
                            case SubType.V2:
                                break;
                            case SubType.V3:
                                break;
                        }
                        break;
                    case MasterType.Boss:
                        break;
                }
                break;
        }
    }

    private void Shoot () {
        /*if (detected) {
            Vector3 target = player.position- transform.position;
            var rotation = Quaternion.LookRotation(target);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
        }*/
    }

}
