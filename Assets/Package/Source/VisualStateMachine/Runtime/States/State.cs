using System;
using UnityEngine;

namespace VisualStateMachine.States
{
	public abstract class State : ScriptableObject
	{
		[NonSerialized] 
		protected StateMachineCore StateMachineCore;
		
		public virtual void InitializeState()
		{
			//Any initialization code that makes sense to perform before the state is entered
		}

		//Any code that makes sense to perform when the state is entered
		public abstract void EnterState();
		
		public virtual void UpdateState()
		{
			//Any code that makes sense to perform when the state is updated like state logic
		}
		
		//Any code that makes sense to perform when the state is exited like state cleanup
		public abstract void ExitState();

		public virtual void FixedUpdateState()
		{
			//Any code that requires a fixed update step like physics related code
		}

		public State Clone(StateMachineCore stateMachineCore)
		{
			var state = Instantiate(this);
			state.StateMachineCore = stateMachineCore;

			return state;
		}
	}
}