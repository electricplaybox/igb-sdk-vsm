using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;
using Random = UnityEngine.Random;

namespace Samples.Example
{
	public class RandomOptionPickerState : State
	{
		[Transition]
		public event Action OptionOne;

		[Transition] 
		public event Action OptionTwo;
		
		public override void Enter()
		{
			if (Random.value >= 0.5f)
			{
				OptionOne.Invoke();
			}
			else
			{
				OptionTwo.Invoke();
			}
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			
		}
	}
}