using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Event)]
	public class TransitionAttribute : Attribute
	{
		public string PortLabel { get; private set; } = string.Empty;
		public string PortColor { get; private set; } = default;
		
		public TransitionAttribute()
		{
			
		}
		
		public TransitionAttribute(string portLabel, string portColor = default)
		{
			PortLabel = portLabel;
			PortColor = portColor;
		}
	}
}