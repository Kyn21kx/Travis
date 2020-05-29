using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SmoothMovement))]

public class FeetGrounder : MonoBehaviour {

    #region Variables
    private SmoothMovement movRef;
    private Vector3 rFootPos, lFootPos, rFootIkPos, lFootIkPos;
    private Quaternion leftIkRot, rightIkRot;

    private float lastPelvisPosY, lastRightFootPosY, lastLeftFootPosY;
    [SerializeField]
    private float disToGround, dis;
    private Animator anim;
    
    [SerializeField]
    private LayerMask environmentLayer;
    private float pelvisOffset;
    [SerializeField]
    private float pelvisUpDownSpeed;
    [SerializeField]
    private float feetToIkPosSpeed;
    #endregion

    private void Start() {
        anim = GetComponent<Animator>();
        movRef = GetComponent<SmoothMovement>();
    }

    private void FixedUpdate() {
        AdjustFeetTarget(ref rFootPos, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref lFootPos, HumanBodyBones.LeftFoot);

        //Raycast
        FeetPosSolver(rFootPos, ref rFootIkPos, ref rightIkRot);
        FeetPosSolver(lFootPos, ref lFootIkPos, ref leftIkRot);

    }
    private void OnAnimatorIK(int layerIndex) {
        MovePelvis();
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("RightFootCurve"));
        MoveToIkPoint(AvatarIKGoal.RightFoot, rFootIkPos, rightIkRot, ref lastRightFootPosY);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("RightFootCurve"));
        MoveToIkPoint(AvatarIKGoal.RightFoot, lFootIkPos, leftIkRot, ref lastLeftFootPosY);
    }

    private void MoveToIkPoint (AvatarIKGoal foot, Vector3 posIkHolder, Quaternion rotIkHolder, ref float lastFootPosY) {
        Vector3 targetIkPos = anim.GetIKPosition(foot);
        if (posIkHolder != Vector3.zero) {
            targetIkPos = transform.InverseTransformPoint(targetIkPos);
            posIkHolder = transform.InverseTransformPoint(posIkHolder);
            float _y = Mathf.Lerp(lastFootPosY, posIkHolder.y, feetToIkPosSpeed);
            targetIkPos.y += _y;
            lastFootPosY = _y;
            targetIkPos = transform.TransformPoint(targetIkPos);
            anim.SetIKRotation(foot, rotIkHolder);
        }
        anim.SetIKPosition(foot, targetIkPos);
    }
    private void MovePelvis () {
        if (rFootIkPos.Equals(Vector3.zero) || lFootIkPos.Equals(Vector3.zero) || lastPelvisPosY == 0f) {
            lastPelvisPosY = anim.bodyPosition.y;
            return;
        }
        else {
            float lOffsetPos = lFootIkPos.y - transform.position.y;
            float rOffsetPos = rFootIkPos.y - transform.position.y;
            float totalOffset = (lOffsetPos < rOffsetPos) ? lOffsetPos : rOffsetPos;
            Vector3 newPelvisPos = anim.bodyPosition + Vector3.up * totalOffset;
            newPelvisPos.y = Mathf.Lerp(lastPelvisPosY, newPelvisPos.y, pelvisUpDownSpeed);
            anim.bodyPosition = newPelvisPos;
            lastPelvisPosY = anim.bodyPosition.y;
        }
    }

    private void FeetPosSolver (Vector3 fromSkyPos, ref Vector3 feetIkPos, ref Quaternion feetIkRot) {
        RaycastHit feetOutHit;
        if (Physics.Raycast(fromSkyPos, Vector3.down, out feetOutHit, dis + disToGround, environmentLayer)) {
            feetIkPos = fromSkyPos;
            feetIkPos.y = feetOutHit.point.y + pelvisOffset;
            feetIkRot = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
            return;
        }
        feetIkPos = Vector3.zero;
    }
   
    private void AdjustFeetTarget(ref Vector3 feetPos, HumanBodyBones foot) {
        feetPos = anim.GetBoneTransform(foot).position;
        feetPos.y = transform.position.y + disToGround;
    }

}
