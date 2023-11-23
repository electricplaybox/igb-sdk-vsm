using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class RelayLeft : Relay
	{
		public RelayLeft()
		{
			Direction = RelayDirection.Left;
		}
	}
}