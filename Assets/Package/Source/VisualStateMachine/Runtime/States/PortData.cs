using System;

namespace VisualStateMachine.States
{
	[Serializable]
	public class PortData
	{
		public int FrameDelay = 1;
		public string PortLabel = string.Empty;
		public string PortColor = default;
	}
}