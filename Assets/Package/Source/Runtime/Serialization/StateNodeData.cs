using System;
using UnityEngine.Serialization;

namespace Vsm.Serialization
{
	[Serializable]
	public class StateNodeData : BaseNodeData
	{
		public string State;
		public string Guid;
		public bool EntyPoint;
	}
}