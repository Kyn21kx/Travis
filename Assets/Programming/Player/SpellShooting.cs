using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class SpellShooting : MonoBehaviour {
    public enum Type {Fire, Electric, Ice, Magnetism};
    #region Variables
    public List<GameObject> spells;
    public GameObject spell, holdspell;
    public float height;
    [SerializeField]
    private Vector2 DPad;
    public Type type;
    [SerializeField]
    private float holdTime;
    private bool count;
    #endregion

    private void Start() {
        type = Type.Fire;
        count = false;
        holdTime = 0f;
    }

    private void Update() {
        _Input();
    }

    private void _Input() {
        DPad = new Vector2(Input.GetAxis("DPadX"), Input.GetAxis("DPadY"));
        if (Input.GetButtonDown("RB")) {
            count = true;
        }
        if (Input.GetButtonUp("RB")) {
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
        if (holdTime >= 1.5f) {
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
        if (GetComponent<ManaManager>().manaAmount >= 10f && holdTime < 1.5f) {
            Instantiate(spell, new Vector3(transform.position.x, transform.position.y + height, transform.position.z), transform.rotation);
            GetComponent<ManaManager>().Reduce(10f);
        }
        else if (GetComponent<ManaManager>().manaAmount >= 15f && holdTime >= 1.5f) {
            Instantiate(holdspell, new Vector3(transform.position.x, transform.position.y + height, transform.position.z), transform.rotation);
            GetComponent<ManaManager>().Reduce(15f);
        }
    }
    private void TypeManager () {
        
    }

    private void Cooldown (float time) {

    }

}
