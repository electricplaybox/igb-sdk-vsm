using System;
using UnityEngine;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class SomeOtherState : State
	{
		[Transition] public event Action Foo;
		
		public override void OnEnter()
		{
			Debug.Log($"OnEnter {GetType().Name}");
		}

		public override void OnUpdate()
		{
			Debug.Log($"OnUpdate {GetType().Name}");
		}

		public override void OnExit()
		{
			Debug.Log($"OnExit {GetType().Name}");
		}
	}
}