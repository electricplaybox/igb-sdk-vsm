using System;
using UnityEditor.Experimental.GraphView;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PortOrientationAttribute : Attribute
	{
		public Orientation Orientation { get; private set; }
		
		public PortOrientationAttribute(Orientation orientation)
		{
			Orientation = orientation;
		}
	}
}