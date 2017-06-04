using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourMachine;

public class LocomotionToAiming : StateBehaviour
{
	private AimingState aiming;
	private LocomotionState locomotion;
	private Animator characterAnimator;
	private Animator verticalAimingAnimator;

	private Quaternion targetRotationHorizontal;
	private float targetRotationVertical;
	private float remainingTime;

	private void OnEnable()
	{
		remainingTime = 0.5f;
		if (aiming == null) aiming = GetComponent<AimingState>();
		if (locomotion == null) locomotion = GetComponent<LocomotionState>();
		if (characterAnimator == null) characterAnimator = GetComponent<Animator>();
		if (verticalAimingAnimator == null) verticalAimingAnimator = aiming.aimingRigAnimator;
		
		#region Calculate Target Rotations
		targetRotationHorizontal = Quaternion.LookRotation(locomotion.ProjectedCameraTransform.forward, Vector3.up);
		targetRotationHorizontal *= Quaternion.Euler(0f,45f,0f);

		targetRotationVertical = Mathf.Clamp((locomotion.cameraBehaviour.AngleYZ - Mathf.PI/2) * Mathf.Rad2Deg , -55f, 55f);
		targetRotationVertical = targetRotationVertical.Remap(-55f, 55f, 0f, 1f);
		aiming.VerticalAngle = targetRotationVertical;
		#endregion

		characterAnimator.SetBool("Aiming", true);		
	}
	private void FixedUpdate()
	{	
		if (transform.rotation == targetRotationHorizontal || remainingTime <= Mathf.Epsilon)
		{
			SendEvent("AIMING_FORWARD");
			return;
		}
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotationHorizontal, 1440 * Time.fixedDeltaTime);
		remainingTime -= Time.fixedDeltaTime;
	}
}

public static class ExtensionMethods {
 
public static float Remap (this float value, float from1, float to1, float from2, float to2) {
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}
   
}