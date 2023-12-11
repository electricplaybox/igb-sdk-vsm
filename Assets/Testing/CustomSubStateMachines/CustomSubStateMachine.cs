using System;
using VisualStateMachine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing
{
	public class CustomSubStateMachine : BaseSubStateMachine
	{
		[Transition] 
		public event Action OutcomeOne;
		
		[Transition] 
		public event Action OutcomeTwo;
		
		[Transition] 
		public event Action OutcomeThree;
		
		protected override void SubStateMachineComplete(StateMachineCore core, State finalState)
		{
			switch (finalState)
			{
				case OutcomeOne outcomeOne:
					OutcomeOne?.Invoke();
					break;
				case OutcomeTwo outcomeTwo:
					OutcomeTwo?.Invoke();
					break;
				case OutcomeThree outcomeThree:
					OutcomeThree?.Invoke();
					break;
			}
		}
	}
}