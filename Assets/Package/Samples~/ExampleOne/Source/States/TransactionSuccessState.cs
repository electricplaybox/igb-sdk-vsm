using System;
using System.Collections.Generic;
using VisualStateMachine.Attributes;

namespace ExampleOne.States
{
	public class TransactionSuccessState : MenuState
	{
		[Transition("Continue")] 
		public event Action OnContinue;
		
		protected override Dictionary<string, Action> MapActions()
		{
			return new Dictionary<string, Action>
			{
				{ "Continue", () => OnContinue?.Invoke() }
			};
		}
	}
}