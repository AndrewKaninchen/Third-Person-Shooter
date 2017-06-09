using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : StateMachineBehaviour {

	LocomotionState locomotion;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (locomotion == null) locomotion = animator.GetComponent<LocomotionState>();
		animator.SetFloat("ArmsLayerWeight", 1f);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetLayerWeight(animator.GetLayerIndex("Arms"), animator.GetFloat("ArmsLayerWeight"));
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetFloat("ArmsLayerWeight", 1f);
		animator.SetLayerWeight(animator.GetLayerIndex("Arms"), 1f);
		locomotion.StopRolling();
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
