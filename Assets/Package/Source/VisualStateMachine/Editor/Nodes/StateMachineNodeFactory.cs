using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Edges;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.Editor.Windows;

namespace VisualStateMachine.Editor
{
	public class StateMachineNodeFactory
	{
		public const string DefaultInputNodeName = "Enter";
		
		public static NodeView CreateStateNode(StateNode stateNode, StateMachineGraphView graphView)
		{
			var node = new StateNodeView(stateNode, graphView);
			
			graphView.AddElement(node);
			return node;
		}

		public static NodeView CreateStateNode(Type stateType, Vector2 position, StateMachineGraphView graphView)
		{
			var stateManager = graphView.StateManager;
			var stateNode = new StateNode(stateType);
			stateNode.SetPosition(position);
			
			var isEntryNode = graphView.nodes.ToList().Count == 0;
		
			var node = stateManager.AddNode(stateNode);
			stateManager.StateMachine.AddNode(stateNode);
			
			if (isEntryNode)
			{
				stateManager.StateMachine.SetEntryNode(stateNode);
			}
		
			return node;
		}
		
		public static T CreateNode<T>(StateNode stateNode, StateMachineGraphView graphView) where T : NodeView
		{
			var node = Activator.CreateInstance(typeof(T), new object[] {stateNode, graphView}) as T;
			
			graphView.AddElement(node);
			return node;
		}
		
		public static VisualElement CreateUIElementInspector(UnityEngine.Object target, 
			List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
			var serializedObject = new SerializedObject(target);
			var fields = GetInheritedSerializedFields(target.GetType());
 
			foreach (var field in fields)
			{
				if ( propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) continue;

				var serializedProperty = serializedObject.FindProperty(field.Name);
				if (serializedProperty != null)
				{
					var propertyField = new PropertyField(serializedProperty);
					container.Add(propertyField);
				}
				else
				{
					Debug.LogWarning($"Property {field.Name} not found in serialized object.");
				}
			}
			
			container.Bind(serializedObject);
			
			return container;
		}
		
		public static FieldInfo[] GetVisibleSerializedFields(Type T)
		{
			var publicFields = T.GetFields(BindingFlags.Instance | BindingFlags.Public);
			var infoFields = publicFields.Where(t => t.GetCustomAttribute<HideInInspector>() == null).ToList();
			var privateFields = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			
			infoFields.AddRange(privateFields.Where(t => t.GetCustomAttribute<SerializeField>() != null));

			return infoFields.ToArray();
		}
		
		public static IEnumerable<FieldInfo> GetInheritedSerializedFields(Type type)
		{
			var infoFields = new List<FieldInfo>();

			while (type != null && type != typeof(UnityEngine.Object))
			{
				var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				infoFields.AddRange(publicFields.Where(field => field.GetCustomAttribute<HideInInspector>() == null));
				
				var privateFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				infoFields.AddRange(privateFields.Where(field => field.GetCustomAttribute<SerializeField>() != null));

				type = type.BaseType;
			}

			return infoFields.ToArray();
		}

		public static void CreateInputPort(NodeView node, StateMachineGraphView graphView)
		{
			var nodeType = node.Data.State.GetType();
			var orientationAtt = AttributeUtils.GetInheritedCustomAttribute<PortOrientationAttribute>(nodeType);
			var orientation = orientationAtt != null ? (Orientation) orientationAtt.PortOrientation : Orientation.Horizontal;
			var inputPort = node.InstantiatePort(orientation, Direction.Input, Port.Capacity.Multi, typeof(Node));

			inputPort.name = inputPort.portName = DefaultInputNodeName;
			inputPort.AddManipulator(new EdgeConnector<StateNodeEdge>(new StateEdgeListener(graphView)));
			node.inputContainer.Add(inputPort);
		}

		public static void CreateOutputPorts(NodeView node, StateMachineGraphView graphView)
		{
			var type = node.Data.State.GetType();
			var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			
			foreach (var eventInfo in events)
			{
				if (eventInfo.EventHandlerType != typeof(Action)) continue;
				
				var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
				if (attributes.Length > 0) CreateOutputPort(node, eventInfo.Name, graphView, attributes[0] as TransitionAttribute);
			}
		}
		
		public static void CreateOutputPort(NodeView node, string portName, StateMachineGraphView graphView, 
			TransitionAttribute transitionAttribute)
		{
			var nodeType = node.Data.State.GetType();
			var orientationAtt = AttributeUtils.GetInheritedCustomAttribute<PortOrientationAttribute>(nodeType);
			var orientation = orientationAtt != null ? (Orientation)orientationAtt.PortOrientation : Orientation.Horizontal;
			
			var outputPort = node.InstantiatePort(orientation, 
				Direction.Output, 
				Port.Capacity.Single,
				typeof(Node));
			
			var portText = string.IsNullOrEmpty(transitionAttribute.PortLabel) 
				? portName 
				: transitionAttribute.PortLabel;
			
			outputPort.name = portName;
			outputPort.portName = portText;
			outputPort.AddManipulator(new EdgeConnector<StateNodeEdge>(new StateEdgeListener(graphView)));
			
			node.outputContainer.Add(outputPort);
		}

		public static Edge ConnectStateNode(Port outputPort, NodeView destinationNode, GraphView graphView)
		{
			var inputPort = destinationNode.Q<Port>(null, "port", "input");
			if (inputPort == null) return null;
			
			var edge = new StateNodeEdge()
			{
				input = inputPort,
				output = outputPort
			};

			var position = edge.GetPosition();
			position.position -= (Vector2)graphView.contentViewContainer.transform.position;
			edge.SetPosition(position);
			
			inputPort.Connect(edge);
			outputPort.Connect(edge); 
			graphView.Add(edge);

			return edge;
		}
		
		public static void ConnectStateNode(StateNode stateNode, GraphView graphView)
		{
			var nodeView = graphView.Q<NodeView>(stateNode.Id);

			var connections = stateNode.Connections.ToList();
			foreach (var connection in connections)
			{
				var connectedNodeView = graphView.Q<NodeView>(connection.ToNodeId);
				if (connectedNodeView == null) continue;
				
				var outputPort = nodeView.Q<Port>(connection.FromPortName);
				if (outputPort == null)
				{
					stateNode.RemoveConnection(connection);
					continue;
				}
				
				var inputPort = connectedNodeView.Q<Port>(null, "port", "input");
				if (inputPort == null)
				{
					stateNode.RemoveConnection(connection);
					continue;
				}
				
				var edge = new StateNodeEdge
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