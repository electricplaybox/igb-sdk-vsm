using System;
using UnityEngine;

namespace StateMachine
{
	[Serializable]
	public class StateConnection
	{
		public event Action<StateConnection> OnTransition;
		
		public string ToNodeId => _toNodeId;
		public string FromNodeId => _fromNodeId;
		public string FromPortName => _fromPortName;
		
		[SerializeField]
		private string _toNodeId;
		
		[SerializeField]
		private string _fromNodeId;
		
		[SerializeField]
		private string _fromPortName;

		public StateConnection(string fromNodeId, string fromPortName, string toNodeId)
		{
			_fromNodeId = fromNodeId;
			_fromPortName = fromPortName;
			_toNodeId = toNodeId;
		}
		
		public void Transition()
		{
			OnTransition?.Invoke(this);
		}
	}
}