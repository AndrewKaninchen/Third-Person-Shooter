using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : MonoBehaviour {

	#region Components
	private Rigidbody rb;
	private Animator anim;
	#endregion

	#region Fields
	private float sprintMultiplier;
	private float speedMultiplier;
	private Vector3 velocity = Vector3.zero;
	#endregion

	private Vector3 inputDir;
	public Transform camTransform;
	public float moveSpeed = 5f;


	void Start () {
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
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
		rb.velocity = Vector3.Lerp
		(
			rb.velocity,
			camTransform.TransformDirection(inputDir) * speedMultiplier * (sprintMultiplier * speedMultiplier / moveSpeed),
			.5f
		);

		//rb.velocity = Vector3.SmoothDamp
		//(
		//	rb.velocity,
		//	camTransform.TransformDirection(inputDir) * speedMultiplier * (sprintMultiplier * speedMultiplier / moveSpeed),
		//	ref velocity,
		//	.1f
		//);
		if (rb.velocity.sqrMagnitude > Mathf.Epsilon)
		{			
			transform.LookAt(transform.position + rb.velocity, Vector3.up);
		}
		anim.SetFloat("MoveVertical", rb.velocity.magnitude, .1f, Time.fixedDeltaTime);
		anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 1f);
	}
}
