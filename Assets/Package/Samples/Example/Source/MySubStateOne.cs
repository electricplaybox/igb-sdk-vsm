using System;
using UnityEngine;
using VisualStateMachine;

namespace Package.Samples.Example.Source
{
	public class MySubStateOne : State
	{
		[Transition]
		public event Action Complete;
		
		[SerializeField] private float _duration = 1f;
		
		private float _entryTime;
		
		public override void Enter()
		{
			_entryTime = Time.time;
		}

		public override void Update()
		{
			if (Time.time - _entryTime > _duration)
			{
				Complete?.Invoke();
			}
		}

		public override void Exit()
		{
			
		}
	}
}