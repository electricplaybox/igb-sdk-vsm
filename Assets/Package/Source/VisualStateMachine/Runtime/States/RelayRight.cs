using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[HideNode]
	[PortOrientation(PortOrientation.Horizontal)]
	public class RelayRight : Relay
	{
		public RelayRight()
		{
			Direction = RelayDirection.Right;
		}
	}
}