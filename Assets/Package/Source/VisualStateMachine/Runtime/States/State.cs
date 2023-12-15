using System;
using UnityEngine;

namespace VisualStateMachine.States
{
	public abstract class State : ScriptableObject
	{
		[NonSerialized] 
		protected StateMachineCore StateMachineCore;
		
		//Any code that makes sense to perform when the state is entered
		public abstract void EnterState();
		
		//Any code that makes sense to perform when the state is exited like state cleanup
		public abstract void ExitState();
		
		public virtual void AwakeState()
		{
			//Any Awake phase code that makes sense to perform before the state is entered
		}

		public virtual void StartState()
		{
			//Any Start code that makes sense to perform before the state is entered
		}
		
		public virtual void UpdateState()
		{
			//Any code that makes sense to perform when the state is updated like state logic
		}

		public virtual void FixedUpdateState()
		{
			//Any code that requires a fixed update step like physics related code
		}

		public virtual void DestroyState()
		{
			//Any clean up code that makes sense to perform when the state is destroyed
		}

		public State Clone(StateMachineCore stateMachineCore)
		{
			var state = Instantiate(this);
			state.StateMachineCore = stateMachineCore;

			return state;
		}
	}
}