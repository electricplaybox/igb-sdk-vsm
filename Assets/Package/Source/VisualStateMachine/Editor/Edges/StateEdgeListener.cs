using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VisualStateMachine.Editor.Nodes;

namespace VisualStateMachine.Editor.Edges
{
	public class StateEdgeListener : IEdgeConnectorListener
	{
		private readonly StateMachineGraphView _graphView;

		public StateEdgeListener(StateMachineGraphView graphView)
		{
			_graphView = graphView;
		}
		
		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			if (edge.output == null) return;
			if(edge.output.connections.ToList().Count > 0) return;
			
			var port = edge.output;
			_graphView.CreateNewStateNodeFromOutputPort(port, position);
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			if (edge.output == null) return;
			if(edge.output.connections.ToList().Count > 0) return;
			
			var port = edge.output;
			var inputNode = edge.input.node as StateNodeView;
			
			var connectedEdge = StateMachineNodeFactory.ConnectStateNode(port, inputNode, _graphView);
			_graphView.AddConnectionToState(connectedEdge);
		}
	}

}