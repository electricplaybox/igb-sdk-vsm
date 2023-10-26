using System;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Vsm.Attributes;
using Vsm.Editor.Nodes;
using Vsm.Serialization;

namespace Vsm.Editor.Graph
{
	public class VsmPortConnector
	{
		private readonly VsmGraphView _graphView;

		public VsmPortConnector(VsmGraphView graphView)
		{
			_graphView = graphView;
		}

		public void ConnectPorts(EdgeData edge)
		{
			var outputNode = GetNodeByGUID(edge.OutputNode);
			var outputPort = GetOutputPortByName(outputNode, edge.OutputPort);

			var inputNode = GetNodeByGUID(edge.InputNode);
			var inputPort = GetInputPortByName(inputNode, edge.InputPort);

			ConnectPorts(outputPort, inputPort);
		}

		public void ConnectPorts(Port output, Port input)
		{
			if (output == null || input == null) return;

			var edge = new Edge
			{
				input = input,
				output = output
			};

			input.Connect(edge);
			output.Connect(edge);

			_graphView.Add(edge);
		}

		private Node GetNodeByGUID(string guid)
		{
			var elements = _graphView.graphElements.ToList();

			return elements.FirstOrDefault(node => (node as BaseNode).Guid == guid) as Node;
		}

		private Port GetOutputPortByName(Node node, string portName)
		{
			return node.outputContainer.Query<Port>().ToList().FirstOrDefault(port => port.portName == portName);
		}

		private Port GetInputPortByName(Node node, string portName)
		{
			return node.inputContainer.Query<Port>().ToList().FirstOrDefault(port => port.portName == portName);
		}

		public void CreateOutputPort(Node node, string portName)
		{
			var outputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
				typeof(Node));
			outputPort.portName = portName;
			node.outputContainer.Add(outputPort);
		}

		public void CreateInputPort(Node node)
		{
			var inputPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,
				typeof(Node));
			inputPort.portName = "Enter";
			node.inputContainer.Add(inputPort);
		}

		public void AddOutputPorts(StateNode node)
		{
			var type = node.State.GetType();

			// Loop through all public events declared in the type
			foreach (var eventInfo in type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
			{
				// Skip events that are not of type Action
				if (eventInfo.EventHandlerType != typeof(Action)) continue;

				// Get custom attributes of the Port type
				var attributes = eventInfo.GetCustomAttributes(typeof(Transition), false);

				// If the event has the Port attribute, print its details
				if (attributes.Length > 0) CreateOutputPort(node, eventInfo.Name);
			}
		}
	}
}