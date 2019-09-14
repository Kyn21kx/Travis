using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using X_Time;
using System.Linq;

[RequireComponent(typeof(XTime))]
public class ComboManager : MonoBehaviour {

    /*TODO:
     * Limit the maximum times an input can be pressed
     * Add all heavy and light combos
     * Add mixed combos
     * Fix animation with input
    */
    //Enums for the combos
    public enum Combos_Master {Default, A, B, C, D, Airbone, Dash, AirSmash};
    private enum Btn {X, Y, B, None};
    #region Variables
    public bool lockOn;
    private XTime xTime;
    [SerializeField]
    private ParticleSystem distortionEffect;
    [SerializeField]
    float spacingTime = 0f;
    [SerializeField]
    private bool startTime;
    //***Controls time and hits of input for the combo to reset
    [SerializeField]
    float maxTime = 3f;
    [SerializeField]
    int maxInput = 12;
    //***********
    [SerializeField]
    int btnCntrX = 0, btnCntrY = 0;
    [SerializeField]
    bool preserveCombo = false;
    [SerializeField]
    private Btn previousBtn, currentBtn;
    public float previousSpacingTime;
    //Light combos controlled by different spacing time between the X button
    public Combos_Master lightCombo, heavyCombo;
    [SerializeField]
    private AudioClip AirSmashClip;
    List<Combos_Master> lightAvailableCombos;
    List<Combos_Master> heavyAvailableCombos;
    #endregion

    #region Stats
    public float airSmashRadius;
    public float airSmashDmg;
    #endregion

    private void Start() {
        xTime = GetComponent<XTime>();
        currentBtn = Btn.None;
        previousBtn = Btn.None;
        lightCombo = Combos_Master.Default;
        lightAvailableCombos = new List<Combos_Master>();
        heavyAvailableCombos =  new List<Combos_Master>();
    }

    private void Update() {
        XboxInput();
        TimeStart();
        Combo_Selector();
        Combo_Effect();
        foreach (var combo in lightAvailableCombos) {
            Debug.Log(combo);
        }
    }

    private void XboxInput () {
        if (Input.GetButtonDown("X")) {
            btnCntrX++;
            if (currentBtn != Btn.None) {
                previousBtn = currentBtn;
            }
            currentBtn = Btn.X;
            previousSpacingTime = spacingTime;
            spacingTime = 0;
            startTime = true;
        }
        else if (Input.GetButtonDown("Y")) {
            btnCntrY++;
            if (currentBtn != Btn.None) {
                previousBtn = currentBtn;
            }
            currentBtn = Btn.Y;
            previousSpacingTime = spacingTime;
            spacingTime = 0;
            startTime = true;
        }
    }
    private void Combo_Selector () {
        if (!preserveCombo) {
            if (!GetComponent<SmoothMovement>().grounded && Input.GetButtonDown("Y")) {
                preserveCombo = true;
                GetComponent<Rigidbody>().velocity += Vector3.down * 45f;
                GetComponent<SmoothMovement>().canMove = false;
                lightCombo = Combos_Master.AirSmash;
                //Animation here
            }
            if (btnCntrX > 1) {
                //Deshabilitar combos que empiezan con Y o tengan menos speacing time
                lightAvailableCombos.Remove(Combos_Master.D);
                if (previousSpacingTime < 0.5f) {
                    lightAvailableCombos.Remove(Combos_Master.C);
                }
                else {
                    foreach (var combo in lightAvailableCombos) {
                        if (combo != Combos_Master.C) {
                            lightAvailableCombos.Remove(combo);
                        }
                    }
                }
                lightCombo = lightAvailableCombos.First();
            }
        }
    }
    private void Combo_Effect () {
        switch (lightCombo) {
            case Combos_Master.Default:
                if (lightAvailableCombos.Count.Equals(0)) {
                    foreach (var combo in Enum.GetValues(typeof(Combos_Master)).Cast<Combos_Master>().ToList()) {
                        lightAvailableCombos.Add(combo);
                    }
                }
                if (heavyAvailableCombos.Count.Equals(0)) {
                    foreach (var combo in Enum.GetValues(typeof(Combos_Master)).Cast<Combos_Master>().ToList()) {
                        heavyAvailableCombos.Add(combo);
                    }
                }
                break;
            case Combos_Master.A:
                break;
            case Combos_Master.B:
                break;
            case Combos_Master.C:
                break;
            case Combos_Master.Airbone:
                break;
            case Combos_Master.Dash:
                break;
            case Combos_Master.AirSmash:
                if (GetComponent<SmoothMovement>().grounded) {

                    GameObject[] affectedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in affectedEnemies) {
                        float dis = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(enemy.transform.position.x, enemy.transform.position.z));
                        if (dis <= airSmashRadius) {
                            enemy.GetComponent<Behaviour>().Damage(airSmashDmg, 0f, 0f);
                        }
                    }
                    AudioSource.PlayClipAtPoint(AirSmashClip, transform.position);
                    distortionEffect.Play();
                    xTime.SlowTime(0.5f, 0.3f);
                    preserveCombo = false;
                    lightCombo = Combos_Master.Default;
                    //Activate Can Move when the animation stops
                    GetComponent<SmoothMovement>().canMove = true;
                }
                break;
        }
    }

    private void TimeStart () {
        if (startTime) {
            spacingTime += Time.deltaTime;
            spacingTime = Mathf.Clamp(spacingTime, 0f, maxTime);
        }
        //Clear time once it gets to the maximum time allowed (maxTime)
        #region Clear
        if (spacingTime >= maxTime) {
            spacingTime = 0;
            startTime = false;
            btnCntrX = 0;
            preserveCombo = false;
            btnCntrY = 0;
        }
        if (btnCntrX > maxInput) {
            btnCntrX = 1;
            preserveCombo = false;
            spacingTime = 0;
        }
        if (btnCntrY > maxInput) {
            btnCntrY = 1;
            preserveCombo = false;
            spacingTime = 0;
        }
        #endregion
    }

}
