using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : MonoBehaviour {

	#region Fields
	private Rigidbody rb;
	#endregion

	private Vector3 inputDir;
	public Transform camTransform;
	public float moveSpeed = 5f;


	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		inputDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		if (inputDir.sqrMagnitude > 0f)
		{
			rb.velocity = Vector3.Lerp
			(
				rb.velocity,
				camTransform.TransformDirection(inputDir) * moveSpeed,
				.1f
			);
			transform.LookAt(transform.position + rb.velocity, Vector3.up);
		}
		else
		{
			rb.velocity = Vector3.Lerp
			(
				rb.velocity,
				rb.velocity = Vector3.zero,
				.1f
			);			
		}		
	}

	private void FixedUpdate()
	{
		
	}
}
