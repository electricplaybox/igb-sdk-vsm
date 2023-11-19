using System;
using VisualStateMachine;

namespace Samples.Example.Source
{
	public class StateOne : State
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