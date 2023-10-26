using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Vsm.Editor.Nodes;
using Vsm.Serialization;
using Vsm.States;

namespace Vsm.Editor.Graph
{
	public class VsmNodeFactory
	{
		private readonly VsmGraphView _graphView;
		private readonly VsmPortConnector _portConnector;

		public VsmNodeFactory(VsmGraphView graphView)
		{
			_graphView = graphView;
			_portConnector = new VsmPortConnector(graphView);
		}

		public void CreateNode(Type state, Vector2 position)
		{
			var node = new StateNode();
			node.GUID = Guid.NewGuid().ToString();
			node.styleSheets.Add(Resources.Load<StyleSheet>("BaseNode"));
			node.title = state.Name;
			node.State = Activator.CreateInstance(state) as State;

			_portConnector.CreateInputPort(node);
			_portConnector.AddOutputPorts(node);

			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(position, Vector2.one));

			_graphView.AddElement(node);
		}

		public void CreateNode(StateNodeData data)
		{
			var state = Type.GetType(data.State);
			if (state == null) return;

			var node = new StateNode();
			node.GUID = data.GUID;
			node.styleSheets.Add(Resources.Load<StyleSheet>("BaseNode"));
			node.title = data.Title;
			node.State = Activator.CreateInstance(state) as State;

			_portConnector.CreateInputPort(node);
			_portConnector.AddOutputPorts(node);

			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(data.Position, Vector2.one));

			_graphView.AddElement(node);
		}

		public void SetAsEntryNode(BaseNode entryNode)
		{
			var elements = _graphView.graphElements.ToList();
			var baseNodes = elements.OfType<BaseNode>();

			foreach (var baseNode in baseNodes) baseNode.SetAsEntryPoint(baseNode.GUID == entryNode.GUID);
		}
	}
}