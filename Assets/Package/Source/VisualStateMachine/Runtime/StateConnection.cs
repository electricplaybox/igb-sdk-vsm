using System;
using UnityEngine;
using VisualStateMachine.States;

namespace VisualStateMachine
{
	[Serializable]
	public class StateConnection
	{
		public event Action<StateConnection> OnTransition;
		
		public string ToNodeId => _toNodeId;
		public string FromNodeId => _fromNodeId;
		public string FromPortName => _fromPortName;
		public PortData PortData => _portData;
		
		[SerializeField]
		private string _toNodeId;
		
		[SerializeField]
		private string _fromNodeId;
		
		[SerializeField]
		private string _fromPortName;
		
		[SerializeField]
		private PortData _portData;

		public StateConnection(string fromNodeId, string fromPortName, string toNodeId, PortData portData)
		{
			_fromNodeId = fromNodeId;
			_fromPortName = fromPortName;
			_toNodeId = toNodeId;
			_portData = portData;
		}
		
		public void Transition()
		{
			OnTransition?.Invoke(this);
		}
	}
}