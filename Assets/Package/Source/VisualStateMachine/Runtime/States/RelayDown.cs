using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[HideNode]
	public class RelayDown : Relay
	{
		public RelayDown()
		{
			Direction = RelayDirection.Down;
		}
	}
}