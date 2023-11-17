﻿using System;
using UnityEngine;
using VisualStateMachine;

namespace Example
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
			//Debug.Log($"Enter {this.GetType().Name}, {this.GetInstanceID()}");
			_duration = Controller.GetComponent<Duration>().DurationTime;
			_remainingTime = _duration;
		}

		public override void Update()
		{
			//Debug.Log($"Update {this.GetType().Name}, {this.GetInstanceID()} - {_remainingTime}/{_duration}");
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
			//Debug.Log($"Exit {this.GetType().Name}, {this.GetInstanceID()}");
		}
	}
}