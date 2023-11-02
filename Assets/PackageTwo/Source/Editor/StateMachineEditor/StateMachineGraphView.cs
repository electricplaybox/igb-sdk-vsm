using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StateMachine;
using StateMachine.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	public class StateMachineGraphView : GraphView
	{
		private StateMachineToolbar _toolbar;
		private StateMachineContextMenu _contextMenu;
		
		private StateMachineGraph _graph;

		public StateMachineGraphView(StateMachineGraph graph)
		{
			_graph = graph;
			
			this.AddToClassList("stretch-to-parent-size");
			this.RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			CreateGrid();
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			AddManipulators();
			CreateToolbar(graph);
			CreateContextMenu();
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
		
		public void CreateInputPort(Node node)
		{
			var inputPort = node.InstantiatePort(Orientation.Horizontal, 
				Direction.Input, 
				Port.Capacity.Multi,
				typeof(Node));
			inputPort.portName = "Enter";
			node.inputContainer.Add(inputPort);
		}
		
		public void CreateOutputPorts(StateNodeView node)
		{
			var type = Type.GetType(node.Data.StateType);
			if (type == null) return;

			foreach (var eventInfo in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
			{
				if (eventInfo.EventHandlerType != typeof(Action)) continue;
				
				var attributes = eventInfo.GetCustomAttributes(typeof(Transition), false);
				if (attributes.Length > 0) CreateOutputPort(node, eventInfo.Name);
			}
		}
		
		public void CreateOutputPort(StateNodeView node, string portName)
		{
			var outputPort = node.InstantiatePort(Orientation.Horizontal, 
				Direction.Output, 
				Port.Capacity.Single,
				typeof(Node));
			outputPort.portName = portName;
			node.outputContainer.Add(outputPort);
		}

		private void CreateContextMenu()
		{
			_contextMenu = new StateMachineContextMenu(this);
			_contextMenu.OnCreateNewStateNode += OnCreateNewStateNode;
		}

		private void OnCreateNewStateNode(Type state, Vector2 position)
		{
			Debug.Log($"Create Node: {state.Name}");
			
			var node = new StateNodeView();
			node.Data = new StateNode()
			{
				Id = Guid.NewGuid().ToString(),
				StateType = state.AssemblyQualifiedName,
				EntryPoint = this.nodes.ToList().Count == 0
			};
			node.title = state.Name;
			
			CreateInputPort(node);
			CreateOutputPorts(node);
			
			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(position, Vector2.one));
			
			AddElement(node);
		}

		private void OnDestroy()
		{
			_toolbar.OnSave -= HandleSaveGraph;
			_toolbar.OnGraphChanged -= HandleGraphChanged;
			_contextMenu.OnCreateNewStateNode -= OnCreateNewStateNode;
		}

		private void CreateToolbar(StateMachineGraph graph)
		{
			_toolbar = new StateMachineToolbar(graph);
			_toolbar.OnSave += HandleSaveGraph;
			_toolbar.OnGraphChanged += HandleGraphChanged;
			
			Add(_toolbar);
		}

		private void HandleGraphChanged(StateMachineGraph graph)
		{
			Debug.Log("Graph Changed");
		}

		private void HandleSaveGraph()
		{
			if (Application.isPlaying) return;
			if (_graph == null) return;

			_graph.Clear();
			
			foreach (var node in nodes)
			{
				if (node is not StateNodeView) continue;
				var stateNodeView = node as StateNodeView;

				var edges = this.edges.Where(edge => edge.output.node == node);
				foreach (var edge in edges)
				{
					var connection = new StateConnection()
					{
						FromEventName = edge.output.portName,
						FromNodeId = stateNodeView.Data.Id,
						ToNodeId = (edge.output.connections.First().input.node as StateNodeView).Data.Id
					};
						
					stateNodeView.Data.AddConnection(connection);
				}
				
				_graph.AddNode(stateNodeView.Data);
			}
			
			EditorUtility.SetDirty(_graph);
			AssetDatabase.SaveAssets();
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
	}
}