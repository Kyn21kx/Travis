using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallOfBeyond : MonoBehaviour {
    //Wrath level is the parameter that fuels the call of beyond, it can be upgraded for increased capacity
    #region Constants
    const float minumum = 37.5f;
    #endregion
    #region Variables
    public float wrath;
    #endregion
    #region Stats
    [Header("Stats")]
    public float maxWrath;
    #endregion

    /*TODO: When the game starts, the wrath level is just enough for you to activate your call of beyond (150/4)
     *Identify every single enemy in screen and their health at the very beggining of the game (or when they spawn)
     *Identify when any of those enemies get their health down, and add 10% of that to the wrath level.
    */
    private void Start() {
        wrath = minumum;
    }
}
