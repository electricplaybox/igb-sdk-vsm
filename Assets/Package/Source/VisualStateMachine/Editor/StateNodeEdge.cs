using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor
{
	public class StateNodeEdge : Edge
	{
		protected override EdgeControl CreateEdgeControl() => new StateNodeEdgeControl()
		{
			capRadius = 4f,
			interceptWidth = 6f
		};

		public StateNodeEdge()
		{
			edgeControl.RegisterCallback<GeometryChangedEvent>(HandleGeometryChange);
		}

		private void HandleGeometryChange(GeometryChangedEvent evt)
		{
			if (output != null && input == null)
			{
				SetControlPointsForOutputDragging(output);
			}
			else if (input != null && output == null)
			{
				SetControlPointsForInputDragging(input);
			}
			if (input != null && output != null)
			{
				SetControlPointsForConnectedEdge(input, output);
			}
		}

		private void SetControlPointsForInputDragging(Port port)
		{
			var points = edgeControl.controlPoints;
			if (points == null) return;
			
			var direction = port.resolvedStyle.flexDirection;
			
			switch (direction)
			{
				case FlexDirection.Row:
					edgeControl.controlPoints[1].x += 5;
					edgeControl.controlPoints[2].x -= 5;
					break;
				case FlexDirection.RowReverse:
					edgeControl.controlPoints[1].x -= 55;
					edgeControl.controlPoints[2].x += 55;
					break;
				case FlexDirection.ColumnReverse:
					edgeControl.controlPoints[2].y += 55;
					break;
			}
		}
		
		private void SetControlPointsForOutputDragging(Port port)
		{
			var points = edgeControl.controlPoints;
			if (points == null) return;
			
			var direction = port.resolvedStyle.flexDirection;
			
			switch (direction)
			{
				case FlexDirection.Row:
					edgeControl.controlPoints[1].x -= 55;
					edgeControl.controlPoints[2].x += 55;
					break;
				case FlexDirection.RowReverse:
					edgeControl.controlPoints[1].x -= 5;
					edgeControl.controlPoints[2].x += 5;
					break;
				case FlexDirection.Column:
					edgeControl.controlPoints[1].y -= 55;
					edgeControl.controlPoints[2].y += 55;
					break;
			}
		}

		private void SetControlPointsForConnectedEdge(Port input, Port output)
		{
			var points = edgeControl.controlPoints;
			if (points == null) return;
				
			var inputDirection = input.resolvedStyle.flexDirection;
			var outputDirection = output.resolvedStyle.flexDirection;
			
			switch (inputDirection)
			{
				case FlexDirection.RowReverse:
					edgeControl.controlPoints[1].x += 5;
					edgeControl.controlPoints[2].x += 55;
					break;
				case FlexDirection.ColumnReverse:
					edgeControl.controlPoints[2].y += 55;
					break;
			}
			
			switch (outputDirection)
			{
				case FlexDirection.Row:
					edgeControl.controlPoints[1].x -= 55;
					break;
				case FlexDirection.RowReverse:
					edgeControl.controlPoints[1].x += 5;
					break;
				case FlexDirection.Column:
					edgeControl.controlPoints[1].y -= 55;
					break;
			}
		}
	}
}