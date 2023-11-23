using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Event)]
	public class TransitionAttribute : Attribute
	{
		public string PortLabel { get; private set; } = string.Empty;
		
		public TransitionAttribute()
		{
			
		}
		
		public TransitionAttribute(string portLabel)
		{
			PortLabel = portLabel;
		}
	}
}