﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : MonoBehaviour {
    #region Variables
    Image staminaBar;
    public float maxStamina;
    public bool reduced = false;
    public bool outOfStamina;
    float cntr = 0f;
    private float proportion;
    #endregion
    #region Stats variables
    public float cooldownRegeneration;
    public float staminaAmount = 50f;
    public float regenerationAmount = 5f;
    #endregion

    private void Start() {
        staminaBar = GameObject.FindGameObjectWithTag("Stamina").GetComponent<Image>();
        maxStamina = staminaAmount;
    }

    private void Update() {
        proportion = staminaAmount / maxStamina;
        staminaBar.fillAmount = proportion;
        Regenerate();
    }

    public void Reduce(float cost) {
        staminaAmount -= cost;
        cntr = 0f;
        reduced = true;
    }

    public void Regenerate() {
        if (reduced) {
            cntr += Time.deltaTime;
            if (cntr >= cooldownRegeneration) {
                staminaAmount += regenerationAmount * Time.deltaTime;
                staminaAmount = Mathf.Clamp(staminaAmount, 0f, maxStamina);
            }
        }
    }
}
