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

		public override void AwakeState()
		{
			Debug.Log("AWAKE");
		}

		public override void StartState()
		{
			Debug.Log("START");
		}
		
		public override void EnterState()
		{
			Debug.Log("ENTER");
			OnNewThings?.Invoke();
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