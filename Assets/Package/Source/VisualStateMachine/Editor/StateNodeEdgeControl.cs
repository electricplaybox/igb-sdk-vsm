using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor
{
	public class StateNodeEdgeControl : EdgeControl
	{
		private readonly Edge _edge;

		public StateNodeEdgeControl(Edge edge) : base()
		{
			_edge = edge;
			
		}
		//
		// public override void UpdateLayout()
		// {
		// 	DrawCustomEdge();
		// 	base.UpdateLayout();
		// 	
		// }
		//
		// private void DrawCustomEdge()
		// {
		// 	if (_edge.output != null && _edge.input == null)
		// 	{
		// 		SetControlPointsForDragging(_edge.output);
		// 	}
		// 	else if (_edge.input != null && _edge.output == null)
		// 	{
		// 		SetControlPointsForDragging(_edge.input);
		// 	}
		// 	else if (_edge.input != null && _edge.output != null)
		// 	{
		// 		SetControlPointsForConnectedEdge(_edge.input, _edge.output);
		// 	}
		// }
		//
		// private void SetControlPointsForConnectedEdge(Port input, Port output)
		// {
		// 	var points = controlPoints;
		// 	if (points == null) return;
		// 	
		// 	var inputDirection = input.style.flexDirection;
		// 	var outputDirection = output.style.flexDirection;
		// 	
		// 	switch (inputDirection.value)
		// 	{
		// 		case FlexDirection.Row:
		// 			
		// 			break;
		// 		case FlexDirection.RowReverse:
		// 			
		// 			break;
		// 		case FlexDirection.Column:
		// 			
		// 			break;
		// 		case FlexDirection.ColumnReverse:
		// 			
		// 			break;
		// 	}
		// 	
		// 	switch (outputDirection.value)
		// 	{
		// 		case FlexDirection.Row:
		// 			points[1].x -= 5;
		// 			break;
		// 		case FlexDirection.RowReverse:
		// 			
		// 			break;
		// 		case FlexDirection.Column:
		// 			
		// 			break;
		// 		case FlexDirection.ColumnReverse:
		// 			
		// 			break;
		// 	}
		// }
		//
		// private void SetControlPointsForDragging(Port originPort)
		// {
		// 	var points = controlPoints;
		// 	if (points == null) return;
		//
		// 	var direction = originPort.style.flexDirection;
		// 	
		// 	switch (direction.value)
		// 	{
		// 		case FlexDirection.Row:
		// 			points[1].x -= 30;
		// 			break;
		// 		case FlexDirection.RowReverse:
		// 			
		// 			break;
		// 		case FlexDirection.Column:
		// 			
		// 			break;
		// 		case FlexDirection.ColumnReverse:
		// 			
		// 			break;
		// 	}
		// }
	}
}