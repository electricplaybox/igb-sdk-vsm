using System;
using UnityEngine;
using Vsm.Attributes;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class EntryState : State
	{
		[Transition] public event Action Exit;
		[Transition] public event Action Continue;
		
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