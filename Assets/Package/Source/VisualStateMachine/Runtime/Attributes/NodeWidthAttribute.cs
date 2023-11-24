using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeWidthAttribute : Attribute
	{
		public float Width { get; private set; }

		public NodeWidthAttribute(float width)
		{
			Width = width;
		}
	}
}