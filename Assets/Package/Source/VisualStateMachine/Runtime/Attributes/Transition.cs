using System;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Event)]
	public class Transition : Attribute
	{
		public string PortLabel { get; private set; } = string.Empty;
		
		public Transition()
		{
			
		}
		
		public Transition(string portLabel)
		{
			PortLabel = portLabel;
		}
	}
}