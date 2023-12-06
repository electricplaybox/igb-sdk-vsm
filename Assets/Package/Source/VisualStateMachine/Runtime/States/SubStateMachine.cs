using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class SubStateMachine : BaseSubStateMachine
	{
		[Transition("Complete")]
		public event Action OnComplete;

		protected override void SubStateMachineComplete(State finalState)
		{
			OnComplete?.Invoke();
		}
	}
}