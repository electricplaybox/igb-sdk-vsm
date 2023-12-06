using System;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;
using Random = UnityEngine.Random;

namespace Testing
{
	public class RandomPathState : State
	{
		[Transition]
		public event Action OptionOne;
		
		[Transition]
		public event Action OptionTwo;
		
		[Transition]
		public event Action OptionThree;

		public override void EnterState()
		{
			var options = new Action[] {OptionOne, OptionTwo, OptionThree};
			options[Random.Range(0, options.Length)]?.Invoke();
		}

		public override void ExitState()
		{
			//Do nothing
		}
	}
}