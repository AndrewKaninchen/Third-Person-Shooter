using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourMachine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[AddComponentMenu("TPS/LocomotionState")]
public class LocomotionState : StateBehaviour {

	#region Fields
	public float moveSpeed = 5f;
	#endregion

	#region Components
	private Rigidbody rb;
	private Animator anim;
	#endregion

	#region Internal Variables
	private float sprintMultiplier;
	private float speedMultiplier;
	private Vector3 inputDir;

	//[Header("Camera Variables")]
	[SerializeField] private Transform cameraTransform;
	private Transform projectedCameraTransform;
	public Transform ProjectedCameraTransform { get { return projectedCameraTransform; } }
	public CameraOrbit cameraBehaviour;
	#endregion

	private void Start ()
	{
		projectedCameraTransform = new GameObject().transform;
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
		#region Check For State Transition Events
		if(Input.GetMouseButtonDown(1))
		{
			SendEvent("TOGGLE_AIMING");
		}

		#endregion

		var isSprinting = Input.GetKey(KeyCode.LeftShift);
		inputDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		speedMultiplier = Mathf.MoveTowards(speedMultiplier, (inputDir.sqrMagnitude > Mathf.Epsilon)?moveSpeed:0f, Time.deltaTime * 30f);
		sprintMultiplier = Mathf.MoveTowards(sprintMultiplier, ((isSprinting) ? 2f : 1f), Time.deltaTime * 30f);
	}

	private void FixedUpdate()
	{
		inputDir.Normalize();
		projectedCameraTransform.position = new Vector3 (cameraTransform.position.x, 0f, cameraTransform.position.z);		
		projectedCameraTransform.LookAt(new Vector3 (transform.position.x, 0f, transform.position.z));			
		//rb.AddForce(projectedCameraTransform.TransformDirection(inputDir) * speedMultiplier * (sprintMultiplier * speedMultiplier / moveSpeed), ForceMode.VelocityChange);
		

		rb.velocity = Vector3.MoveTowards
		(
			rb.velocity,
			projectedCameraTransform.TransformDirection(inputDir) * speedMultiplier * (sprintMultiplier * speedMultiplier / moveSpeed),
			Time.fixedDeltaTime * 30f
		);

		if (rb.velocity.sqrMagnitude > Mathf.Epsilon)
		{				
			transform.LookAt(transform.position + new Vector3 (rb.velocity.x, 0f, rb.velocity.z), Vector3.up);
		}
		anim.SetFloat("MoveVertical", rb.velocity.magnitude, .1f, Time.fixedDeltaTime);
		anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 1f);
	}	
}