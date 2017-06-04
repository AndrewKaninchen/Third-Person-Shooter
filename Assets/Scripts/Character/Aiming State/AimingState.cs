using UnityEngine;
using BehaviourMachine;
using RootMotion.FinalIK;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FullBodyBipedIK))]
[RequireComponent(typeof(AimIK))]
//[AddComponentMenu("TPS/AimingState")]
public class AimingState : StateBehaviour
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
	public Transform cameraRig;	
	[SerializeField] private Animator characterAnimator;
	public Animator aimingRigAnimator;
	//private Animator rightHandIKAnimator;
	private Rigidbody rb;
	#endregion

	#region Fields
	private float
		targetVerticalAngle = 0f,
		verticalAngle = 0f;
	private bool
		shooting = false;
	public float VerticalAngle { get { return verticalAngle; } set { targetVerticalAngle = value;}}
	[SerializeField] private Transform 
		rightHandIK = null,
		leftHandIK = null,
		lookIK = null,
		target;
	private Vector3 currentTargetPosition;
	#endregion

	AimIK aimIK;
	LookAtIK lookAtIK;
	FullBodyBipedIK fbbIK;

	private void OnEnable()
	{
		if (characterAnimator == null)	characterAnimator = GetComponent<Animator>();
		if (rb == null)	rb = GetComponent<Rigidbody>();
		//if (rightHandIKAnimator == null) rightHandIKAnimator = rightHandIK.GetComponent<Animator>();

		cameraRig.gameObject.SetActive(true);
		characterAnimator.SetBool("Aiming", true);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Right Hand IK"), 1f);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Left Hand IK"), 1f);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Legs"), 1f);		

		aimingRigAnimator.Play("Vertical Aiming", 0, verticalAngle);
		aimingRigAnimator.Update(1f);

		if (aimIK == null) aimIK = GetComponent<AimIK>();
		if (fbbIK == null) fbbIK = GetComponent<FullBodyBipedIK>();
		if (lookAtIK == null) lookAtIK = GetComponent<LookAtIK>();
		aimIK.Disable();
		fbbIK.Disable();
		lookAtIK.Disable();

		fbbIK.solver.leftArmChain.bendConstraint.weight = 1f;		
	}

	private void OnDisable()
	{		
		characterAnimator.SetBool("Aiming", false);
		cameraRig.gameObject.SetActive(false);

		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Right Hand IK"), 0f);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Left Hand IK"), 0f);
		characterAnimator.SetLayerWeight(characterAnimator.GetLayerIndex("Legs"), 0f);
	}

	private void Update()
	{
		#region State Machine Transitions
		if ((Cursor.lockState == CursorLockMode.Locked) && !Input.GetMouseButton(1))
		{
			SendEvent("TOGGLE_AIMING");
		}
		#endregion

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
			characterAnimator.SetFloat("AimingVertical", verticalAngle);

			aimingRigAnimator.Play("Vertical Aiming", 0, verticalAngle);
			aimingRigAnimator.Update(1f);

			var rot = Vector3.up * Mathf.Clamp((Input.GetAxis("Mouse X") * aimingSensitivity.x), -10f, 10f);
			characterAnimator.SetFloat("TurnHorizontal", rot.y);

			transform.Rotate(rot);
		}
		
		#endregion

		#region Movement

		characterAnimator.SetFloat("MoveHorizontal", Input.GetAxis("Horizontal"), .1f, Time.deltaTime);
		characterAnimator.SetFloat("MoveVertical", Input.GetAxis("Vertical"), .1f, Time.deltaTime);

		Vector3 vel = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
		vel = cameraRig.TransformDirection(vel);
		
		//vel Quaternion.Euler(0f,45f,0f);
		rb.velocity = new Vector3 (vel.x * moveSpeed.x, 0f, vel.z * moveSpeed.z);
		
		Debug.DrawRay(transform.position, vel, Color.cyan);
		#endregion

		#region Shooting
		shooting = Input.GetMouseButton(0);
		//characterAnimator.SetBool("Shooting", shooting);
		#endregion
	}

	private void LateUpdate()
	{
		lookAtIK.solver.Update();
		aimIK.solver.Update();
				
		fbbIK.solver.leftHandEffector.position = leftHandIK.position;
		fbbIK.solver.leftHandEffector.rotation = leftHandIK.rotation;

		fbbIK.solver.Update();
	}

	/*private void OnAnimatorIK(int layerIndex)
	{
		#region Position Aiming Rig
		aimingRigAnimator.Play("Vertical Aiming", 0, verticalAngle);
		aimingRigAnimator.Update(Time.deltaTime);
		//Forcing the Aiming Rig Animator to Update before setting the IK positions to avoid lag.
		#endregion

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
	}*/
}