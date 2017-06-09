using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourMachine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[AddComponentMenu("TPS/States/LocomotionState")]
public class LocomotionState : StateBehaviour {

	#region Fields
	public float walkSpeed = 1.5f;
	public float runSpeed = 5f;
	public float sprintSpeed = 7f;
	#endregion

	#region Components
	private Rigidbody rb;
	private Animator anim;
	#endregion

	#region Internal Variables
	private float sprintMultiplier;
	private float speedMultiplier;
	private Vector3 inputDir;
	private bool isRunning = true;
	private bool isRolling = false;

	//[Header("Camera Variables")]
	[SerializeField] private Transform cameraTransform;
	private Transform projectedCameraTransform;
	public Transform ProjectedCameraTransform { get { return projectedCameraTransform; } }
	public CameraOrbit cameraBehaviour;
	#endregion

	private void Start ()
	{
		projectedCameraTransform = new GameObject(gameObject.name + "_LocomotionCameraProjectionOnGround").transform;
		projectedCameraTransform.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
	}

	private void OnEnable()
	{
		if (rb == null) rb = GetComponent<Rigidbody>();
		if (anim == null) anim = GetComponent<Animator>();
		if (cameraBehaviour == null) cameraBehaviour = cameraTransform.GetComponentInChildren<CameraOrbit>();
		cameraTransform.gameObject.SetActive(true);
		anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 1f);
	}

	private void OnDisable()
	{
		if(cameraTransform != null)
			cameraTransform.gameObject.SetActive(false);
		anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 0f);
	}

	private void Update ()
	{
		if (!isRolling)
		{
			#region Check For State Transition Events
			if (Input.GetMouseButtonDown(1))
			{
				SendEvent("TOGGLE_AIMING");
			}

			#endregion

			if (Input.GetKeyDown(KeyCode.CapsLock))
			{
				isRunning = !isRunning;
				anim.SetBool("Running", isRunning);
			}

			var isSprinting = Input.GetKey(KeyCode.LeftShift);
			anim.SetBool("Sprinting", isSprinting);

			inputDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			speedMultiplier = Mathf.MoveTowards(speedMultiplier, (inputDir.sqrMagnitude > Mathf.Epsilon) ? (isSprinting ? (sprintSpeed) : (isRunning ? runSpeed : walkSpeed)) : 0f, Time.deltaTime * 10f);

			if ((inputDir.sqrMagnitude > 0f) && Input.GetKeyDown(KeyCode.Space))
			{
				//speedMultiplier = 8f;
				anim.SetFloat("ArmsLayerWeight", 1f);
				anim.SetTrigger("Roll");
				isRolling = true;				
			}
		}

		else
		{
			//anim.SetLayerWeight(anim.GetLayerIndex("Arms"), anim.GetFloat("ArmsLayerWeight"));
		}

	}

	public void StopRolling()
	{
		isRolling = false;
		//anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 1f);
	}

	private void FixedUpdate()
	{
		inputDir.Normalize();
		projectedCameraTransform.position = new Vector3(cameraTransform.position.x, 0f, cameraTransform.position.z);
		projectedCameraTransform.LookAt(new Vector3(transform.position.x, 0f, transform.position.z));
		//rb.AddForce(projectedCameraTransform.TransformDirection(inputDir) * speedMultiplier * (sprintMultiplier * speedMultiplier / moveSpeed), ForceMode.VelocityChange);


		var targetVelocity = projectedCameraTransform.TransformDirection(inputDir) * speedMultiplier;
		targetVelocity.y = rb.velocity.y;

		rb.velocity = Vector3.MoveTowards
		(
			rb.velocity,
			targetVelocity,
			Time.fixedDeltaTime * 30f
		);

		if (rb.velocity.sqrMagnitude > Mathf.Epsilon)
		{
			transform.LookAt(transform.position + new Vector3(rb.velocity.x, 0f, rb.velocity.z), Vector3.up);
			anim.SetBool("Walking", true);
		}
		else
		{
			anim.SetBool("Walking", false);
		}

		anim.SetFloat("MoveVertical", rb.velocity.magnitude, .1f, Time.fixedDeltaTime);


		//anim.SetFloat("MoveVertical", speedMultiplier*sprintMultiplier, .1f, Time.fixedDeltaTime);
	}
}