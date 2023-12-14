using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class DelayFrameState : State
	{
		[Transition("Exit", NodeColor.Green, frameDelay: 1)] 
		public event Action Exit;
		
		public override void EnterState()
		{
			Exit?.Invoke();
		}

		public override void ExitState()
		{
			
		}
	}
}