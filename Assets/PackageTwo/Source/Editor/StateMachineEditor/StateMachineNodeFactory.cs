using System;
using System.Reflection;
using StateMachine;
using StateMachine.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	public class StateMachineNodeFactory
	{
		public static void CreateStateNode(StateNode stateNode, GraphView graphView)
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

			graphView.AddElement(node);
		}
		
		public static void CreateInputPort(Node node)
		{
			var inputPort = node.InstantiatePort(Orientation.Horizontal, 
				Direction.Input, 
				Port.Capacity.Multi,
				typeof(Node));
			inputPort.name = inputPort.portName = "Enter";
			node.inputContainer.Add(inputPort);
		}
		
		public static void CreateOutputPorts(StateNodeView node)
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
		
		public static void CreateOutputPort(StateNodeView node, string portName)
		{
			var outputPort = node.InstantiatePort(Orientation.Horizontal, 
				Direction.Output, 
				Port.Capacity.Single,
				typeof(Node));
			outputPort.name = outputPort.portName = portName;
			node.outputContainer.Add(outputPort);
		}
		
		public static void ConnectStateNode(StateNode stateNode, GraphView graphView)
		{
			var nodeView = graphView.Q<StateNodeView>(stateNode.Id);

			foreach (var connection in stateNode.Connections)
			{
				var connectedNodeView = graphView.Q<StateNodeView>(connection.ToNodeId);
				var outputPort = nodeView.outputContainer.Q<Port>(connection.FromPortName);
				var inputPort = connectedNodeView.inputContainer.Q<Port>(connection.ToPortName);
				
				var edge = new Edge
				{
					input = inputPort,
					output = outputPort
				};
				
				inputPort.Connect(edge);
				outputPort.Connect(edge);
				
				graphView.Add(edge);
			}
		}
	}
}