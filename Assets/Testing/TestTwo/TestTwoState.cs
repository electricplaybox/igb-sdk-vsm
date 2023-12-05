using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing.TestTwo
{
	[NodeLabel("Test Two")]
	public class TestTwoState : State
	{
		[Transition("Foo")]
		public event Action OnFoo;
		
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