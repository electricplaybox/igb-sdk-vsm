using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Windows;
using VisualStateMachine.Tools;

namespace VisualStateMachine.Editor
{
	public class StateMachineGraphView : GraphView
	{
		public GraphStateManager StateManager => _stateManager;
		public GraphUIManager UIManager => _uiManager;
		private StateMachine _lastChangedStateMachine;
		
		private readonly GraphUIManager _uiManager;
		private readonly GraphStateManager _stateManager;

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
				_stateManager.UpdateNodes();
				_stateManager.EnforceEntryNode();
			}
			
			graphViewChanged += OnGraphViewChanged;
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			foreach (var port in ports)
			{
				if (DoElementsBothContainClass(port, startPort, "output")) continue;
				if (DoElementsBothContainClass(port, startPort, "input")) continue;
				
				if (startPort != port && startPort.node != port.node)
				{
					compatiblePorts.Add(port);
				}
			}
			
			return compatiblePorts;
		}

		private bool DoElementsBothContainClass(VisualElement elementA, VisualElement elementB, string className)
		{
			var elementAHasClass = elementA.GetClasses().Contains(className);
			var elementBHasClass = elementB.GetClasses().Contains(className);
			return elementAHasClass == elementBHasClass;
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

		public void AddConnectionToState(Edge edge)
		{
			var sourceNode = edge.output.node as StateNodeView;
			var targetNode = edge.input.node as StateNodeView;

			var connection = new StateConnection(
				fromNodeId: sourceNode.Data.Id,
				fromPortName: edge.output.name,
				toNodeId: targetNode.Data.Id
			);
					
			sourceNode.Data.AddConnection(connection);
			DevLog.Log("AddConnectionToState");
		}

		public Vector3 ScreenPointToGraphPoint(Vector2 screenPoint)
		{
			return (Vector3)screenPoint - contentViewContainer.transform.position;
		}
		
		public void CreateNewStateNodeFromOutputPort(Port port, Vector2 position)
		{
			if (_stateManager.StateMachine == null) return;
			
			StateSelectorWindow.Open(_stateManager.StateMachine, position, stateType =>
			{
				if (stateType == null) return;
				
				var newNode = CreateStateNode(stateType, ScreenPointToGraphPoint(position));
				var edge = StateMachineNodeFactory.ConnectStateNode(port, newNode, this);

				AddConnectionToState(edge);
			});
		}

		public StateNodeView CreateStateNode(Type stateType, Vector2 position)
		{
			var stateNode = new StateNode(stateType);
			stateNode.SetPosition(position);
			
			var isEntryNode = this.nodes.ToList().Count == 0;

			var node = _stateManager.AddNode(stateNode);
			_stateManager.StateMachine.AddNode(stateNode);
			
			if (isEntryNode)
			{
				_stateManager.StateMachine.SetEntryNode(stateNode);
			}

			return node;
		}

		public void OnDestroy()
		{
			_stateManager.SaveGraphViewState();
		}
		
		public void HandleGraphDragged(Vector3 position)
		{
			_stateManager.SaveGraphViewState();
		}

		public Rect GetGraphRect()
		{
			var rect = new Rect();
			rect.width = float.IsNaN(resolvedStyle.width) ? StateMachineWindow.WindowWidth : resolvedStyle.width;
			rect.height = float.IsNaN(resolvedStyle.height) ? StateMachineWindow.WindowHeight : resolvedStyle.height;
			
			return rect;
		}
	}
}