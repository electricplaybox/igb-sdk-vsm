using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleTwo.Source
{
	public class MySubStateOne : State
	{
		[Transition]
		public event Action Complete;
		
		[SerializeField] private float _duration = 1f;
		
		private float _entryTime;
		
		public override void EnterState()
		{
			_entryTime = Time.time;
		}

		public override void UpdateState()
		{
			if (Time.time - _entryTime > _duration)
			{
				Complete?.Invoke();
			}
		}

		public override void ExitState()
		{
			
		}
	}
}