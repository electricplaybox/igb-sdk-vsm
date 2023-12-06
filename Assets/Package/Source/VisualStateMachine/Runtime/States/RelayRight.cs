using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[PortOrientation(PortOrientation.Horizontal)]
	public class RelayRight : Relay
	{
		public RelayRight()
		{
			Direction = RelayDirection.Right;
		}

		public override void InitializeState()
		{
			
		}
	}
}