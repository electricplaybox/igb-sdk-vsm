using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[PortOrientation(Orientation.Horizontal)]
	public class RelayRight : Relay
	{
		public RelayRight()
		{
			Direction = RelayDirection.Right;
		}
	}
}