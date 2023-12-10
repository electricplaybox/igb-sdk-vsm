using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[HideNode]
	[PortOrientation(PortOrientation.Horizontal)]
	public class RelayLeft : Relay
	{
		public RelayLeft()
		{
			Direction = RelayDirection.Left;
		}
	}
}