using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Ports;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor.Nodes
{
	public class NodeView : Node
	{
		public StateNode Data;
		public StateMachineGraphView GraphView;

		public NodeView(StateNode stateNode, StateMachineGraphView graphView)
		{
			Data = stateNode;
			GraphView = graphView;
			
			if (stateNode == null) return;
			
			this.name = stateNode.Id;
			var stateType = stateNode.State.GetType();
			
			var noInputPort = AttributeUtils.GetInheritedCustomAttribute<NoInputPortAttribute>(stateType);
			if(noInputPort == null) StateMachineNodeFactory.CreateInputPort(this, graphView);
			StateMachineNodeFactory.CreateOutputPorts(this, graphView);
			
			this.RefreshPorts();
			this.RefreshExpandedState();
			this.SetPosition(new Rect(stateNode.Position, Vector2.one));
		}

		public virtual void Update(StateMachine stateMachine)
		{
			
		}
		
		public override Port InstantiatePort(
			Orientation orientation,
			Direction direction,
			Port.Capacity capacity,
			System.Type type)
		{
			return StatePort.Create<Edge>(orientation, direction, capacity, type);
		}

		public void RemoveTitle()
		{
			var title = this.Q<VisualElement>("title");
			if (title == null) return;
			title.parent.Remove(title);
		}
	}
}