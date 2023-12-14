using System;
using VisualStateMachine.States;

namespace VisualStateMachine.Attributes
{
	[AttributeUsage(AttributeTargets.Event)]
	public class TransitionAttribute : Attribute
	{
		public PortData PortData { get; private set; } = new ();
		
		public TransitionAttribute()
		{
			
		}
		
		public TransitionAttribute(string portLabel, string portColor = default, int frameDelay = 1)
		{
			PortData.PortLabel = portLabel;
			PortData.PortColor = portColor;
			PortData.FrameDelay = frameDelay;
		}
	}
}