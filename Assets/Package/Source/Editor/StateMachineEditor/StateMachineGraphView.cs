using System;
using System.Collections.Generic;
using Package.Source.Runtime.StateMachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Package.Source.Editor.StateMachineEditor
{
	public class StateMachineGraphView : GraphView
	{
		private StateMachineToolbar _toolbar;
		//private StateMachineContextMenu _contextMenu;
		
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
		
		private void HandleSetAsEntryNode(StateNodeView node)
		{
			var currentEntryNode = this.Q<StateNodeView>(_graph.EntryNodeId);
			if(currentEntryNode != null) currentEntryNode.Data.EntryPoint = false;
			
			node.Data.EntryPoint = true;
			_graph.EntryNodeId = node.Data.Id;
		}

		private void HandleDeleteStateNode(StateNodeView node)
		{
			_graph.RemoveNode(node.Data);
			DeleteSelection();
		}

		private void HandleCreateNewStateNode(Type state, Vector2 position)
		{
			var id = Guid.NewGuid().ToString();
			var entryPoint = this.nodes.ToList().Count == 0;
			var stateNode = new StateNode(id, state, entryPoint, position);
			
			StateMachineNodeFactory.CreateStateNode(stateNode, this);
		}
		
		private void OnDestroy()
		{
			_toolbar.OnSave -= HandleSaveGraph;
			_toolbar.OnGraphChanged -= HandleGraphChanged;
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
			if (graph == null) return;
			graph.Load();
			
			foreach (var kvp in graph.Nodes)
			{
				StateMachineNodeFactory.CreateStateNode(kvp.Value, this);
			}
			
			foreach (var kvp in graph.Nodes)
			{
				StateMachineNodeFactory.ConnectStateNode(kvp.Value, this);
			}
		}

		private void HandleSaveGraph()
		{
			if (Application.isPlaying) return;
			if (_graph == null) return;
			
			StateMachineReadWrite.SaveGraph(_graph, this);
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