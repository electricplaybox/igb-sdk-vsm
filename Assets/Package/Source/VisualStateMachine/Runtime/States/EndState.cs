using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Red), NodeIcon(NodeIcon.VsmFlatFlagWhite, opacity:0.3f)]
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