using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Vsm.Editor.Nodes;
using Vsm.Serialization;

namespace Vsm.Editor.Graph
{
	public class VsmDataManager
	{
		private readonly GraphView _graphView;
		private VsmGraphData _graphData;

		public VsmDataManager(GraphView graphView)
		{
			_graphView = graphView;
		}

		public event Action<StateNodeData> OnCreateNode;
		public event Action<EdgeData> OnConnectPorts;
		public event Action OnClearGraph;
		public event Action<VsmGraphData> OnSaved;

		public void LoadData(VsmGraphData graphData)
		{
			_graphData = graphData;
			LoadData();
		}

		public void LoadData()
		{
			OnClearGraph?.Invoke();

			if (_graphData == null) return;
			if (_graphData.nodes.Count == 0) return;

			foreach (var node in _graphData.nodes) OnCreateNode.Invoke(node);
			foreach (var edge in _graphData.edges) OnConnectPorts.Invoke(edge);
		}

		public void SaveData()
		{
			if (_graphData == null) return;

			var assetPath = AssetDatabase.GetAssetPath(_graphData);
			var graphData = ScriptableObject.CreateInstance<VsmGraphData>();
			var nodes = _graphView.nodes;
			var edges = _graphView.edges;

			foreach (var node in nodes.ToList())
			{
				if (node is not StateNode) continue;
				var stateNode = node as StateNode;

				var nodeData = new StateNodeData
				{
					Title = stateNode.title,
					Position = stateNode.GetPosition().position,
					State = stateNode.State.GetType().AssemblyQualifiedName,
					GUID = stateNode.GUID
				};

				graphData.nodes.Add(nodeData);
			}

			foreach (var edge in edges.ToList())
			{
				var inputNode = edge.input.node as BaseNode;
				var inputPort = edge.input;

				var outputNode = edge.output.node as BaseNode;
				var outputPort = edge.output;

				var edgeData = new EdgeData
				{
					OutputNode = outputNode?.GUID,
					OutputPort = outputPort?.portName,
					InputNode = inputNode?.GUID,
					InputPort = inputPort?.portName
				};

				graphData.edges.Add(edgeData);
			}

			_graphData = graphData;
			AssetDatabase.CreateAsset(graphData, assetPath);

			OnSaved.Invoke(_graphData);
		}
	}
}