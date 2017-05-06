using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]


public class AimingState : StateMachineTry.State
{
	#region Debug Fields
	public bool ikActive = false;
	public bool ikActiveLook = false;
	public bool ikActiveRightHand = false;
	public bool ikActiveLeftHand = false;
	public bool forceRightHandLookAtTarget; 
	//Provavelmente ai ser false quando o cara estiver (andando e não atirando) e true quando ele estiver (atirando ou não andando).
	#endregion

	#region Settings Fields
	public Vector2 aimingSensitivity = new Vector2(.1f, .1f);
	public Vector3 moveSpeed;
	#endregion

	#region Component Fields
	[SerializeField] private Transform cameraRig;	
	[SerializeField] private Animator characterAnimatorController;	
	[SerializeField] private Animator aimingRigAnimatorController;
	private Animator rightHandIKAnimator;
	private Rigidbody rb;
	#endregion

	#region Fields
	private float
		targetVerticalAngle = 0f,
		verticalAngle = 0f;	

	[SerializeField] private Transform 
		rightHandIK = null,
		leftHandIK = null,
		lookIK = null,
		target;
	#endregion

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rightHandIKAnimator = rightHandIK.GetComponent<Animator>();
	}

	private void Update()
	{
		#region Cursor Locking
		if (Input.GetKeyDown(KeyCode.K))
		{
			if (Cursor.lockState == CursorLockMode.Locked)
				Cursor.lockState = CursorLockMode.None;
			else
				Cursor.lockState = CursorLockMode.Locked;
		}
		#endregion

		#region Rotation
		if(Cursor.lockState == CursorLockMode.Locked)
		{
			targetVerticalAngle = Mathf.Clamp01(targetVerticalAngle + Input.GetAxis("Mouse Y") * aimingSensitivity.y);
			verticalAngle = Mathf.Lerp(verticalAngle, targetVerticalAngle, .3f);
			transform.Rotate(Vector3.up * Mathf.Clamp((Input.GetAxis("Mouse X") * aimingSensitivity.x), -10f, 10f));
		}
		
		#endregion

		#region Movement

		characterAnimatorController.SetFloat("MoveHorizontal", Input.GetAxis("Horizontal"), .1f, Time.deltaTime);
		characterAnimatorController.SetFloat("MoveVertical", Input.GetAxis("Vertical"), .1f, Time.deltaTime);

		Vector3 vel = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
		vel = cameraRig.TransformDirection(vel);		
		rb.velocity = new Vector3 (vel.x * moveSpeed.x, 0f, vel.z * moveSpeed.z);
		
		Debug.DrawRay(transform.position, vel, Color.cyan);
		#endregion
	}

	void OnAnimatorIK(int layerIndex)
	{
		#region Position Aiming Rig
		//if (Cursor.lockState == CursorLockMode.Locked)
		{
			aimingRigAnimatorController.Play("Vertical Aiming", 0, verticalAngle);
			aimingRigAnimatorController.Update(Time.deltaTime);
		}
		//Forcing the Aiming Rig Animator to Update before setting the IK positions to avoid lag.
		#endregion

		#region Position IK Targets and Set IK Weights
		if (ikActive)
		{
			#region Look
			if (lookIK != null && ikActiveLook && layerIndex == 0)
			{
				characterAnimatorController.SetLookAtWeight(1, .5f, .5f, 1);
				characterAnimatorController.SetLookAtPosition(target.position);
			}
			#endregion

			#region Right Hand
			if (rightHandIK != null && ikActiveRightHand && layerIndex == 1)
			{
				if (forceRightHandLookAtTarget)
				{
					rightHandIK.LookAt(target, Vector3.up);
					rightHandIK.Rotate(0f, 0f, -90f);

					rightHandIKAnimator.Play("Vertical Aiming", 0, verticalAngle);
					rightHandIKAnimator.Update(Time.deltaTime);

				}
				characterAnimatorController.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
				characterAnimatorController.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
				characterAnimatorController.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.position);
				characterAnimatorController.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.rotation);
			}
			#endregion

			#region Left Hand
			if (leftHandIK != null && ikActiveLeftHand && layerIndex == 2)
			{
				characterAnimatorController.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
				characterAnimatorController.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
				characterAnimatorController.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.position);
				characterAnimatorController.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.rotation);
			}
			#endregion
		}

		else
		{
			characterAnimatorController.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
			characterAnimatorController.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
			characterAnimatorController.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
			characterAnimatorController.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
			characterAnimatorController.SetLookAtWeight(0);
		} 
		#endregion
	}
}
