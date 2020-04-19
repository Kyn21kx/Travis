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
        try {
            int currentType = System.Array.IndexOf(System.Enum.GetValues(typeof(Type)), type);
            spell = basicSpells[currentType];
            holdspell = holdingSpells[currentType];
        }
        catch (System.Exception err) {
            Debug.Log("Prefabs have not been set, or the index is out of range: " + err.Message);
        }
    }

    private void Cooldown (float time) {

    }

}
