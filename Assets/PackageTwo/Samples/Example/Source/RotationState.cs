using VisualStateMachine;

namespace Example
{
	public class RotationState : State
	{
		private StateMachineReferences _references;

		public override void Enter()
		{
			_references = this.Controller.GetComponent<StateMachineReferences>();
		}

		public override void Update()
		{
			_references.transform.Rotate(0, 0, 1);
		}

		public override void Exit()
		{
			
		}
	}
}