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
		public event Action<VsmGraphData> OnLoaded;

		public void LoadData(VsmGraphData graphData)
		{
			_graphData = graphData;
			
			LoadData();
		}

		public void LoadData()
		{
			OnClearGraph?.Invoke();

			if (_graphData == null) return;
			if (_graphData.Nodes.Count == 0) return;

			foreach (var node in _graphData.Nodes) OnCreateNode.Invoke(node);
			foreach (var edge in _graphData.Edges) OnConnectPorts.Invoke(edge);
			
			OnLoaded?.Invoke(_graphData);
		}

		public void SaveData()
		{
			if (_graphData == null) return;

			var nodes = _graphView.nodes.ToList();
			var edges = _graphView.edges.ToList();
			
			_graphData.Nodes.Clear();
			_graphData.Edges.Clear();

			foreach (var node in nodes)
			{
				if (node is not StateNode) continue;
				var stateNode = node as StateNode;

				var nodeData = new StateNodeData
				{
					Title = stateNode.title,
					Position = stateNode.GetPosition().position,
					State = stateNode.State.GetType().AssemblyQualifiedName,
					Guid = stateNode.Guid,
					EntryPoint = stateNode.EntryPoint
				};

				_graphData.Nodes.Add(nodeData);
			}

			foreach (var edge in edges)
			{
				var inputNode = edge.input.node as BaseNode;
				var inputPort = edge.input;

				var outputNode = edge.output.node as BaseNode;
				var outputPort = edge.output;

				var edgeData = new EdgeData
				{
					OutputNode = outputNode?.Guid,
					OutputPort = outputPort?.portName,
					InputNode = inputNode?.Guid,
					InputPort = inputPort?.portName
				};

				_graphData.Edges.Add(edgeData);
			}
			
			EditorUtility.SetDirty(_graphData);
			AssetDatabase.SaveAssets();

			OnSaved.Invoke(_graphData);
		}
	}
}