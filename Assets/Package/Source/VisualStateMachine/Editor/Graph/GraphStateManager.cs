using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.Editor.Windows;
using VisualStateMachine.Tools;

namespace VisualStateMachine.Editor
{
	public class GraphStateManager
	{
		private StateMachineGraphView _graphView;
		
		public GraphStateManager(StateMachineGraphView graphView) 
		{
			_graphView = graphView;
		}

		public void LoadStateMachine(StateMachine stateMachine)
		{
			if (stateMachine != _graphView.StateMachine)
			{
				stateMachine.Save();
			}
			
			var nodeCount = stateMachine.Nodes.Count;
			if (nodeCount == 0)
			{
				stateMachine.AddEntryNode();
			}
			
			if (stateMachine == _graphView.StateMachine && nodeCount == _graphView.nodes.Count()) return;
			
			_graphView.SetStateMachine(stateMachine);
			LoadGraphViewState();
			ClearGraph();

			foreach (var node in stateMachine.Nodes)
			{
				AddNode(node);
			}
			
			foreach (var node in stateMachine.Nodes)
			{
				StateMachineNodeFactory.ConnectStateNode(node, _graphView);
			}
		}

		public void SaveGraphViewState()
		{
			if (_graphView.StateMachine == null) return;

			var contentContainer = _graphView.contentViewContainer;
			var position = contentContainer.transform.position;
			if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z)) return;
			
			_graphView.StateMachine.UpdateGraphViewState(contentContainer.transform.position, _graphView.scale);
		}

		public void UpdateNodes() 
		{
			foreach (var node in _graphView.nodes)
			{
				if (node is not StateNodeView stateNodeView) continue;

				stateNodeView.Update();
			}
		}
		
		public StateNodeView AddNode(StateNode stateNode)
		{
			var stateNodeType = stateNode.State.GetType();
			var nodeType = AttributeUtils.GetInheritedCustomAttribute<NodeTypeAttribute>(stateNodeType);
			var type = nodeType?.NodeType ?? NodeType.None;
			
			switch (type)
			{
				case NodeType.Relay:
					return StateMachineNodeFactory.CreateStateNode<RelayNodeView>(stateNode, _graphView);
				case NodeType.None:
				default:
					return StateMachineNodeFactory.CreateStateNode<StateNodeView>(stateNode, _graphView);
			}
		}
		
		public void LoadGraphViewState()
		{
			var statemachine = _graphView.StateMachine;
			if (statemachine == null) return;
			
			var contentContainer = _graphView.contentViewContainer.transform;

			if (statemachine.GraphViewState.Scale < 0.01f)
			{
				var center = _graphView.GetGraphRect().center;
				center.x -= 90;
				center.y -= 43;

				contentContainer.position = center;
				contentContainer.scale = Vector3.one;
			}
			else
			{
				contentContainer.position = statemachine.GraphViewState.Position;
				contentContainer.scale = Vector3.one * statemachine.GraphViewState.Scale;
			}
		}
		
		public void CreateNewStateNodeFromContextMenu(Vector2 position)
		{
			var statemachine = _graphView.StateMachine;
			if (statemachine == null) return;
			
			StateSelectorWindow.Open(statemachine, position, stateType =>
			{
				if (stateType == null) return;
				
				_graphView.CreateStateNode(stateType, _graphView.ScreenPointToGraphPoint(position));
			});
		}
		
		public void EnforceEntryNode()
		{
			var statemachine = _graphView.StateMachine;
			if (statemachine.Nodes.Count > 0) return;

			statemachine.AddEntryNode();
		}
		
		public void ClearGraph()
		{
			if (_graphView.contentViewContainer.childCount == 0) return;
			
			_graphView.DeleteElements(_graphView.nodes);
			_graphView.DeleteElements(_graphView.edges);
		}

		public void CreateEdges(List<Edge> edgesToCreate)
		{
			if (edgesToCreate == null) return;
			
			foreach (var edge in edgesToCreate)
			{
				_graphView.AddConnectionToState(edge);
			}
				
			_graphView.StateMachine.Save();
		}
		
		public void MoveNodes(List<GraphElement> nodesToMove)
		{
			if (nodesToMove == null) return;
			
			foreach (var node in nodesToMove)
			{
				MoveNode(node);
			}
				
			_graphView.StateMachine.Save();
		}

		public void MoveNode(GraphElement element)
		{
			if (element is not StateNodeView) return;
			
			var stateNodeView = element as StateNodeView;
			var position = element.GetPosition().position;
			stateNodeView.Data.SetPosition(position);
		}
		
		public void RemoveEdges(List<GraphElement> edgesToRemove)
		{
			if (edgesToRemove == null) return;
			
			foreach (var edge in edgesToRemove)
			{
				RemoveEdge(edge);
			}
				
			_graphView.StateMachine.Save();
		}

		public void RemoveEdge(GraphElement element)
		{
			if (element is not Edge && !element.GetType().IsSubclassOf(typeof(Edge))) return;
			
			var edge = element as Edge;
			_graphView.StateMachine.RemoveConnection(edge.output.node.name, edge.input.node.name);
		}

		public void RemoveNodes(List<GraphElement> nodesToRemove)
		{
			if (nodesToRemove == null) return;
			
			foreach (var node in nodesToRemove)
			{
				RemoveNode(node);
			}
				
			_graphView.StateMachine.Save();
		}
		
		public void RemoveNode(GraphElement element)
		{
			if (element is not StateNodeView) return;
			
			var stateNodeView = element as StateNodeView;
			_graphView.StateMachine.RemoveNode(stateNodeView.Data);
		}
	}
}