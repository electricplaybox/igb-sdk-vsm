using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.Example
{
	public class StateOne : State
	{
		[Transition]
		public event Action Complete;

		public int Value;
		
		public override void Enter()
		{
			Complete?.Invoke();
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			
		}
	}
}