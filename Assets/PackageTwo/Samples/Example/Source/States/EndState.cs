using System;
using StateMachine;
using StateMachine.Attributes;

namespace Example.States
{
	public class EndState : State
	{
		[Transition]
		public event Action Continue;
		
		public override void Enter()
		{
			
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			
		}
	}
}