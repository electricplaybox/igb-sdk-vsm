using UnityEngine;

namespace StateMachine
{
	public abstract class State : ScriptableObject
	{
		public abstract void Enter();
		public abstract void Update();
		public abstract void Exit();
	}
}