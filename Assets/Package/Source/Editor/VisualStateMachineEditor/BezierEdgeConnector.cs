using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.VisualStateMachineEditor
{
	public class BezierEdgeConnector : IEdgeConnectorListener
	{
		private readonly StateMachineGraphView _graphView;

		public BezierEdgeConnector(StateMachineGraphView graphView)
		{
			_graphView = graphView;
		}
		
		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			var port = edge.output;
			_graphView.CreateNewStateNodeFromOutputPort(port, position);
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			var customEdge = edge as BezierEdge ?? new BezierEdge();
			graphView.AddElement(customEdge);
		}
	}

}