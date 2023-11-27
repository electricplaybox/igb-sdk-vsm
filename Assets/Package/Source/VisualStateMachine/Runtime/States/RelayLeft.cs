using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[PortOrientation(Orientation.Horizontal)]
	public class RelayLeft : Relay
	{
		public RelayLeft()
		{
			Direction = RelayDirection.Left;
		}
	}
}