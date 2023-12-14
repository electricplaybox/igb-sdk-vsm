using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing.Test
{
	public class TestState : State
	{
		[Transition] 
		public event Action OnNewThings;
		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}

		public override void DestroyState()
		{
			Debug.Log($"This {this.GetType().Name} Destroyed");
		}
	}
}