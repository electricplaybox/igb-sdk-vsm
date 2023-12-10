using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class RandomTwoState : RandomState
	{
		[Transition("1")] public event Action OptionOne;
		[Transition("2")] public event Action OptionTwo;
		
		public RandomTwoState()
		{
			AddTransition(() => OptionOne?.Invoke());
			AddTransition(() => OptionTwo?.Invoke());
		}
	}
}