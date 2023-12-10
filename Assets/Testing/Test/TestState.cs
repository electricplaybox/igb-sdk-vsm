using System;
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
	}
}