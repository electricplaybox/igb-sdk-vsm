using System;
using UnityEngine;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class MyThingyState : State
	{
		[Transition] public event Action GoSomeWhere;
		[Transition] public event Action DoSomething;
		
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