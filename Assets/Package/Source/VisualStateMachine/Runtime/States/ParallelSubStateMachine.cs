using System;
using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeWidth(350)]
	public class ParallelSubStateMachine : BaseParallelSubStateMachine
	{
		[Transition("All Complete")]
		public event Action OnAllComplete;

		private int _complete = 0;
		private int _target;

		public override void EnterState()
		{
			Debug.Log("Enter Parallel Sub State Machine");
			_complete = 0;
			_target = SubStateMachines.Count;
			
			base.EnterState();
		}

		protected override void SubStateMachineComplete(StateMachineCore core, State finalState)
		{
			_complete++;
			Debug.Log($"SubStateMachineComplete: {core.StateMachine.name}, {_complete}");

			if (_complete == _target)
			{
				OnAllComplete?.Invoke();
			}
		}
	}
}