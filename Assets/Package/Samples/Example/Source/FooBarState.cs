﻿using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.Example
{
	public class FooBarState : State
	{
		[Transition]
		public event Action ExitOne;

		[Transition] 
		public event Action ExitTwo;
		
		private float _remainingTime;
		private float _duration;
		
		public override void Enter()
		{
			_duration = Controller.GetComponent<Duration>().DurationTime;
			_remainingTime = _duration;
		}

		public override void Update()
		{
			if (_remainingTime > 0)
			{
				_remainingTime -= Time.deltaTime;
			}
			else
			{
				ExitOne?.Invoke();
			}
		}

		public override void Exit()
		{
			
		}
	}
}