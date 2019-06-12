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
    public enum MasterType { Melee, Ranged, Buffer, Hybrid };
    public enum SubType { V1, V2, V3 };
    public enum States { Patrol, Heard, Warning, Combat };
    #region Enemy Selector
    [Header("Enemy type selector")]
    public MasterType masterType;
    [Header("Variants of the enemy type")]
    public SubType subType;
    #endregion
    #region Variables
    public bool detected;
    public TextMeshPro text;
    Transform player;
    [Header("Posciciones a los que se va a mover el agente, en orden")]
    public Transform[] targets;
    private bool shot;
    private States state;
    private NavMeshAgent agent;
    #endregion

    #region Stats
    [Header("Stats")]
    public float health = 1f;
    public GameObject[] spells;
    [Header("The highest, the fastest... (Recommended value: 2)")]
    [Header("The time it takes the enemy to face the player")]
    public float rotateSpeed = 1f;
    public float angle, radius;
    #endregion

    //Initialization
    private void Start() {
        detected = false;
        shot = false;
        state = States.Patrol;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        HealthBehaviour();
        Shoot();
        StealthBehaviour();
        detected = Detect(player);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, radius);
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
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * radius);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * radius);
    }

    public void Damage (float dmg) {
        health -= dmg;
    }

    public void HealthBehaviour () {
        text.text = health.ToString();
        if (health <= 0) {
            Destroy(gameObject);
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
    #endregion
    private void StealthBehaviour () {
        if (detected) {
            cntr += Time.deltaTime;
            state = States.Heard;
        }
        switch (state) {
            case States.Patrol:
                if (targets.Length != 0) {

                }
                break;
            case States.Heard:

                break;
            case States.Warning:
                break;
            case States.Combat:
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
