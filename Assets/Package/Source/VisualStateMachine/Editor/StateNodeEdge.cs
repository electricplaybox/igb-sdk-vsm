using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualStateMachine.Tools;

namespace VisualStateMachine.Editor
{
	public class StateNodeEdge : Edge
	{
		protected override EdgeControl CreateEdgeControl() => new StateNodeEdgeControl(this)
		{
			capRadius = 4f,
			interceptWidth = 6f
		};

		public StateNodeEdge()
		{
			edgeControl.RegisterCallback<GeometryChangedEvent>(HandleGeometryChange);
		}

		public override bool UpdateEdgeControl()
		{
			if (!base.UpdateEdgeControl()) return false;

			UpdateEdge();
			return true;
		}

		private void HandleGeometryChange(GeometryChangedEvent evt)
		{
				if (output != null && input == null)
				{
					SetControlPointsForDragging(output);
				}
				else if (input != null && output == null)
				{
					SetControlPointsForDragging(input);
				}
				else if (input != null && output != null)
				{
					SetControlPointsForConnectedEdge(input, output);
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
				case FlexDirection.Column:
					break;
				case FlexDirection.ColumnReverse:
					break;
				case FlexDirection.Row:
					break;
				case FlexDirection.RowReverse:
					edgeControl.controlPoints[2].x += 55;
					break;
			}
			
			switch (outputDirection)
			{
				case FlexDirection.Column:
					break;
				case FlexDirection.ColumnReverse:
					break;
				case FlexDirection.Row:
					edgeControl.controlPoints[1].x -= 55;
					break;
				case FlexDirection.RowReverse:
					edgeControl.controlPoints[1].x += 5;
					break;
			}
		}

		private void SetControlPointsForDragging(Port originPort)
		{
			var points = edgeControl.controlPoints;
			if (points == null) return;
			
			var direction = originPort.resolvedStyle.flexDirection;

			switch (direction)
			{
				case FlexDirection.Column:
					break;
				case FlexDirection.ColumnReverse:
					break;
				case FlexDirection.Row:
					// edgeControl.controlPoints[1].x -= 60;
					break;
				case FlexDirection.RowReverse:
					break;
			}
		}

		private void UpdateEdge()
		{
			
		}
	}
}