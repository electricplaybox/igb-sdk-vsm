using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class PlaceholderState : State
	{
		[Transition] 
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