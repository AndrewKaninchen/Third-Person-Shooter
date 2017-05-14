using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
	#region Fields
	public float distanceFromTarget;
	public Vector2 speed;
	public Transform target;

	private float angleXY, angleYZ;	
	private Vector3 localPosition = new Vector3();
	#endregion

	#region Properties
	public Vector3 CurrentPosition { get { return localPosition; } }
	public float AngleXZ { get { return angleYZ; } }
	public float AngleYZ { get { return angleXY; } }
	public float DistanceFromTarget { get { return distanceFromTarget; } }
	#endregion

	private void OnEnable()
	{
		transform.position = target.TransformPoint(new Vector3(0f, 0f, -distanceFromTarget));
		transform.LookAt(target);		
	}

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
		angleYZ = Mathf.Repeat(angleYZ + Input.GetAxis("Mouse X") * Time.deltaTime * speed.x, Mathf.PI * 2);
		angleXY = Mathf.Clamp(angleXY + Input.GetAxis("Mouse Y") * Time.deltaTime * speed.y, (Mathf.PI) - (Mathf.PI * .9f), Mathf.PI * .6f);
		#endregion

		SetPositionFromSphericalCoordinates();
	}

	public void SetPositionFromSphericalCoordinates()
	{
		//https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates
		//Axis are a little different in Unity: x = z(U), y = x(U), z = y(U), where n(U) is Unity's 'n' axis when compared to regular mathematical axis
		localPosition.x = distanceFromTarget * Mathf.Sin(angleXY) * Mathf.Sin(angleYZ);
		localPosition.z = distanceFromTarget * Mathf.Sin(angleXY) * Mathf.Cos(angleYZ);
		localPosition.y = distanceFromTarget * Mathf.Cos(angleXY);
		
		UpdatePosition();
	}

	public void SetPositionFromSphericalCoordinates(float distanceFromTarget, float angleXY, float angleYZ)
	{
		this.angleXY = angleXY;
		this.angleYZ = angleYZ;
		this.distanceFromTarget = distanceFromTarget;

		SetPositionFromSphericalCoordinates();
	}

	public void SetPositionFromCartesianCoordinates(Vector3 position)
	{
		this.localPosition = position - target.position;

		//https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates
		//Axis are a little different in Unity: x = x(U), y = z(U), z = y(U), where n(U) is Unity's 'n' axis when compared to regular mathematical axis
		distanceFromTarget = Mathf.Sqrt(Mathf.Pow(localPosition.x, 2) + Mathf.Pow(localPosition.y, 2) + Mathf.Pow(localPosition.z, 2));
		angleXY = Mathf.Acos(localPosition.y / distanceFromTarget);
		angleYZ = Mathf.Atan(localPosition.z / localPosition.x);
		
		UpdatePosition();
	}
	
	private void UpdatePosition()
	{
		transform.position = target.position + localPosition;
		transform.LookAt(target);
	}
}