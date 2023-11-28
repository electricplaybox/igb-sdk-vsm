using System.Linq;
using VisualStateMachine.Attributes;
using VisualStateMachine.Editor.Nodes;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.Tools;

namespace VisualStateMachine.Editor
{
	public class GraphStateManager
	{
		private StateMachineGraphView _graphView;
		
		public GraphStateManager(StateMachineGraphView graphView) 
		{
			_graphView = graphView;
		}

		public void LoadStateMachine(StateMachine stateMachine)
		{
			if (stateMachine != _graphView.StateMachine)
			{
				stateMachine.Save();
			}
			
			var nodeCount = stateMachine.Nodes.Count;
			if (nodeCount == 0)
			{
				stateMachine.AddEntryNode();
			}
			
			if (stateMachine == _graphView.StateMachine && nodeCount == _graphView.nodes.Count()) return;
			
			_graphView.SetStateMachine(stateMachine);
			_graphView.LoadGraphViewState();
			_graphView.ClearGraph();

			foreach (var node in stateMachine.Nodes)
			{
				AddNode(node);
			}
			
			foreach (var node in stateMachine.Nodes)
			{
				StateMachineNodeFactory.ConnectStateNode(node, _graphView);
			}
		}

		public void SaveGraphViewState()
		{
			if (_graphView.StateMachine == null) return;

			var contentContainer = _graphView.contentViewContainer;
			var position = contentContainer.transform.position;
			if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z)) return;
			
			_graphView.StateMachine.UpdateGraphViewState(contentContainer.transform.position, _graphView.scale);
		}

		public void UpdateNodes() 
		{
			foreach (var node in _graphView.nodes)
			{
				if (node is not StateNodeView stateNodeView) continue;

				stateNodeView.Update();
			}
		}
		
		public StateNodeView AddNode(StateNode stateNode)
		{
			var stateNodeType = stateNode.State.GetType();
			var nodeType = AttributeUtils.GetInheritedCustomAttribute<NodeTypeAttribute>(stateNodeType);
			var type = nodeType?.NodeType ?? NodeType.None;
			
			switch (type)
			{
				case NodeType.Relay:
					return StateMachineNodeFactory.CreateStateNode<RelayNodeView>(stateNode, _graphView);
				case NodeType.None:
				default:
					return StateMachineNodeFactory.CreateStateNode<StateNodeView>(stateNode, _graphView);
			}
		}
	}
}