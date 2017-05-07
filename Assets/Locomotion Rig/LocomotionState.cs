using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : MonoBehaviour {

	#region Components
	private Rigidbody rb;
	private Animator anim;
	#endregion

	#region Fields

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
		if (inputDir.sqrMagnitude > 0f)
		{
			inputDir.Normalize();
			rb.velocity = Vector3.Lerp
			(
				rb.velocity,
				camTransform.TransformDirection(inputDir) * moveSpeed * ((isSprinting)?2f:1f),
				.5f //* Time.deltaTime
			);
			transform.LookAt(transform.position + rb.velocity, Vector3.up);
		}
		else
		{
			rb.velocity = Vector3.Lerp
			(
				rb.velocity,
				rb.velocity = Vector3.zero,
				.25f //* Time.deltaTime
			);			
		}

		anim.SetFloat("MoveVertical", rb.velocity.magnitude);
		anim.SetLayerWeight(anim.GetLayerIndex("Arms"), 1f);
	}

	private void FixedUpdate()
	{
		
	}
}
