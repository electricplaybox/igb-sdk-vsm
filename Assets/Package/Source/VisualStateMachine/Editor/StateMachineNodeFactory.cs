using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor
{
	public class StateMachineNodeFactory
	{
		public static StateNodeView CreateStateNode(StateNode stateNode, StateMachineGraphView graphView)
		{
			var stateType = stateNode.State.GetType();
			var stateName = stateType.Name;
			var stateTitle = StringUtils.PascalCaseToTitleCase(stateName);
			
			var node = new StateNodeView();
			node.Data = stateNode;
			node.title = stateTitle;
			node.name = stateNode.Id;
			
			if(stateNode.State is not EntryState) CreateInputPort(node, graphView);
			CreateOutputPorts(node, graphView);
			
			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(stateNode.Position, Vector2.one));
			
			var container = new VisualElement();
			container.name = "title-container";
			
			var title = node.Query<VisualElement>("title").First();
			var titleLabel = title.Query<VisualElement>("title-label").First();
			var titleButton = title.Query<VisualElement>("title-button-container").First();
			title.Remove(titleButton);
			
			var icon = new Image();
			icon.name = "title-icon";
			icon.scaleMode = ScaleMode.ScaleToFit;
			
			title.Add(container);
			container.Add(icon);
			container.Add(titleLabel);

			var progressBar = new ProgressBar();
			progressBar.name = "progress-bar";
			title.Add(progressBar);
			
			var contents = node.Query<VisualElement>("contents").First();
			var propertyContainer = new VisualElement()
			{
				name = "property-container"
			};
			
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);

			if (stateNode.State != null)
			{
				var stateInspector = CreateUIElementInspector(stateNode.State);
				propertyContainer.Add(stateInspector);
				
				if (stateInspector.childCount > 0)
				{
					propertyContainer.AddToClassList("has-properties");
				}

			}
			
			graphView.AddElement(node);

			return node;
		}
		
		public static VisualElement CreateUIElementInspector(UnityEngine.Object target, List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
 
			var serializedObject = new SerializedObject(target);
 
			var fields = GetInheritedSerializedFields(target.GetType());
 
			for (var i = 0; i < fields.Length; ++i) 
			{
				var field = fields[i];
				
				if ( propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) {
					continue;
				}

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
			var infoFields = new List<FieldInfo>();
 
			var publicFields = T.GetFields(BindingFlags.Instance | BindingFlags.Public);
			for (var i = 0; i < publicFields.Length; i++)
			{
				if (publicFields[i].GetCustomAttribute<HideInInspector>() != null) continue;
				infoFields.Add(publicFields[i]);
			}
 
			var privateFields = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (var t in privateFields)
			{
				if (t.GetCustomAttribute<SerializeField>() == null) continue;
				infoFields.Add(t);
			}
 
			return infoFields.ToArray();
		}
		
		public static FieldInfo[] GetInheritedSerializedFields(Type type)
		{
			var infoFields = new List<FieldInfo>();

			while (type != null && type != typeof(UnityEngine.Object))
			{
				var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
				foreach (var field in publicFields)
				{
					if (field.GetCustomAttribute<HideInInspector>() == null)
					{
						infoFields.Add(field);
					}
				}

				var privateFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
				foreach (var field in privateFields)
				{
					if (field.GetCustomAttribute<SerializeField>() != null)
					{
						infoFields.Add(field);
					}
				}

				// Move to the base type
				type = type.BaseType;
			}

			return infoFields.ToArray();
		}
		
		public static void CreateInputPort(Node node, StateMachineGraphView graphView)
		{
			var inputPort = node.InstantiatePort(Orientation.Horizontal, 
				Direction.Input, 
				Port.Capacity.Multi,
				typeof(Node));
			
			inputPort.name = inputPort.portName = "Enter";
			//inputPort.RemoveManipulator(inputPort.edgeConnector);
			inputPort.AddManipulator(new EdgeConnector<Edge>(new BezierEdgeListener(graphView)));
			node.inputContainer.Add(inputPort);
		}
		
		public static void CreateOutputPorts(StateNodeView node, StateMachineGraphView graphView)
		{
			var type = node.Data.State.GetType();
			var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			
			foreach (var eventInfo in events)
			{
				if (eventInfo.EventHandlerType != typeof(Action)) continue;
				
				var attributes = eventInfo.GetCustomAttributes(typeof(Transition), false);
				if (attributes.Length > 0) CreateOutputPort(node, eventInfo.Name, graphView, attributes[0] as Transition);
			}
		}
		
		public static void CreateOutputPort(StateNodeView node, string portName, StateMachineGraphView graphView, Transition transition)
		{
			var outputPort = node.InstantiatePort(Orientation.Horizontal, 
				Direction.Output, 
				Port.Capacity.Single,
				typeof(Node));
			
			var portText = string.IsNullOrEmpty(transition.PortLabel) ? portName : transition.PortLabel;
			
			outputPort.name = portName;
			outputPort.portName = portText;
			//outputPort.RemoveManipulator(outputPort.edgeConnector);
			outputPort.AddManipulator(new EdgeConnector<Edge>(new BezierEdgeListener(graphView)));
			
			node.outputContainer.Add(outputPort);
		}

		public static Edge ConnectStateNode(Port outputPort, StateNodeView destinationNode, GraphView graphView)
		{
			var inputPort = destinationNode.inputContainer.Q<Port>("Enter");
			
			var edge = new Edge()
			{
				input = inputPort,
				output = outputPort
			};
			
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
				var outputPort = nodeView.outputContainer.Q<Port>(connection.FromPortName);
				var inputPort = connectedNodeView.inputContainer.Q<Port>("Enter");
				
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