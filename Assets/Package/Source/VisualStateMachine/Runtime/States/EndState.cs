using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Red)]
	public class EndState : State
	{
		public override void EnterState()
		{
			StateMachineCore.Complete(this);
		}

		public override void ExitState()
		{
			
		}
	}
}