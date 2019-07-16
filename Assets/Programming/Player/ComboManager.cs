using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using X_Time;

[RequireComponent(typeof(XTime))]
public class ComboManager : MonoBehaviour {

    /*TODO:
     * Limit the maximum times an input can be pressed
     * Add all heavy and light combos
     * Add mixed combos
     * Fix animation with input
    */
    //Enums for the combos
    public enum Combos_Master {Default, A, B, C, Airbone, Dash, AirSmash};

    #region Variables
    public bool lockOn;
    private XTime xTime;
    [SerializeField]
    float spacingTime = 0f;
    [SerializeField]
    private bool startTime;
    //***Controls time and hits of input for the combo to reset
    [SerializeField]
    float maxTime = 3f;
    [SerializeField]
    int maxInput = 4;
    //***********
    [SerializeField]
    int btnCntrX = 0, btnCntrY = 0;
    [SerializeField]
    bool preserveCombo = false;
    public float previousSpacingTime;
    //Light combos controlled by different spacing time between the X button
    public Combos_Master lightCombo;
    [SerializeField]
    private AudioClip AirSmashClip;
    #endregion

    #region Stats
    public float airSmashRadius;
    public float airSmashDmg;
    #endregion

    private void Start() {
        xTime = GetComponent<XTime>();
    }

    private void Update() {
        XboxInput();
        TimeStart();
        Combo_Selector();
        Combo_Effect();
    }

    private void XboxInput () {
        if (Input.GetButtonDown("X")) {
            btnCntrX++;
            previousSpacingTime = spacingTime;
            spacingTime = 0;
            startTime = true;
        }
    }
    private void Combo_Selector () {
        if (!preserveCombo) {
            if (!GetComponent<SmoothMovement>().grounded && Input.GetButtonDown("Y")) {
                GetComponent<Rigidbody>().velocity += Vector3.down * 45f;
                lightCombo = Combos_Master.AirSmash;
                //Animation here
            }
        }
    }
    float timeScale = 0f;
    private void Combo_Effect () {
        switch (lightCombo) {
            case Combos_Master.Default:
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
                    xTime.SlowTime(0.5f, 0.3f);
                    lightCombo = Combos_Master.Default;
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
