using System;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	public class RelayRight : Relay
	{
		public RelayRight()
		{
			Direction = RelayDirection.Right;
		}
	}
}