using UnityEngine;
using Vsm.States;

namespace Samples.VSMExample.Source.States
{
	public class EndState : State
	{
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