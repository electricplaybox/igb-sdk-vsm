using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class RandomThreeState : RandomState
	{
		[Transition("1", NodeColor.Red)] public event Action OptionOne;
		[Transition("2", NodeColor.Green)] public event Action OptionTwo;
		[Transition("3", NodeColor.Blue)] public event Action OptionThree;
		
		public RandomThreeState()
		{
			AddTransition(() => OptionOne?.Invoke());
			AddTransition(() => OptionTwo?.Invoke());
			AddTransition(() => OptionThree?.Invoke());
		}
	}
}