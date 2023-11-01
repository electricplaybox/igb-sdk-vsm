using System;

namespace StateMachine
{
	public class StateConnection
	{
		public string FromPortId;
		public string ToPortId;
		
		public event Action<StateConnection> OnTransition;
		
		public void Transition()
		{
			OnTransition?.Invoke(this);
		}
	}
}