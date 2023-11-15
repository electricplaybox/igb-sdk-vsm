using System;

namespace Package.Source.Runtime.StateMachine
{
	[Serializable]
	public class StateConnection
	{
		public string FromPortName;
		public string ToPortName;
		
		public string FromNodeId;
		public string ToNodeId;
		
		public event Action<StateConnection> OnTransition;
		
		public void Transition()
		{
			OnTransition?.Invoke(this);
		}
	}
}