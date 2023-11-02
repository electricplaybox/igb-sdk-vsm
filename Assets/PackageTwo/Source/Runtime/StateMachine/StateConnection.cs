using System;

namespace StateMachine
{
	[Serializable]
	public class StateConnection
	{
		public string FromEventName;
		public string FromNodeId;
		public string ToNodeId;
		
		public event Action<StateConnection> OnTransition;
		
		public void Transition()
		{
			OnTransition?.Invoke(this);
		}
	}
}