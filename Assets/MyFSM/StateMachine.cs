using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFSM
{	
	public class StateMachine : MonoBehaviour
	{
		public State[] states;		
		public State startingState;
		//private State currentState;

		public void Start()
		{
			//currentState = startingState;
		}

		private void Update()
		{

		}

	}
}