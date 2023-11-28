using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Editor.Manipulators;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Windows;
using VisualStateMachine.Tools;

namespace VisualStateMachine.Editor
{
	public class StateMachineGraphView : GraphView
	{
		public StateMachine StateMachine => _stateMachine;
		public GraphStateManager StateManager => _stateManager;
		
		private StateMachine _stateMachine;
		private StateMachine _lastChangedStateMachine;
		
		private readonly GraphUIManager _uiManager;
		private readonly GraphStateManager _stateManager;

		private const int WindowWidth = 600;
		private const int WindowHeight = 400;

		public StateMachineGraphView(StateMachine stateMachine = null)
		{
			_uiManager = new GraphUIManager(this);
			_stateManager = new GraphStateManager(this);
			
			graphViewChanged -= OnGraphViewChanged;
			
			CreateEmptyGraphView();
			
			if (stateMachine != null)
			{
				_stateMachine = stateMachine;
				_stateManager.LoadStateMachine(stateMachine);
			}
			
			graphViewChanged += OnGraphViewChanged;
		}


		public void Update(StateMachine stateMachine)
		{
			// DevLog.Log($"StateMachineGraphView.Update({stateMachine})");
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
	
		private void CreateEmptyGraphView()
		{
			AddToClassList("stretch-to-parent-size");
			RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			CreateGrid();
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			AddManipulators();
			_uiManager.CreateToolbar();
			_uiManager.CreateContextMenu();
		}
		
		private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			if (_stateMachine == null) return graphViewChange;
			
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
			if (_stateMachine == null) return;
			
			StateSelectorWindow.Open(_stateMachine, position, stateType =>
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
			_stateMachine.AddNode(stateNode);
			
			if (isEntryNode)
			{
				_stateMachine.SetEntryNode(stateNode);
			}

			return node;
		}

		private void OnDestroy()
		{
			_stateManager.SaveGraphViewState();
		}

		private void CreateGrid()
		{
			var grid = new GridBackground();
			grid.name = "grid";
			grid.AddToClassList("stretch-to-parent-size");
			Insert(0, grid);
		}
		
		private void AddManipulators()
		{
			var contentDragger = new StateMachineGraphContentDragger();
			contentDragger.OnDrag += HandleGraphDragged;
			
			this.AddManipulator(contentDragger);
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());
		}

		private void HandleGraphDragged(Vector3 position)
		{
			_stateManager.SaveGraphViewState();
		}

		public Rect GetGraphRect()
		{
			var rect = new Rect();
			rect.width = float.IsNaN(resolvedStyle.width) ? WindowWidth : resolvedStyle.width;
			rect.height = float.IsNaN(resolvedStyle.height) ? WindowHeight : resolvedStyle.height;
			
			return rect;
		}
		
		private void LoadStateMachine(StateMachine stateMachine)
		{
			if (stateMachine != _stateMachine)
			{
				stateMachine?.Save();
			}
			
			var nodeCount = stateMachine.Nodes.Count;
			if (nodeCount == 0)
			{
				DevLog.Log($"StateMachineGraphView.LoadStateMachine AddEntryNode");
				stateMachine.AddEntryNode();
			}
			
			if (stateMachine == _stateMachine && nodeCount == nodes.Count()) return;
			DevLog.Log($"StateMachineGraphView.LoadStateMachine({stateMachine})");
			_stateMachine = stateMachine;
			
			_stateManager.LoadGraphViewState();
			
			_uiManager.UpdateToolbar();

			_stateManager.ClearGraph();
			
			foreach (var node in _stateMachine.Nodes)
			{
				_stateManager.AddNode(node);
			}
			
			foreach (var node in _stateMachine.Nodes)
			{
				StateMachineNodeFactory.ConnectStateNode(node, this);
			}
		}

		public void SetStateMachine(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			_uiManager.UpdateToolbar();
		}
	}
}