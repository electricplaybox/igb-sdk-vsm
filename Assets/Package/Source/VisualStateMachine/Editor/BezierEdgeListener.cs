using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VisualStateMachine.Editor
{
	public class BezierEdgeListener : IEdgeConnectorListener
	{
		private readonly StateMachineGraphView _graphView;

		public BezierEdgeListener(StateMachineGraphView graphView)
		{
			_graphView = graphView;
		}
		
		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			if(edge.output.connections.ToList().Count > 0) return;
			
			var port = edge.output;
			_graphView.CreateNewStateNodeFromOutputPort(port, position);
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			// graphView.AddElement(edge);
			// _graphView.AddConnectionToState(edge);
		}
	}

}