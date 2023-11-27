using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleOne.Source
{
	public class StateThree : State
	{
		[Transition]
		public event Action Complete;
		
		[Transition]
		public event Action Foo;
		
		[Transition]
		public event Action Bar;
		
		[Transition]
		public event Action Hum;

		[Transition]
		public event Action Bug;
		
		[SerializeField] private float _things;
		[SerializeField] private float _foo;
		[SerializeField] private float _bar;
		[SerializeField] private float _hum;
		[SerializeField] private float _bug;
		
		public override void EnterState()
		{
			Complete?.Invoke();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}