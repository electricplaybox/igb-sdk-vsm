using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing
{
	public class ReallyLongStateNameState : State
	{
		[Transition] 
		public event Action MyTransition;

		[SerializeField] private float _foo;

		public override void InitializeState()
		{
			
		}

		public override void EnterState()
		{
			MyTransition?.Invoke();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}