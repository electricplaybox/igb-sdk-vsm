using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.VisualStateMachineEditor
{
	public class BezierEdgeConnector : IEdgeConnectorListener
	{
		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			var customEdge = edge as BezierEdge ?? new BezierEdge();
			graphView.AddElement(customEdge);
		}
	}

}