using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vsm.States;

namespace Vsm.Serialization
{
	[CreateAssetMenu(fileName = "VsmGraphData", menuName = "StateMachine/VsmGraphData")]
	public class VsmGraphData : ScriptableObject
	{
		[FormerlySerializedAs("nodes")] public List<StateNodeData> Nodes = new();
		[FormerlySerializedAs("edges")] public List<EdgeData> Edges = new();

		public StateNodeData CurrentState;

		public void Initialize()
		{
			foreach (var node in Nodes)
			{
				node.Initialize();
			}
		}
	}
}