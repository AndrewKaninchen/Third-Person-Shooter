using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour {

	public Transform target;

	private void Start()
	{
		if(target == null)
			target = GetComponentInChildren<Transform>();
	}

	private void Update()
	{
		RaycastHit hit;
		Ray ray = new Ray(transform.position, transform.forward);

		if (Physics.Raycast(ray, out hit, 100f, -1))
		{
			Debug.DrawLine(transform.position, hit.point, Color.red);
			target.position = hit.point;
		}
		else
		{
			target.localPosition = Vector3.forward * 100f;
		}
	}

}
