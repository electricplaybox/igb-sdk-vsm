using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeLabelAttribute : Attribute
	{
		public string Text { get; private set; }
		
		public NodeLabelAttribute(string text)
		{
			Text = text;
		}
	}
}