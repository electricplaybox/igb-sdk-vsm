using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor
{
	public class StateMachineGraphView : GraphView
	{
		private StateMachine _stateMachine;
		private StateMachineToolbar _toolbar;
		private StateMachineContextMenu _contextMenu;

		public StateMachineGraphView()
		{
			CreateEmptyGraphView();
			LoadGraphViewState();
		}
		
		public StateMachineGraphView(StateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			
			CreateEmptyGraphView();
			LoadStateMachine(stateMachine);
		}
		
		public void Update(StateMachine stateMachine)
		{
			LoadStateMachine(stateMachine);
			UpdateNodes();
			ClearNullStateMachine();
			EnforceEntryNode();
			// MessWithEdges();
		}

		private void MessWithEdges()
		{
			foreach (var edge in edges)
			{
				var originalPoints = edge.edgeControl.controlPoints;
				originalPoints[1] = originalPoints[0];
				originalPoints[2] = originalPoints[3];
				edge.MarkDirtyRepaint();
			}
		}

		private void EnforceEntryNode()
		{
			if (_stateMachine.Nodes.Count > 0) return;

			_stateMachine.AddEntryNode();
		}

		public void ClearNullStateMachine()
		{
			if (_stateMachine != null) return;
			
			contentViewContainer.Clear();
			
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			ports.ForEach(port =>
			{
				if (startPort != port && startPort.node != port.node)
				{
					compatiblePorts.Add(port);
				}
			});
			
			return compatiblePorts;
		}
		
		private void SaveGraphViewState()
		{
			if (_stateMachine == null) return;

			var position = contentViewContainer.transform.position;
			if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z)) return;
			
			_stateMachine.UpdateGraphViewState(contentViewContainer.transform.position, scale);
		}
		
		private void LoadGraphViewState()
		{
			if (_stateMachine == null) return;
			
			if (_stateMachine.GraphViewState != null && _stateMachine.GraphViewState.Scale > 0.1f)
			{
				contentViewContainer.transform.position = Vector3.zero;
				contentViewContainer.transform.scale = Vector3.one * _stateMachine.GraphViewState.Scale;
			}
			else
			{
				var center = GetRect().center;
				center.x -= 90;
				center.y -= 43;
				
				contentViewContainer.transform.scale = Vector3.one;
				contentViewContainer.transform.position = center;
			}
		}

		private void UpdateNodes()
		{
			if (_stateMachine == null) return;
			
			foreach (var node in nodes)
			{
				if (node is not StateNodeView stateNodeView) continue;

				stateNodeView.Update();
			}
		}

		private void CreateToolbar(StateMachine stateMachine)
		{
			_toolbar = new StateMachineToolbar(stateMachine);
			Add(_toolbar);
		}

		private void CreateEmptyGraphView()
		{
			AddToClassList("stretch-to-parent-size");
			RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			CreateGrid();
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			AddManipulators();
			CreateToolbar(_stateMachine);
			CreateContextMenu();
			
			graphViewChanged += OnGraphViewChanged;
		}
		
		private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			if (_stateMachine == null) return graphViewChange;
			
			if (graphViewChange.edgesToCreate != null)
			{
				foreach (var edge in graphViewChange.edgesToCreate)
				{
					AddConnectionToState(edge);
				}
				
				_stateMachine.Save();
			}
			
			if (graphViewChange.elementsToRemove != null)
			{
				foreach (var element in graphViewChange.elementsToRemove)
				{
					if (element is StateNodeView)
					{
						var stateNodeView = element as StateNodeView;
						// RemoveAllEdgesTo(stateNodeView);
						_stateMachine.RemoveNode(stateNodeView.Data);
					}
					else if (element.GetType().IsSubclassOf(typeof(Edge)))
					{
						var edge = element as Edge;
						_stateMachine.RemoveConnection(edge.output.node.name, edge.input.node.name);
					}
				}
			}
			
			if (graphViewChange.movedElements != null)
			{
				foreach (var element in graphViewChange.movedElements)
				{
					if (element is StateNodeView)
					{
						var stateNodeView = element as StateNodeView;
						var position = element.GetPosition().position;
						stateNodeView.Data.SetPosition(position);
					}
				}
			}
			
			return graphViewChange;
		}

		private void RemoveAllEdgesTo(StateNodeView stateNodeView)
		{
			foreach (var node in nodes)
			{
				if (node is not StateNodeView stateNode) continue;
				
				stateNode.Data.RemoveConnectionToNode(stateNodeView.Data.Id);
			}
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
		}

		private void CreateContextMenu()
		{
			_contextMenu = new StateMachineContextMenu(this);
			_contextMenu.OnCreateNewStateNode += HandleCreateNewStateNode;
			_contextMenu.OnSetAsEntryNode += HandleSetAsEntryNode;
		}

		private void HandleSetAsEntryNode(StateNodeView node)
		{
			_stateMachine.SetEntryNode(node.Data);
		}

		private void HandleCreateNewStateNode(Vector2 position)
		{
			StateSelectorWindow.Open(stateType =>
			{
				if (stateType == null) return;
				
				CreateStateNode(stateType, ScreenPointToGraphPoint(position));
			});
		}

		public Vector3 ScreenPointToGraphPoint(Vector2 screenPoint)
		{
			return (Vector3)screenPoint - contentViewContainer.transform.position;
		}
		
		public void CreateNewStateNodeFromOutputPort(Port port, Vector2 position)
		{
			StateSelectorWindow.Open(stateType =>
			{
				if (stateType == null) return;
				
				var newNode = CreateStateNode(stateType, ScreenPointToGraphPoint(position));
				var edge = StateMachineNodeFactory.ConnectStateNode(port, newNode, this);

				//TODO save edge to statemachine
				AddConnectionToState(edge);
			});
		}

		private StateNodeView CreateStateNode(Type stateType, Vector2 position)
		{
			var stateNode = new StateNode(stateType);
			stateNode.SetPosition(position);
			
			var isEntryNode = this.nodes.ToList().Count == 0;
			
			var node = StateMachineNodeFactory.CreateStateNode(stateNode, this);
			_stateMachine.AddNode(stateNode);
			
			if (isEntryNode)
			{
				_stateMachine.SetEntryNode(stateNode);
			}

			return node;
		}

		private void OnDestroy()
		{
			SaveGraphViewState();
				
			if (_contextMenu != null)
			{
				_contextMenu.OnCreateNewStateNode -= HandleCreateNewStateNode;
				_contextMenu.OnSetAsEntryNode -= HandleSetAsEntryNode;
			}
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
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());
			
		}

		private Rect GetRect()
		{
			return new Rect()
			{
				width = resolvedStyle.width,
				height = resolvedStyle.height
			};
		}
		
		private void LoadStateMachine(StateMachine stateMachine)
		{
			if (stateMachine == _stateMachine && stateMachine.Nodes.Count == nodes.Count()) return;
			if (_stateMachine != null) SaveGraphViewState();
			
			_stateMachine = stateMachine;
			LoadGraphViewState();
			
			_toolbar?.Update(stateMachine);
			
			DeleteElements(nodes);
			DeleteElements(edges);
			
			foreach (var node in stateMachine.Nodes)
			{
				StateMachineNodeFactory.CreateStateNode(node, this);
			}
			
			foreach (var node in stateMachine.Nodes)
			{
				StateMachineNodeFactory.ConnectStateNode(node, this);
			}
		}
	}
}