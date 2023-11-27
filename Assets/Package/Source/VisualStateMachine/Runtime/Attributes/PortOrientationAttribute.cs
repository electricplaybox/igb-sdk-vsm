using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PortOrientationAttribute : Attribute
	{
		public PortOrientation PortOrientation { get; private set; }
		
		public PortOrientationAttribute(PortOrientation portOrientation)
		{
			PortOrientation = portOrientation;
		}
	}
}