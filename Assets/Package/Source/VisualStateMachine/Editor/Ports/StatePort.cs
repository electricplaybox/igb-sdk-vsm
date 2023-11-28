using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace VisualStateMachine.Editor.Ports
{
	public class StatePort : Port
	{
		private int _portSizeIncrease = 0;
	
		protected StatePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
		{
			
		}
		
		public static StatePort Create<T>(Orientation orientation, Direction direction, Capacity capacity, Type type) where T : Edge
		{
			return new StatePort(orientation, direction, capacity, type);
		}

		public override bool ContainsPoint(Vector2 localPoint)
		{
			var expandedBounds = new Rect(
				this.resolvedStyle.left - (_portSizeIncrease* 0.5f),
				-(_portSizeIncrease* 0.5f),
				this.resolvedStyle.width + _portSizeIncrease,
				this.resolvedStyle.height + _portSizeIncrease
			);
			
			// DevLog.Log($"ContainsPoint: {localPoint}, center:{expandedBounds.center}, offset:({_offsetX},{_offsetY}), top:{this.resolvedStyle.top}, height:{this.resolvedStyle.height}");

			return expandedBounds.Contains(localPoint);
		}
	}
}