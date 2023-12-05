using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class SubStateMachine : BaseSubStateMachine
	{
		[Transition]
		public event Action OnComplete;
		
		public override void UpdateState()
		{
			//Do nothing
		}

		protected override void HandleComplete()
		{
			OnComplete?.Invoke();
		}
	}
}