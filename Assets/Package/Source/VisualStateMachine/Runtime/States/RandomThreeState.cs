using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class RandomThreeState : RandomState
	{
		[Transition("1")] public event Action OptionOne;
		[Transition("2")] public event Action OptionTwo;
		[Transition("3")] public event Action OptionThree;
		
		public RandomThreeState()
		{
			AddTransition(() => OptionOne?.Invoke());
			AddTransition(() => OptionTwo?.Invoke());
			AddTransition(() => OptionThree?.Invoke());
		}
	}
}