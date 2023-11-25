using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VisualStateMachine.Editor
{
	public class StatePort : Port
	{
		private int _portSizeIncrease = 24;
		private int _offsetX = 0;
		private int _offsetY = -5;
		
		protected StatePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
		{
			
		}
		
		public static StatePort Create<T>(Orientation orientation, Direction direction, Capacity capacity, Type type) where T : Edge
		{
			return new StatePort(orientation, direction, capacity, type);
		}

		public override bool ContainsPoint(Vector2 localPoint)
		{
			Rect expandedBounds = new Rect(
				this.resolvedStyle.left - (_portSizeIncrease* 0.5f) + _offsetX,
				this.resolvedStyle.top - (_portSizeIncrease* 0.5f) + _offsetY,
				this.resolvedStyle.width + _portSizeIncrease,
				this.resolvedStyle.height + _portSizeIncrease
			);

			return expandedBounds.Contains(localPoint);
		}
	}
}