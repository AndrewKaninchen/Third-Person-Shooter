using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

	public Transform target;
	public Transform barrelEnd;
	public bool drawSightLine;

	void LateUpdate () {
		transform.LookAt(target, Vector3.up);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		if(drawSightLine)
			Gizmos.DrawLine(barrelEnd.position, barrelEnd.forward*100f);
	}
}
