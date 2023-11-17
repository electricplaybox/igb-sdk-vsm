using System;
using UnityEngine;
using VisualStateMachine;

namespace Example
{
	public class BarState : State
	{
		[Transition]
		public event Action ExitOne;

		[Transition] 
		public event Action ExitTwo;

		[SerializeField] 
		private float _duration;

		private float _remainingTime;
		
		public override void Enter()
		{
			//Debug.Log($"Enter {this.GetType().Name}, {this.GetInstanceID()}");
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