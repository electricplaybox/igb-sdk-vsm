using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.Example
{
	public class StateThree : State
	{
		[Transition]
		public event Action Complete;

		[SerializeField] private float _things;
		
		public override void EnterState()
		{
			Complete?.Invoke();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}