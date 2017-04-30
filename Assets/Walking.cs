using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Walking : MonoBehaviour {
	public Animator anim;
	
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	
	void Update () {
		anim.SetFloat("MoveHorizontal", Input.GetAxis("Horizontal"));
		anim.SetFloat("MoveVertical", Input.GetAxis("Vertical"));
	}
}
