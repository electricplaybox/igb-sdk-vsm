using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleTwo.Source
{
	public class StateTwo : State
	{
		[Transition]
		public event Action Complete;
		
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