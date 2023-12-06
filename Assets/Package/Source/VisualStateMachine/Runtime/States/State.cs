using System;
using UnityEngine;

namespace VisualStateMachine.States
{
	public abstract class State : ScriptableObject
	{
		[NonSerialized]
		protected StateMachineController Controller;
		
		public abstract void InitializeState();
		public abstract void EnterState();
		public abstract void UpdateState();
		public abstract void ExitState();

		public virtual void FixedUpdateState() { }

		public State Clone(StateMachineController stateMachineController)
		{
			var state = Instantiate(this);
			state.Controller = stateMachineController;

			return state;
		}
	}
}