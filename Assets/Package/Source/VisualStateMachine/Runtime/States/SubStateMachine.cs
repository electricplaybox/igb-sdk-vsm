using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeWidth(350)]
	public class SubStateMachine : BaseSubStateMachine
	{
		[Transition("Complete")]
		public event Action OnComplete;

		protected override void SubStateMachineComplete(StateMachineCore core, State finalState)
		{
			OnComplete?.Invoke();
		}
	}
}