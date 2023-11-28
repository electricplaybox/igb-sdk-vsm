using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor.Nodes
{
	public class RelayNodeView : StateNodeView
	{
		public RelayNodeView(StateNode stateNode, string stateTitle, string stateName, StateMachineGraphView graphView) : base(stateNode, stateTitle, stateName, graphView)
		{
			
		}

		public override void Update()
		{
			base.Update();
			
			if (Data == null) return;
			if (Data.State is not Relay) return;
			var relayState = Data.State as Relay;
			
			this.AddToClassList("relay-node");
			
			var title = this.Q<VisualElement>("title");
			if (title == null) return;
			title.parent.Remove(title);

			var stateType = Data.State.GetType();
			switch (stateType.Name)
			{
				case "RelayDown":
				case "RelayUp":
					CreateVerticalRelayNode(relayState);
					break;
				case "RelayRight":
				case "RelayLeft":
					CreateHorizontalRelayNode(relayState);
					break;
			}
		}

		private void CreateHorizontalRelayNode(Relay relayState)
		{
			this.AddToClassList("horizontal");
			
			var propertyContainer = this.Q<VisualElement>("property-container");
			if(propertyContainer == null) return;
			
			propertyContainer.parent.Remove(propertyContainer);

			var contents = this.Q("contents");
			var top = contents.Q("top");

			if (relayState.Direction == RelayDirection.Left)
			{
				SwapNodes();
				SetPortFlexDirection(FlexDirection.RowReverse, FlexDirection.Row);
			}
			else
			{
				SetPortFlexDirection(FlexDirection.Row, FlexDirection.RowReverse);
			}
		}

		private void SwapNodes()
		{
			var input = this.Q(className: "input");
			var output = this.Q(className:"output");
			
			var inputParent = input.parent;
			var outputParent = output.parent;
			
			inputParent.Add(output);
			outputParent.Add(input);
		}

		private void SetPortFlexDirection(FlexDirection inputDirection, FlexDirection outputDirection)
		{
			var input = this.Q(className: "input");
			var inputPort = input.Q<Port>();
			inputPort.style.flexDirection = inputDirection;
			
			var output = this.Q(className:"output");
			var outputPort = output.Q<Port>();
			outputPort.style.flexDirection = outputDirection;
		}

		private void CreateVerticalRelayNode(Relay relayState)
		{
			var propertyContainer = this.Q<VisualElement>("property-container");
			if(propertyContainer == null) return;
			
			propertyContainer.parent.Remove(propertyContainer);
			
			var contents = this.Q("contents");
			var top = contents.Q("top");
			
			var input = contents.Q("input");
			var inputPort = input.Q<Port>();
			
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

			if (relayState.Direction == RelayDirection.Up)
			{
				SwapNodes();
				SetPortFlexDirection(FlexDirection.ColumnReverse, FlexDirection.Column);
			}
			else
			{
				SetPortFlexDirection(FlexDirection.Column, FlexDirection.ColumnReverse);
			}
		}
	}
}