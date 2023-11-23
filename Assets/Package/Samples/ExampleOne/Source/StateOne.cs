using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleOne.Source
{
	public class StateOne : State
	{
		[Transition]
		public event Action Complete;

		public int Value;
		
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