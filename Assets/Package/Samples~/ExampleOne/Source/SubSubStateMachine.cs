using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Samples.ExampleOne.Source
{
	[NodeColor(NodeColor.Violet)]
	public class SubSubStateMachine : SubStateMachine
	{
		[Transition]
		public event Action QuitEarly;
		
		[SerializeField] private float _duration;
		
		private float _time;

		public override void EnterState()
		{
			_time = Time.time;
			base.EnterState();
		}

		public override void UpdateState()
		{
			base.UpdateState();
			
			if(Time.time - _time > _duration)
			{
				QuitEarly?.Invoke();
			}
		}
	}
}