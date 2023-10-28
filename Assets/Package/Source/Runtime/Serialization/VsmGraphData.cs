using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Vsm.Serialization
{
	[CreateAssetMenu(fileName = "VsmGraphData", menuName = "StateMachine/VsmGraphData")]
	public class VsmGraphData : ScriptableObject
	{
		[FormerlySerializedAs("nodes")] public List<StateNodeData> Nodes = new();
		[FormerlySerializedAs("edges")] public List<EdgeData> Edges = new();

		public StateNodeData CurrentState;
	}
}