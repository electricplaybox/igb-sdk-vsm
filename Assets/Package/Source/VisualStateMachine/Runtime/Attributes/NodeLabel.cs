using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NodeLabel : Attribute
	{
		public string Text { get; private set; }
		
		public NodeLabel(string text)
		{
			Text = text;
		}
	}
}