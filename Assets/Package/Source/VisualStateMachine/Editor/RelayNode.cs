using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor
{
	public class RelayNode : StateNodeView
	{
		public RelayNode(StateNode stateNode, string stateTitle, string stateName, StateMachineGraphView graphView) : base(stateNode, stateTitle, stateName, graphView)
		{
		}

		public virtual void Update()
		{
			if (Data == null) return;
			
			base.Update();

			CreateVerticalRelayNode();
		}

		private void CreateVerticalRelayNode()
		{
			if (Data.State is not Relay) return;
			var relayState = Data.State as Relay;
			
			this.AddToClassList("relay-node");
			
			var title = this.Q<VisualElement>("title");
			if (title == null) return;
			
			title.parent.Remove(title);
			
			var propertyContainer = this.Q<VisualElement>("property-container");
			if(propertyContainer == null) return;
			
			propertyContainer.parent.Remove(propertyContainer);

			var contents = this.Q("contents");
			var top = contents.Q("top");
			
			var input = contents.Q("input");
			var inputPort = input.Q<Port>();
			inputPort.style.flexDirection = FlexDirection.Column;
			
			var output = contents.Q("output");
			var outputPort = output.Q<Port>();
			outputPort.style.flexDirection = FlexDirection.ColumnReverse;
			
			var contentDivider = contents.Q("divider");
			contentDivider.parent.Remove(contentDivider);
			
			var topDivider = top.Q("divider");
			topDivider.parent.Remove(topDivider);
			
			var bottom = new VisualElement();
			bottom.name = "bottom";
			bottom.Insert(0, inputPort);
			contents.Insert(0, bottom);
			
			var topInput = top.Q("input");
			top.Remove(topInput);
		}
	}
}