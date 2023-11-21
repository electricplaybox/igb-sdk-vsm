using UnityEngine;
using VisualStateMachine.States;

namespace Samples.Example
{
	public class MySubStateTwo : State
	{
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
				Controller.Complete();
			}
		}

		public override void ExitState()
		{
			
		}
	}
}