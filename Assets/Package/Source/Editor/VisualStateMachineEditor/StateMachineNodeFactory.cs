using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine;

namespace Editor.VisualStateMachineEditor
{
	public class StateMachineNodeFactory
	{
		public static void CreateStateNode(StateNode stateNode, GraphView graphView)
		{
			var node = new StateNodeView();
			node.Data = stateNode;
			node.title = stateNode.State.GetType().Name;
			node.name = stateNode.Id;
			
			CreateInputPort(node);
			CreateOutputPorts(node);
			
			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(stateNode.Position, Vector2.one));
			
			var container = new VisualElement();
			container.name = "title-container";
			
			var title = node.Query<VisualElement>("title").First();
			var titleLabel = title.Query<VisualElement>("title-label").First();
			var titleButton = title.Query<VisualElement>("title-button-container").First();
			
			title.Add(container);
			container.Add(titleLabel);
			container.Add(titleButton);

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
			}
			
			graphView.AddElement(node);
		}
		
		public static VisualElement CreateUIElementInspector(UnityEngine.Object target, List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
 
			var serializedObject = new SerializedObject(target);
 
			var fields = GetVisibleSerializedFields(target.GetType());
 
			for (var i = 0; i < fields.Length; ++i) {
				var field = fields[i];
				
				if ( propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) {
					continue;
				}

				var serializedProperty = serializedObject.FindProperty(field.Name);
				if (serializedProperty != null)
				{
					var propertyField = new PropertyField(serializedProperty);
					if(propertyField != null) container.Add(propertyField);
				}
				else
				{
					Debug.LogWarning($"Property {field.Name} not found in serialized object.");
				}
			}
           
			if(serializedObject != null) container.Bind(serializedObject);
 
 
			return container;
		}
		
		public static FieldInfo[] GetVisibleSerializedFields(Type T)
		{
			List<FieldInfo> infoFields = new List<FieldInfo>();
 
			var publicFields = T.GetFields(BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < publicFields.Length; i++) {
				if (publicFields[i].GetCustomAttribute<HideInInspector>() == null) {
					infoFields.Add(publicFields[i]);
				}
			}
 
			var privateFields = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			for (int i = 0; i < privateFields.Length; i++) {
				if (privateFields[i].GetCustomAttribute<SerializeField>() != null) {
					infoFields.Add(privateFields[i]);
				}
			}
 
			return infoFields.ToArray();
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
			var type = node.Data.State.GetType();
			
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