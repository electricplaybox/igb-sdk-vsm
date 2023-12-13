using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class RandomTwoState : RandomState
	{
		[Transition("1", NodeColor.Red)] public event Action OptionOne;
		[Transition("2", NodeColor.Green)] public event Action OptionTwo;
		
		public RandomTwoState()
		{
			AddTransition(() => OptionOne?.Invoke());
			AddTransition(() => OptionTwo?.Invoke());
		}
	}
}