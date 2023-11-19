using System;
using UnityEngine;
using VisualStateMachine;

namespace Package.Samples.Example.Source
{
	public class StateThree : State
	{
		[Transition]
		public event Action Complete;

		[SerializeField] private float _things;
		
		public override void Enter()
		{
			Complete?.Invoke();
		}

		public override void Update()
		{
			
		}

		public override void Exit()
		{
			
		}
	}
}