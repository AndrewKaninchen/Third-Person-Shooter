﻿using System.Collections;
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
		
		if (drawSightLine)
		{
			//Gizmos.color = Color.blue;
			//Gizmos.DrawLine(barrelEnd.position, barrelEnd.forward * 50f);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(barrelEnd.position, target.position);
		}
	}
}
