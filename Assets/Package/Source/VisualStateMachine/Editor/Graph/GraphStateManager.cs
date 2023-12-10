using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.Editor.Windows;

namespace VisualStateMachine.Editor
{
	public class GraphStateManager
	{
		public  StateMachine StateMachine => _stateMachine;
		
		private StateMachineGraphView _graphView;
		private StateMachine _stateMachine;

		public GraphStateManager(StateMachineGraphView graphView) 
		{
			_graphView = graphView;
		}

		public void LoadStateMachine(StateMachine newStateMachine)
		{
			if (newStateMachine != _stateMachine)
			{
				newStateMachine.Save();
			}
			
			var nodeCount = newStateMachine.Nodes.Count;
			if (nodeCount == 0)
			{
				newStateMachine.AddEntryNode();
			}
			
			if (newStateMachine == _stateMachine && nodeCount == _graphView.nodes.Count()) return;
			
			SetStateMachine(newStateMachine);
			LoadGraphViewState();
			ClearGraph();

			var nodes = newStateMachine.Nodes.ToList();
			foreach (var node in nodes)
			{
				if (node.State == null)
				{
					newStateMachine.RemoveNode(node);
					return;
				}
				
				AddNode(node);
			}
			
			foreach (var node in newStateMachine.Nodes)
			{
				StateMachineNodeFactory.ConnectStateNode(node, _graphView);
			}
		}

		public void SaveGraphViewState()
		{
			if (_stateMachine == null) return;

			var contentContainer = _graphView.contentViewContainer;
			var position = contentContainer.transform.position;
			if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z)) return;
			
			_stateMachine.UpdateGraphViewState(contentContainer.transform.position, _graphView.scale);
		}

		public void UpdateNodes(StateMachine stateMachine) 
		{
			foreach (var node in _graphView.nodes)
			{
				if (node is not NodeView nodeView) continue;

				nodeView.Update(stateMachine);
			}
		}
		
		public NodeView AddNode(StateNode stateNode)
		{
			var stateNodeType = stateNode.State.GetType();
			var nodeType = AttributeUtils.GetInheritedCustomAttribute<NodeTypeAttribute>(stateNodeType);
			var type = nodeType?.NodeType ?? NodeType.None;
			
			switch (type)
			{
				case NodeType.Relay:
					return StateMachineNodeFactory.CreateNode<RelayNodeView>(stateNode, _graphView);
				case NodeType.Jump:
					return StateMachineNodeFactory.CreateNode<JumpNodeView>(stateNode, _graphView);
				case NodeType.None:
				default:
					return StateMachineNodeFactory.CreateNode<StateNodeView>(stateNode, _graphView);
			}
		}
		
		public void LoadGraphViewState()
		{
			if (_stateMachine == null) return;
			
			var contentContainer = _graphView.contentViewContainer.transform;

			if (_stateMachine.GraphViewState.Scale < 0.01f)
			{
				var center = GraphUtils.GraphRect(_graphView).center;
				center.x -= 90;
				center.y -= 43;

				contentContainer.position = center;
				contentContainer.scale = Vector3.one;
			}
			else
			{
				contentContainer.position = _stateMachine.GraphViewState.Position;
				contentContainer.scale = Vector3.one * _stateMachine.GraphViewState.Scale;
			}
		}
		
		public void CreateNewStateNodeFromContextMenu(Vector2 position)
		{
			if (_stateMachine == null) return;
			
			StateSelectorWindow.Open(_stateMachine, position, stateType =>
			{
				if (stateType == null) return;

				var graphPoint = GraphUtils.ScreenPointToGraphPoint(position, _graphView);
				StateMachineNodeFactory.CreateStateNode(stateType, graphPoint, _graphView);
			});
		}
		
		public void EnforceEntryNode()
		{
			if (_stateMachine.Nodes.Count > 0) return;

			_stateMachine.AddEntryNode();
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
				StateMachineGraphView.AddConnectionToState(edge);
			}
				
			_stateMachine.Save();
		}
		
		public void MoveNodes(List<GraphElement> nodesToMove)
		{
			if (nodesToMove == null) return;
			
			foreach (var node in nodesToMove)
			{
				MoveNode(node);
			}
				
			_stateMachine.Save();
		}

		public void RemoveEdges(List<GraphElement> edgesToRemove)
		{
			if (edgesToRemove == null) return;
			
			foreach (var edge in edgesToRemove)
			{
				RemoveEdge(edge);
			}
				
			_stateMachine.Save();
		}

		public void RemoveNodes(List<GraphElement> nodesToRemove)
		{
			if (nodesToRemove == null) return;
			
			foreach (var node in nodesToRemove)
			{
				RemoveNode(node);
			}
				
			_stateMachine.Save();
		}

		public void SetStateMachine(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			_graphView.UIManager.UpdateToolbar();
		}

		private void MoveNode(GraphElement element)
		{
			if (element is not NodeView) return;
			
			var nodeView = element as NodeView;
			var position = element.GetPosition().position;
			nodeView.Data.SetPosition(position);
		}

		private void RemoveNode(GraphElement element)
		{
			if (element is not NodeView) return;
			
			var nodeView = element as NodeView;
			_stateMachine.RemoveNode(nodeView.Data);
		}

		private void RemoveEdge(GraphElement element)
		{
			if (element is not Edge && !element.GetType().IsSubclassOf(typeof(Edge))) return;
			
			var edge = element as Edge;
			_stateMachine.RemoveConnection(edge.output.node.name, edge.input.node.name);
		}

		public void Save()
		{
			_stateMachine?.Save();
		}
	}
}