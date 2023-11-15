using System;
using UnityEngine;

namespace StateMachine
{
	public abstract class State : ScriptableObject
	{
		[NonSerialized]
		protected StateMachineController Controller;
		
		public abstract void Enter();
		public abstract void Update();
		public abstract void Exit();

		public State Clone(StateMachineController stateMachineController)
		{
			var state = Instantiate(this);
			state.Controller = stateMachineController;

			return state;
		}
	}
}