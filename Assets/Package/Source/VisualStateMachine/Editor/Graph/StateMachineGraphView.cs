using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.Editor.Windows;

namespace VisualStateMachine.Editor
{
	public class StateMachineGraphView : GraphView
	{
		public GraphStateManager StateManager => _stateManager;
		public GraphUIManager UIManager => _uiManager;
		
		private readonly GraphUIManager _uiManager;
		private readonly GraphStateManager _stateManager;
		private StateMachine _lastChangedStateMachine;
		
		public StateMachineGraphView(StateMachine stateMachine = null)
		{
			graphViewChanged -= OnGraphViewChanged;
			
			_stateManager = new GraphStateManager(this);
			_uiManager = new GraphUIManager(this);
			_uiManager.CreateEmptyGraphView();
			
			if (stateMachine != null)
			{
				_stateManager.LoadStateMachine(stateMachine);
			}

			graphViewChanged += OnGraphViewChanged;
		}
		
		public void Update(StateMachine stateMachine)
		{
			graphViewChanged -= OnGraphViewChanged;

			if (stateMachine == null)
			{
				_stateManager.ClearGraph();
			} 
			else
			{
				_stateManager.LoadStateMachine(stateMachine);
				_stateManager.UpdateNodes(stateMachine);
				_stateManager.EnforceEntryNode();
			}
			
			graphViewChanged += OnGraphViewChanged;
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			foreach (var port in ports)
			{
				if (ElementUtils.BothContainClass(port, startPort, "output")) continue;
				if (ElementUtils.BothContainClass(port, startPort, "input")) continue;

				if (startPort == port || startPort.node == port.node) continue;
				compatiblePorts.Add(port);
			}
			
			return compatiblePorts;
		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			_stateManager.CreateEdges(graphViewChange.edgesToCreate);
			_stateManager.RemoveNodes(graphViewChange.elementsToRemove);
			_stateManager.RemoveEdges(graphViewChange.elementsToRemove);
			_stateManager.MoveNodes(graphViewChange.movedElements);
			_stateManager.SaveGraphViewState();
			
			return graphViewChange;
		}
		
		public static void AddConnectionToState(Edge edge)
		{
			var sourceNode = edge.output.node as NodeView;
			var targetNode = edge.input.node as NodeView;

			var connection = new StateConnection(
				fromNodeId: sourceNode.Data.Id,
				fromPortName: edge.output.name,
				toNodeId: targetNode.Data.Id
			);
					
			sourceNode.Data.AddConnection(connection);
		}
		
		public void CreateNewStateNodeFromOutputPort(Port port, Vector2 position)
		{
			if (_stateManager.StateMachine == null) return;
			
			StateSelectorWindow.Open(_stateManager.StateMachine, position, stateType =>
			{
				if (stateType == null) return;

				var point = GraphUtils.ScreenPointToGraphPoint(position, this);
				var newNode = StateMachineNodeFactory.CreateStateNode(stateType, point, this);
				var edge = StateMachineNodeFactory.ConnectStateNode(port, newNode, this);
				if (edge == null) return;
				
				AddConnectionToState(edge);
				
				_stateManager.Save();
			});
		}
		
		public void OnDestroy()
		{
			_stateManager.SaveGraphViewState();
		}
		
		public void HandleGraphDragged(Vector3 position)
		{
			_stateManager.SaveGraphViewState();
		}

		public void Save()
		{
			
		}
	}
}