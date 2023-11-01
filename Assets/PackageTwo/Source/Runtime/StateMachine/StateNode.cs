using System;

namespace StateMachine
{
	public class StateNode
	{
		public string Id;
		public string StateType;
		public bool EntryPoint;

		private State _state;

		public void Initialize()
		{
			var type = Type.GetType(StateType);
			if (type == null) return;
			
			_state = Activator.CreateInstance(type) as State;
		}

		public void Enter()
		{
			_state.Enter();
		}
		
		public void Update()
		{
			_state.Update();
		}
		
		public void Exit()
		{
			_state.Exit();
		}
	}
}