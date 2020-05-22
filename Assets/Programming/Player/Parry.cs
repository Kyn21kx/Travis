using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.Audio;

[RequireComponent(typeof (HealthManager))]
[RequireComponent(typeof (ManaManager))]
[RequireComponent (typeof(SmoothMovement))]
public class Parry : MonoBehaviour {

    #region Variables
    public const float perfectTime = 0.1f;
    public float timeDifference;
    [SerializeField]
    float auxHealth;
    float health;
    public bool collided;
    [SerializeField]
    float timeDown;
    public float dmg;
    [SerializeField]
    bool startTime;
    public bool perfect;
    public bool blocking;
    public float healthDifference;
    [SerializeField]
    private AudioClip soundEffect;
    public AudioSource camAudio;
    SmoothMovement movRef;
    private Animator anim;
    #endregion
    #region Stats Variables
    public float block_amount = 0.4f;
    public float manaCost = 30f;
    private Combat combatRef;
    #endregion
    //Testing vars
    [SerializeField]
    int cntr = 0;
    
    private void Start() {
        movRef = GetComponent<SmoothMovement>();
        health = GetComponent<HealthManager>().Health;
        auxHealth = health;
        anim = GetComponent<Animator>();
        combatRef = GetComponent<Combat>();
    }
    public void ActiveParry () {
        blocking = true;
        anim.SetBool("Blocking", true);
        combatRef.canMeleeAttack = false;
        movRef.walkSpeed = 2.5f;
        movRef.runSpeed = 2.5f;
        //GetComponent<SmoothMovement>().stealthSpeed *= 0.5f;
        timeDown = 0;
        startTime = true;
    }
    public void DisableParry () {
        blocking = false;
        combatRef.canMeleeAttack = true;
        movRef.walkSpeed = movRef.auxSpeed;
        movRef.runSpeed = movRef.auxRunningSpeed;
        //GetComponent<SmoothMovement>().stealthSpeed /= 0.5f;
        timeDown = 0;
        startTime = false;
    }
    private void Update() {
        health = GetComponent<HealthManager>().Health;
        VibDown();
        //Disable attack when blocking
        if (Input.GetButtonDown("LB")) {
            ActiveParry();
        }
        else if (Input.GetButtonUp("LB")) {
            DisableParry();
        }
        CountDown();
    }
    private void CountDown () {
        if (startTime) {
            timeDown += Time.deltaTime;
        }
        //Collided
        if (collided) {
            timeDifference = timeDown;
            perfect = PerfectParry();
            if (perfect) {
                cntr++;
                vib = true;
                dmg = 0f;
                camAudio.volume = 0.3f;
                camAudio.PlayOneShot(soundEffect);
                perfect = false;
                timeDifference = 0f;
            }
            else if (blocking && GetComponent<StaminaManager>().staminaAmount >= manaCost) {
                dmg *= block_amount;
                GetComponent<StaminaManager>().Reduce(manaCost);
            }
            GetComponent<HealthManager>().Health -= dmg;
            collided = false;
        }
    }

    

    private bool PerfectParry () {
        //0.16f
        if (timeDifference <= perfectTime && timeDifference != 0f) {
            return true;
        }
        else {
            return false;
        }
    }
    #region Global Aux variables
    float vibrationTimer = 0f;
    bool vib = false;
    #endregion
    
    private void VibDown () {
        if (vib) {
            GamePad.SetVibration(PlayerIndex.One, 0.4f, 0.4f);
            vibrationTimer += Time.deltaTime;
            if (vibrationTimer > 0.5f) {
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                vibrationTimer = 0;
                vib = false;
            }
            
        }
    }
}
