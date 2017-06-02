using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour {
	#region Debug Fields
	public bool ikActive = false;
	public bool ikActiveLook = false;
	public bool ikActiveRightHand = false;
	public bool ikActiveLeftHand = false;
	public bool forceRightHandLookAtTarget;
	//Provavelmente ai ser false quando o cara estiver (andando e não atirando) e true quando ele estiver (atirando ou não andando).
	#endregion

	[SerializeField]
	private Animator characterAnimator;
	public Animator aimingRigAnimator;

	[SerializeField]
	private Transform
			rightHandIK = null,
			leftHandIK = null,
			lookIK = null,
			target;
	private Vector3 currentTargetPosition;

	private void OnEnable()
	{
		if (characterAnimator == null) characterAnimator = GetComponent<Animator>();		
		characterAnimator.SetBool("Aiming", true);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Right Hand IK"), 1f);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Left Hand IK"), 1f);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Legs"), 1f);		
	}

	private void OnAnimatorIK(int layerIndex)
	{
		#region Position IK Targets and Set IK Weights
		if (ikActive)
		{
			#region Look
			if (lookIK != null && ikActiveLook && layerIndex == 0)
			{
				characterAnimator.SetLookAtWeight(1, .5f, .5f, 1);
				currentTargetPosition = Vector3.Lerp(currentTargetPosition, target.position, .4f);
				characterAnimator.SetLookAtPosition(currentTargetPosition);
			}
			#endregion

			#region Right Hand
			if (rightHandIK != null && ikActiveRightHand && layerIndex == 1)
			{
				if (forceRightHandLookAtTarget)
				{
					rightHandIK.LookAt(currentTargetPosition, Vector3.up);
					rightHandIK.Rotate(0f, 0f, -90f);
				}
				characterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
				characterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
				characterAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.position);
				characterAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.rotation);
			}
			#endregion

			#region Left Hand
			if (leftHandIK != null && ikActiveLeftHand && layerIndex == 2)
			{
				characterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
				characterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
				characterAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);
				characterAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);
			}
			#endregion
		}

		else
		{
			characterAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
			characterAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
			characterAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
			characterAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
			characterAnimator.SetLookAtWeight(0);
		}
		#endregion
	}
}
