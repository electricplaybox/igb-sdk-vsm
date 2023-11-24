using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleOne.Source
{
	[NodeColor(NodeColor.Teal)]
	public class BarState : State
	{
		[Transition]
		public event Action ExitOne;

		[Transition] 
		public event Action ExitTwo;

		[SerializeField] 
		private float _duration;

		private float _remainingTime;
		
		public override void EnterState()
		{
			//Debug.Log($"EnterState {this.GetType().Name}, {this.GetInstanceID()}");
			_remainingTime = _duration;
		}

		public override void UpdateState()
		{
			//Debug.Log($"UpdateState {this.GetType().Name}, {this.GetInstanceID()} - {_remainingTime}/{_duration}");
			if (_remainingTime > 0)
			{
				_remainingTime -= Time.deltaTime;
			}
			else
			{
				ExitOne?.Invoke();
			}
		}

		public override void ExitState()
		{
			//Debug.Log($"ExitState {this.GetType().Name}, {this.GetInstanceID()}");
		}
	}
}