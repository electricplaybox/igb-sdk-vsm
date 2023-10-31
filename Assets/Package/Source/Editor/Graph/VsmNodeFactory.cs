using System;
using System.Data;
using System.Linq;
using UnityEditor.UIElements;
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
			node.styleSheets.Add(Resources.Load<StyleSheet>("StateNode"));
			node.title = state.Name;

			node.Data = new StateNodeData();
			node.Data.Guid = Guid.NewGuid().ToString();
			node.Data.State = state.AssemblyQualifiedName;
			
			_portConnector.CreateInputPort(node);
			_portConnector.AddOutputPorts(node);

			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(position, Vector2.one));

			_graphView.AddElement(node);
		}

		public void CreateNode(StateNodeData data)
		{
			var node = new StateNode();
			node.styleSheets.Add(Resources.Load<StyleSheet>("StateNode"));
			node.title = data.Title;
			node.Data = data;
			
			_portConnector.CreateInputPort(node);
			_portConnector.AddOutputPorts(node);

			node.RefreshPorts();
			node.RefreshExpandedState();
			node.SetPosition(new Rect(data.Position, Vector2.one));

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
			
			_graphView.AddElement(node);
		}
		
		public void SetAsEntryNode(StateNode entryNode)
		{
			var elements = _graphView.graphElements.ToList();
			var nodes = elements.OfType<StateNode>();

			foreach (var node in nodes) node.Data.EntryPoint = (node.Data.Guid == entryNode.Data.Guid);
		}
	}
}