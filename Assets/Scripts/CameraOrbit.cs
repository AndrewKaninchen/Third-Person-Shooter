using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	#region Fields
	public float distanceFromTarget;
	public Vector2 speed;
	public Transform target;

	private float tetha, phi;	
	private Vector3 currentPosition = new Vector3();
	#endregion

	#region Properties
	public float AngleXZ { get { return phi; } }
	public float AngleYZ { get { return tetha; } }
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

		#region Read Input in Spherical Coordinates		
		phi = Mathf.Repeat(phi + Input.GetAxis("Mouse X") * Time.deltaTime * speed.x, Mathf.PI * 2);
		tetha = Mathf.Clamp(tetha + Input.GetAxis("Mouse Y") * Time.deltaTime * speed.y, (Mathf.PI) - (Mathf.PI * .9f), Mathf.PI * .6f);
		#endregion

		UpdatePosition();
	}

	private void OnEnable()
	{
		ResetRotation();
	}

	private void ResetRotation()
	{
		currentPosition = target.position + new Vector3(0f, 0f, -distanceFromTarget);
		currentPosition = target.TransformPoint(currentPosition);

		#region Transform from Cartesian to Spherical Space		
		//https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates
		//Axis are a little different in Unity: x = x(U), y = z(U), z = y(U), where n(U) is Unity's 'n' axis when compared to regular mathematical axis
		tetha = Mathf.Acos(currentPosition.y / distanceFromTarget); 
		phi =	Mathf.Atan(currentPosition.z / currentPosition.x);
		#endregion

		UpdatePosition();
	}

	private void UpdatePosition()
	{
		#region Transform from Spherical to Cartesian Space
		//https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates
		//Axis are a little different in Unity: x = z(U), y = x(U), z = y(U), where n(U) is Unity's 'n' axis when compared to regular mathematical axis
		currentPosition.x = distanceFromTarget * Mathf.Sin(tetha) * Mathf.Sin(phi);
		currentPosition.z = distanceFromTarget * Mathf.Sin(tetha) * Mathf.Cos(phi);
		currentPosition.y = distanceFromTarget * Mathf.Cos(tetha);
		#endregion

		#region Set the Transform
		transform.position = currentPosition + target.position;
		transform.LookAt(target);
		#endregion
	}
}