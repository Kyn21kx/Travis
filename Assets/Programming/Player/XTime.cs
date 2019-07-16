using System;
using UnityEngine;

namespace X_Time {
    public class XTime : MonoBehaviour {

        private float slowCntr = 0f;
        private float SlowingFactor;
        private float MaxTime;
        private bool slown;
        
        private void Update() {
            if (slown) {
                Time.timeScale = SlowingFactor;
                slowCntr += Time.deltaTime;
                if (slowCntr >= MaxTime) {
                    Time.timeScale = 1f;
                    slown = false;
                    MaxTime = 0f;
                    slowCntr = 0f;
                    SlowingFactor = 0f;
                }
            }

        }

        public void SlowTime(float slowingFactor, float maxTime) {
            MaxTime = maxTime;
            slown = true;
            SlowingFactor = slowingFactor;
        }
    }
}
