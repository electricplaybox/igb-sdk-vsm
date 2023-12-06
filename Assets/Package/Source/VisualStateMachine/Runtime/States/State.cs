using System;
using UnityEngine;

namespace VisualStateMachine.States
{
	public abstract class State : ScriptableObject
	{
		[NonSerialized] 
		protected StateMachineCore StateMachineCore;
		
		public abstract void InitializeState();
		public abstract void EnterState();
		public abstract void UpdateState();
		public abstract void ExitState();

		public virtual void FixedUpdateState() { }

		public State Clone(StateMachineCore stateMachineCore)
		{
			var state = Instantiate(this);
			state.StateMachineCore = stateMachineCore;

			return state;
		}
	}
}