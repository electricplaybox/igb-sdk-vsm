using UnityEngine;

namespace Package.Source.Runtime.StateMachine
{
	public abstract class State : ScriptableObject
	{
		public abstract void Enter();
		public abstract void Update();
		public abstract void Exit();
	}
}