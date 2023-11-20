using System;
using UnityEngine;
using VisualStateMachine;
using Random = UnityEngine.Random;

namespace Package.Samples.Example.Source
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