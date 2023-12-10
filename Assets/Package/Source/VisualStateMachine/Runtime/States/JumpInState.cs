using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NoInputPort()]
	public class JumpInState : JumpState
	{
		[Transition] public event Action Exit;
		
		public override void EnterState()
		{
			Exit?.Invoke();
		}

		public override void ExitState()
		{
			
		}
	}
}