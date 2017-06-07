using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourMachine;

[AddComponentMenu("TPS/Transitions/AimingToLocomotion")]
public class AimingToLocomotion : StateBehaviour
{
	AimingState aiming;
	LocomotionState locomotion;
	Animator characterAnimator;
	Animator verticalAimingAnimator;

	Quaternion targetRotation;

	private void OnEnable()
	{
		if (aiming == null) aiming = GetComponent<AimingState>();
		if (locomotion == null) locomotion = GetComponent<LocomotionState>();
		if (characterAnimator == null) characterAnimator = GetComponent<Animator>();
		if (verticalAimingAnimator == null) verticalAimingAnimator = aiming.aimingRigAnimator;
		//targetRotation = Quaternion.LookRotation(aiming.cameraRig.transform.forward, Vector3.up);

		characterAnimator.SetBool("Aiming", false);
		locomotion.cameraBehaviour.gameObject.SetActive(true);

		SendEvent("AIMING_FORWARD");
	}
}
