using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Programming {

    public class GeneralBehaviours {

        public bool Detect(Transform target, Transform agent, float r, float a) {
            Collider[] overlaps = new Collider[10];
            int count = Physics.OverlapSphereNonAlloc(agent.position, r, overlaps);
            for (int i = 0; i < count; i++) {
                if (overlaps[i] != null) {
                    if (overlaps[i].transform == target) {
                        Vector3 dir = (target.position - agent.position).normalized;
                        //Increase accuracy
                        dir.y *= 0f;
                        float _angle = Vector3.Angle(agent.forward, dir);
                        //Check if the target is in the field of view by comparing the angle with a max angle value
                        if (_angle <= a) {
                            Ray ray = new Ray(new Vector3(agent.position.x, agent.position.y + 0.5f, agent.position.z), target.position - agent.position);
                            RaycastHit hit;
                            if (Physics.Raycast(ray, out hit, r)) {
                                if (!hit.transform.CompareTag(target.tag)) {
                                    return false;
                                }
                                else {
                                    return true;
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void CollisionAvoidance (Transform target) {

        }

    }

}
