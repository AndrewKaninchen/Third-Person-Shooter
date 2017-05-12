using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	#region Fields
	public float distanceFromTarget;
	public Vector2 speed;
	public Transform target;

	private float tetha = (Mathf.PI) - (Mathf.PI * .8f), phi = Mathf.PI;
	private Vector3 currentPosition = new Vector3();
	#endregion

	private void Update()
	{
		#region Cursor Lock
		if (Input.GetKeyDown(KeyCode.K))
		{
			if (Cursor.lockState == CursorLockMode.Locked)
				Cursor.lockState = CursorLockMode.None;
			else
				Cursor.lockState = CursorLockMode.Locked;
		}
		#endregion
	}

	private void FixedUpdate()
	{
		#region Read Input in Spherical Coordinates		
		phi = Mathf.Repeat(phi + Input.GetAxis("Mouse X") * Time.deltaTime * speed.x, Mathf.PI * 2);
		tetha = Mathf.Clamp(tetha + Input.GetAxis("Mouse Y") * Time.deltaTime * speed.y, (Mathf.PI) - (Mathf.PI * .9f), Mathf.PI * .8f);
		#endregion

		#region Transform from Spherical to Cartesian Space
		currentPosition.z = distanceFromTarget * Mathf.Sin(tetha) * Mathf.Cos(phi);
		currentPosition.x = distanceFromTarget * Mathf.Sin(tetha) * Mathf.Sin(phi);
		currentPosition.y = distanceFromTarget * Mathf.Cos(tetha);
		#endregion

		#region Set the Transform
		transform.position = currentPosition + target.position;
		transform.LookAt(target);
		#endregion
	}
}