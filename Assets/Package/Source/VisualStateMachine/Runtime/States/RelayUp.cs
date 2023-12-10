using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[HideNode]
	public class RelayUp : Relay
	{
		public RelayUp()
		{
			Direction = RelayDirection.Up;
		}
	}
}