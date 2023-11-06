using System;
using System.Collections.Generic;
using StateMachine;
using StateMachine.Attributes;
using UnityEngine;

namespace Example.States
{
	public class EntryState : State
	{
		[Transition]
		public event Action Continue;
		
		[Transition]
		public event Action Alternative;

		public float Duration = 1f;
		public string Foo = "Foo";
		public ScriptableObject Bar;
		
		
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