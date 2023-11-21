using VisualStateMachine.Attributes;

namespace VisualStateMachine.States
{
	[NodeColor(NodeColor.Red)]
	public class EndState : State
	{
		public override void EnterState()
		{
			Controller.Complete();
		}

		public override void UpdateState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}