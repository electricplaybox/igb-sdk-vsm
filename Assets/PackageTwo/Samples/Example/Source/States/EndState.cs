using System;
using StateMachine;
using StateMachine.Attributes;
using UnityEngine;

namespace Example.States
{
	public class EndState : State
	{
		[Transition]
		public event Action Continue;
		
		private float _duration = 3;
		private float _time;
		
		public override void Enter()
		{
			_time = _duration;
		}

		public override void Update()
		{
			_time -= Time.deltaTime;
			
			if(_time <= 0) Continue?.Invoke();
		}

		public override void Exit()
		{
			
		}
	}
}