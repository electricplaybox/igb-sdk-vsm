using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StateMachine;
using StateMachine.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
			
			AddToClassList("stretch-to-parent-size");
			RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			CreateGrid();
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			AddManipulators();
			CreateToolbar(graph);
			CreateContextMenu();
			LoadGraph(graph);
		}
		
		public void Update()
		{
			if (_graph == null) return;

			foreach (var node in nodes)
			{
				if (node is not StateNodeView) continue;
			
				var stateNodeView = node as StateNodeView;
				stateNodeView.Update();
			}
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
			inputPort.name = inputPort.portName = "Enter";
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
			outputPort.name = outputPort.portName = portName;
			node.outputContainer.Add(outputPort);
		}

		private void CreateContextMenu()
		{
			_contextMenu = new StateMachineContextMenu(this);
			_contextMenu.OnCreateNewStateNode += OnCreateNewStateNode;
		}

		private void OnCreateNewStateNode(Type state, Vector2 position)
		{
			var id = Guid.NewGuid().ToString();
			
			var stateNode = new StateNode()
			{
				Id = id,
				StateType = state.AssemblyQualifiedName,
				EntryPoint = this.nodes.ToList().Count == 0,
				Position = position,
				Title = state.Name
			};
			
			CreateStateNode(stateNode);
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

		private void LoadGraph(StateMachineGraph graph)
		{
			foreach (var kvp in graph.Nodes)
			{
				CreateStateNode(kvp.Value);
			}

			foreach (var kvp in graph.Nodes)
			{
				ConnectStateNode(kvp.Value);
			}
		}

		private void ConnectStateNode(StateNode stateNode)
		{
			var nodeView = this.Q<StateNodeView>(stateNode.Id);

			foreach (var connection in stateNode.Connections)
			{
				var connectedNodeView = this.Q<StateNodeView>(connection.ToNodeId);
				var outputPort = nodeView.outputContainer.Q<Port>(connection.FromPortName);
				var inputPort = connectedNodeView.inputContainer.Q<Port>(connection.ToPortName);
				
				var edge = new Edge
				{
					input = inputPort,
					output = outputPort
				};
				
				inputPort.Connect(edge);
				outputPort.Connect(edge);
				
				Add(edge);
			}
		}
		
		private void CreateStateNode(StateNode stateNode)
		{
			var node = new StateNodeView();
			node.Data = stateNode;
			node.title = stateNode.Title;
			node.name = stateNode.Id;
			
			CreateInputPort(node);
			CreateOutputPorts(node);
			
			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(stateNode.Position, Vector2.one));
			
			var container = new VisualElement();
			container.name = "title-container";
			container.style.alignItems = Align.Center;
			container.style.marginTop = 6;
			container.style.marginBottom = 6;
			
			var title = node.Query<VisualElement>("title").First();
			var titleLabel = title.Query<VisualElement>("title-label").First();
			var titleButton = title.Query<VisualElement>("title-button-container").First();
			
			title.Add(container);
			container.Add(titleLabel);
			container.Add(titleButton);

			var progressBar = new ProgressBar();
			progressBar.name = "progress-bar";
			progressBar.styleSheets.Add(Resources.Load<StyleSheet>("ProgressBar"));
			title.Add(progressBar);
			
			AddElement(node);
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
				stateNodeView.Data.Position = node.GetPosition().position;

				if (stateNodeView.Data.EntryPoint)
				{
					_graph.EntryNodeId = stateNodeView.Data.Id;
				}
				
				var edges = this.edges.Where(edge => edge.output.node == node);
				foreach (var edge in edges)
				{
					var connection = new StateConnection()
					{
						FromPortName = edge.output.portName,
						ToPortName = edge.input.portName,
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