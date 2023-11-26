using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Tools;

namespace VisualStateMachine.Editor
{
	public class StateNodeEdge : Edge
	{
		public override bool UpdateEdgeControl()
		{
			Vector2 startPosition;
			Vector2 endPosition;

			// Check if we are dragging the edge from output to input
			if (output != null && input == null)
			{
				startPosition = output.GetGlobalCenter();
				endPosition = edgeControl.to; // Use current end position of the edge control
				// Apply custom logic for dragging from output
				SetControlPointsForDragging(output,startPosition, endPosition, true);
			}
			// Check if we are dragging the edge from input to output
			else if (input != null && output == null)
			{
				startPosition = edgeControl.from; // Use current start position of the edge control
				endPosition = input.GetGlobalCenter();
				// Apply custom logic for dragging from input
				SetControlPointsForDragging(input, startPosition, endPosition, false);
			}
			else if (input != null && output != null)
			{
				// // Normal edge drawing logic when both ports are connected
				// startPosition = output.GetGlobalCenter();
				// endPosition = input.GetGlobalCenter();
				// SetControlPointsForConnectedEdge(startPosition, endPosition);
			}
			
			return base.UpdateEdgeControl();
		}
		
		private void SetControlPointsForDragging(Port originPort, Vector2 start, Vector2 end, bool isOutputDragging)
		{
			var points = this.edgeControl.controlPoints;
			if (points == null) return;

			var direction = originPort.style.flexDirection;
			
			switch (direction.value)
			{
				case FlexDirection.Row:
					points[1].x = -30;
					points[2].x += 30;
					break;
				case FlexDirection.RowReverse:
					
					break;
				case FlexDirection.Column:
					
					break;
				case FlexDirection.ColumnReverse:
					
					break;
			}
		}

		private void SetControlPointsForConnectedEdge(Vector2 start, Vector2 end)
		{
			var points = this.edgeControl.controlPoints;
			if (points == null) return;
			
		}
	}
}