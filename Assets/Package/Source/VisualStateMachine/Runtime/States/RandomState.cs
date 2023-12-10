using System;
using System.Collections.Generic;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeIcon(NodeIcon.VsmFlatRandomWhite, opacity:0.3f)]
	public abstract class RandomState : State
	{
		private readonly System.Random _random;
		private List<Action> _transitions = new ();

		public RandomState()
		{
			_random = new System.Random();
		}
		
		public void AddTransition(Action transition)
		{
			_transitions.Add(transition);
		}
		
		public override void EnterState()
		{
			if (_transitions.Count == 0)
			{
				throw new StateMachineException("RandomState has now transitions.");
			}
			
			var index = _random.Next(0, _transitions.Count);
			_transitions[index]?.Invoke();
		}
		
		public override void ExitState()
		{
			//..
		}
	}
}