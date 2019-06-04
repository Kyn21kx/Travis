using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShooting : MonoBehaviour {
    public enum Type {Fire, Electric, Ice, Magnetism};
    #region Variables
    public List<GameObject> spells;
    public GameObject spell;
    public float height;
    [SerializeField]
    private Vector2 DPad;
    public Type type;
    #endregion

    private void Start() {
        type = Type.Fire;
    }

    private void FixedUpdate() {
        _Input();
    }

    private void _Input() {
        DPad = new Vector2(Input.GetAxis("DPadX"), Input.GetAxis("DPadY"));
        if (Input.GetButtonDown("RB")) {
            Shoot();
        }
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
        if (GetComponent<ManaManager>().manaAmount >= 10f) {
            Instantiate(spell, new Vector3(transform.position.x, transform.position.y + height, transform.position.z + 1), transform.rotation);
            GetComponent<ManaManager>().Reduce(10f);
        }
    }
    private void TypeManager () {
        
    }
}
