using System;
using StateMachine;
using StateMachine.Attributes;
using UnityEngine;

namespace Example.States
{
	public class MyThingState : State
	{
		[Transition]
		public event Action Foo;
		
		[Transition]
		public event Action Bar;

		[SerializeField] 
		private int _myInt;
		
		private float _duration = 3;
		private float _time;
		
		public override void Enter()
		{
			_time = _duration;
		}

		public override void Update()
		{
			_time -= Time.deltaTime;
			
			if(_time <= 0) Bar?.Invoke();
		}

		public override void Exit()
		{
			
		}
	}
}