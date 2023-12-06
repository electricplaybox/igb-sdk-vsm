using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[PortOrientation(PortOrientation.Horizontal)]
	public class RelayLeft : Relay
	{
		public RelayLeft()
		{
			Direction = RelayDirection.Left;
		}

		public override void InitializeState()
		{
			
		}
	}
}