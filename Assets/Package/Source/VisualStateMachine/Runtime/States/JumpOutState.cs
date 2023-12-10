namespace VisualStateMachine.States
{
	public class JumpOutState : JumpState
	{
		public override void EnterState()
		{
			this.StateMachineCore.JumpTo(this.JumpId);
		}

		public override void ExitState()
		{
			
		}
	}
}