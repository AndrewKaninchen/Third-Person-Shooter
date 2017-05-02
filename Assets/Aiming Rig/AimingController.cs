using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class AimingController : MonoBehaviour
{
	#region Components
	public Transform cameraRig;
	public Animator characterAnimatorController;
	private Rigidbody rb;
	#endregion

	#region Aiming Rig Variables
	public Animator aimingRigAnimatorController;
	private float targetVerticalAngle = 0f;
	private float verticalAngle = 0f;

	public bool ikActive = false;
	public Transform rightHandIK = null;
	public Transform leftHandIK = null;
	public Transform lookIK = null;
	#endregion

	#region Settings
	public Vector2 aimingSensitivity = new Vector2(.1f, .1f);
	public Vector3 moveSpeed;
	#endregion

	void Start()
	{
		rb = GetComponent<Rigidbody>();
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
		targetVerticalAngle = Mathf.Clamp01(targetVerticalAngle + Input.GetAxis("Mouse Y") * aimingSensitivity.y);
		verticalAngle = Mathf.Lerp(verticalAngle, targetVerticalAngle, .3f);
		transform.Rotate(Vector3.up * Mathf.Clamp((Input.GetAxis("Mouse X") * aimingSensitivity.x), -30f, 30f));
		#endregion

		#region Movement
		
		Vector3 vel = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
		vel = cameraRig.TransformDirection(vel);		
		rb.velocity = new Vector3 (vel.x * moveSpeed.x, 0f, vel.z * moveSpeed.z);
		
		Debug.DrawRay(transform.position, vel, Color.cyan);
		#endregion
	}

	void OnAnimatorIK()
	{
		#region Position Aiming Rig		
		aimingRigAnimatorController.Play("Vertical Aiming", 0, verticalAngle);
		aimingRigAnimatorController.Update(Time.deltaTime);
		//Forcing the Aiming Rig Animator to Update before setting the IK positions to avoid lag.
		#endregion

		#region Position IK Targets and Set IK Weights
		if (ikActive)
		{
			#region Look
			if (lookIK != null)
			{
				characterAnimatorController.SetLookAtWeight(1, .5f, .5f, 1);
				characterAnimatorController.SetLookAtPosition(lookIK.position);
			}
			#endregion

			#region Right Hand
			if (rightHandIK != null)
			{
				characterAnimatorController.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
				characterAnimatorController.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
				characterAnimatorController.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.position);
				characterAnimatorController.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.rotation);
			}
			#endregion

			#region Left Hand
			if (leftHandIK != null)
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
			characterAnimatorController.SetLookAtWeight(0);
		} 
		#endregion
	}	
}
