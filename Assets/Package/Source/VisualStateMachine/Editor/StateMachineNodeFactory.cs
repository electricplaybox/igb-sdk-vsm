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

namespace VisualStateMachine.Editor
{
	public class StateMachineNodeFactory
	{
		public const string DefaultInputNodeName = "Enter";
		
		public static StateNodeView CreateStateNode(StateNode stateNode, StateMachineGraphView graphView)
		{
			var stateType = stateNode.State.GetType();
			var stateName = stateType.Name;
			var stateTitle = StringUtils.PascalCaseToTitleCase(stateName);
			var node = new StateNodeView(stateNode, stateTitle, stateNode.Id, graphView);
			
			graphView.AddElement(node);
			return node;
		}
		
		public static T CreateStateNode<T>(StateNode stateNode, StateMachineGraphView graphView) where T : StateNodeView
		{
			var stateType = stateNode.State.GetType();
			var stateName = stateType.Name;
			var stateTitle = StringUtils.PascalCaseToTitleCase(stateName);
			var node = Activator.CreateInstance(typeof(T), new object[] {stateNode, stateTitle, stateName, graphView}) as T;
			
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

				// Move to the base type
				type = type.BaseType;
			}

			return infoFields.ToArray();
		}

		public static void CreateInputPort(StateNodeView node, StateMachineGraphView graphView)
		{
			var nodeType = node.Data.State.GetType();
			var orientationAtt = AttributeUtils.GetInheritedCustomAttribute<PortOrientationAttribute>(nodeType);
			var orientation = orientationAtt != null ? (Orientation)orientationAtt.PortOrientation : Orientation.Horizontal;
			var inputPort = node.InstantiatePort(orientation, Direction.Input, Port.Capacity.Multi, typeof(Node));

			inputPort.name = inputPort.portName = DefaultInputNodeName;
			inputPort.AddManipulator(new EdgeConnector<StateNodeEdge>(new StateEdgeListener(graphView)));
			node.inputContainer.Add(inputPort);
		}

		public static void CreateOutputPorts(StateNodeView node, StateMachineGraphView graphView)
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
		
		public static void CreateOutputPort(StateNodeView node, string portName, StateMachineGraphView graphView, 
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

		public static Edge ConnectStateNode(Port outputPort, StateNodeView destinationNode, GraphView graphView)
		{
			var inputPort = destinationNode.Q<Port>(null, "port", "input");
			
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
			var nodeView = graphView.Q<StateNodeView>(stateNode.Id);
			
			foreach (var connection in stateNode.Connections)
			{
				var connectedNodeView = graphView.Q<StateNodeView>(connection.ToNodeId);
				var outputPort = nodeView.Q<Port>(connection.FromPortName);
				var inputPort = connectedNodeView.Q<Port>(null, "port", "input");
				
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