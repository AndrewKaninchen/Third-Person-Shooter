using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : MonoBehaviour {

	#region Components
	private Rigidbody rb;
	private Animator anim;
	#endregion

	#region Variables
	private float sprintMultiplier;
	private float speedMultiplier;
	private Vector3 inputDir;
	#endregion

	#region Fields
	public float moveSpeed = 5f;
	#endregion
	
	#region Camera Variables
	[Header("Camera Variables")]
	public Transform cameraTransform;
	private Transform projectedCameraTransform;
	#endregion


	void Start () {
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		projectedCameraTransform = new GameObject().transform;	
	}
	
	void Update () {
		var isSprinting = Input.GetKey(KeyCode.LeftShift);
		inputDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		speedMultiplier = Mathf.Lerp(speedMultiplier, (inputDir.sqrMagnitude > Mathf.Epsilon)?moveSpeed:0f, .1f);			
		sprintMultiplier = Mathf.Lerp(sprintMultiplier, ((isSprinting) ? 2f : 1f), .1f);				
	}

	private void FixedUpdate()
	{
		inputDir.Normalize();
		projectedCameraTransform.position = new Vector3 (cameraTransform.position.x, 0f, cameraTransform.position.z);
		projectedCameraTransform.LookAt(transform);
		rb.velocity = Vector3.Lerp
		(
			rb.velocity,
			projectedCameraTransform.TransformDirection(inputDir) * speedMultiplier * (sprintMultiplier * speedMultiplier / moveSpeed),
			.5f
		);

		if (rb.velocity.sqrMagnitude > Mathf.Epsilon)
		{			
			transform.LookAt(transform.position + rb.velocity, Vector3.up);
		}
		anim.SetFloat("MoveVertical", rb.velocity.magnitude, .1f, Time.fixedDeltaTime);
		anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 1f);
	}	
}
