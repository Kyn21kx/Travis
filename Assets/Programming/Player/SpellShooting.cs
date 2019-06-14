using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class SpellShooting : MonoBehaviour {
    public enum Type {Fire, Electric, Ice, Magnetism};
    #region Variables
    [Header("1.- Fuego, 2.- Electrico, 3,- Hielo, 4.- Magnetismo")]
    public List<GameObject> basicSpells, holdingSpells;
    public GameObject spell, holdspell, heavy;
    public float height;
    [SerializeField]
    private Vector2 DPad;
    public Type type;
    [SerializeField]
    private float holdTime, releaseTime = 1f;
    private bool count;
    #endregion

    private void Start() {
        type = Type.Fire;
        count = false;
        holdTime = 0f;
    }

    private void Update() {
        TypeManager();
        _Input();
    }

    private void _Input() {
        DPad = new Vector2(Input.GetAxis("DPadX"), Input.GetAxis("DPadY"));
        if (Input.GetButtonDown("RB") || Input.GetKeyDown(KeyCode.Q)) {
            count = true;
        }
        if (Input.GetButtonUp("RB") || Input.GetKeyUp(KeyCode.Q)) {
            count = false;
            Shoot();
        }
        #region Timing for hold power
        if (count) {
            holdTime += Time.deltaTime;
        }
        else {
            holdTime = 0f;
        }
        if (holdTime >= releaseTime) {
            GamePad.SetVibration(PlayerIndex.One, 0.2f, 0.2f);
        }
        else {
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
        }
        #endregion
        #region DPadSelector
        if (DPad.y > 0) {
            type = Type.Fire;
        }
        if (DPad.y < 0) {
            type = Type.Ice;
        }
        if (DPad.x > 0) {
            type = Type.Electric;
        }
        if (DPad.x < 0) {
            type = Type.Magnetism;
        }
        #endregion
    }

    private void Shoot () {
        //Determine mana cost depending of the spell
        if (GetComponent<ManaManager>().manaAmount >= 10f && holdTime < releaseTime) {
            Instantiate(spell, new Vector3(transform.position.x, transform.position.y + height, transform.position.z), transform.rotation);
            GetComponent<ManaManager>().Reduce(10f);
        }
        else if (GetComponent<ManaManager>().manaAmount >= 15f && holdTime >= releaseTime) {
            Instantiate(holdspell, new Vector3(transform.position.x, transform.position.y + height, transform.position.z), transform.rotation);
            GetComponent<ManaManager>().Reduce(15f);
        }
    }
    private void TypeManager () {
        switch (type) {
            case Type.Fire:
                spell = basicSpells[0];
                spell.GetComponentInChildren<RFX1_TransformMotion>().appliedEffect = RFX1_TransformMotion.Effects.Burn;
                holdspell = holdingSpells[0];
                break;
            case Type.Electric:
                spell = basicSpells[1];
                holdspell = holdingSpells[1];
                break;
            case Type.Ice:
                spell = basicSpells[2];
                holdspell = holdingSpells[2];
                break;
            case Type.Magnetism:
                spell = basicSpells[3];
                holdspell = holdingSpells[3];
                break;
        }
    }

    private void Cooldown (float time) {

    }

}
