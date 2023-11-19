using System;
using VisualStateMachine;

namespace Package.Samples.Example.Source
{
	public class StateTwo : State
	{
		[Transition]
		public event Action Complete;
		
		public override void Enter()
		{
			Complete?.Invoke();
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			
		}
	}
}