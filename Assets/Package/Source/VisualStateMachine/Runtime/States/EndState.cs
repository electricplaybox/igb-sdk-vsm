using UnityEngine;
using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Red)]
	public class EndState : State
	{
		public override void InitializeState()
		{
			
		}

		public override void EnterState()
		{
			StateMachineCore.Complete();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}