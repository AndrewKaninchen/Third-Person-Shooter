using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class DrawLine : MonoBehaviour {
	public Transform barrelEnd;
	public Transform target;

	private void OnDrawGizmos()
	{
		if (barrelEnd != null && target != null)
			Debug.DrawLine(barrelEnd.position, target.position, Color.green);
		else if (barrelEnd != null)
			Debug.DrawRay(barrelEnd.position, barrelEnd.forward, Color.blue);
	}
}
