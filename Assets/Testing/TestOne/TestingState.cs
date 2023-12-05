using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing
{
	public class TestingState : State
	{
		[Transition] 
		public event Action MyTransition;
		
		public override void EnterState()
		{
			
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}