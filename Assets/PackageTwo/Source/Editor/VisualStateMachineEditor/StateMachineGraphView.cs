using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine;

namespace Editor.VisualStateMachineEditor
{
	public class StateMachineGraphView : GraphView
	{
		private StateMachine _stateMachine;
		private StateMachineToolbar _toolbar;
		private StateMachineContextMenu _contextMenu;

		public StateMachineGraphView()
		{
			CreateEmptyGraphView();
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
		}

		private void ClearNullStateMachine()
		{
			if (_stateMachine != null) return;
			
			Clear();
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			ports.ForEach(port =>
			{
				if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
			});
			
			return compatiblePorts;
		}

		private void UpdateNodes()
		{
			if (_stateMachine == null) return;
			
			foreach (var node in nodes)
			{
				if (node is not StateNodeView stateNodeView) continue;

				stateNodeView?.Update();
			}
		}

		private void CreateToolbar(StateMachine stateMachine)
		{
			_toolbar = new StateMachineToolbar(stateMachine);
			_toolbar.OnSave += HandleSaveStateMachine;
			_toolbar.OnStateMachineChanged += HandleStateMachineChanged;
			
			Add(_toolbar);
		}

		private void HandleStateMachineChanged(StateMachine stateMachine)
		{
			//SOMETHING CHANGED
			Debug.Log("Statemachine changed");
		}

		private void HandleSaveStateMachine()
		{
			if (Application.isPlaying) return;
			if (_stateMachine == null) return;

			SaveStateMachine(_stateMachine);
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
					var sourceNode = edge.output.node as StateNodeView;
					var targetNode = edge.input.node as StateNodeView;

					var connection = new StateConnection(
						fromNodeId: sourceNode.Data.Id,
						fromPortName: edge.output.portName,
						toNodeId: targetNode.Data.Id
					);
					
					sourceNode.Data.AddConnection(connection);
				}
			}
			
			if (graphViewChange.elementsToRemove != null)
			{
				foreach (var element in graphViewChange.elementsToRemove)
				{
					if (element is StateNodeView)
					{
						var stateNodeView = element as StateNodeView;
						_stateMachine.RemoveNode(stateNodeView.Data);
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

		private void HandleDeleteStateNode(StateNodeView node)
		{
			Remove(node);
		}

		private void HandleCreateNewStateNode(Type type, Vector2 position)
		{
			var stateNode = new StateNode(type, _stateMachine);
			stateNode.SetPosition(position);

			var isEntryNode = this.nodes.ToList().Count == 0;
			
			StateMachineNodeFactory.CreateStateNode(stateNode, this);
			_stateMachine.AddNode(stateNode);

			if (isEntryNode)
			{
				_stateMachine.SetEntryNode(stateNode);
			}
		}

		private void OnDestroy()
		{
			if (_toolbar != null)
			{
				_toolbar.OnSave -= HandleSaveStateMachine;
				_toolbar.OnStateMachineChanged -= HandleStateMachineChanged;
			}

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
		
		private void LoadStateMachine(StateMachine stateMachine)
		{
			if (stateMachine == _stateMachine && stateMachine.Nodes.Count == nodes.Count()) return;
			
			_stateMachine = stateMachine;
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

		private void SaveStateMachine(StateMachine stateMachine)
		{
			stateMachine.RemoveAllNodes();
			
			foreach (var node in this.nodes)
			{
				if (node is not StateNodeView) continue;
				
				var stateNodeView = node as StateNodeView;
				stateNodeView.Data.SetPosition(node.GetPosition().position);
		
				// if (stateNodeView.Data.EntryPoint)
				// {
				// 	graph.EntryNodeId = stateNodeView.Data.Id;
				// }
				
				var edges = this.edges.Where(edge => edge.output.node == node);
				foreach (var edge in edges)
				{
					var connection = new StateConnection(
						fromNodeId: stateNodeView.Data.Id,
						fromPortName: edge.output.portName,
						toNodeId: (edge.output.connections.First().input.node as StateNodeView).Data.Id
					);
					
					stateNodeView.Data.AddConnection(connection);
				}
				
				stateMachine.AddNode(stateNodeView.Data);
			}
		}
	}
}