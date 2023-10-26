using System.Collections.Generic;
using UnityEngine;

namespace Vsm.Serialization
{
	[CreateAssetMenu(fileName = "VsmGraphData", menuName = "StateMachine/VsmGraphData")]
	public class VsmGraphData : ScriptableObject
	{
		public List<StateNodeData> nodes = new();
		public List<EdgeData> edges = new();
	}
}