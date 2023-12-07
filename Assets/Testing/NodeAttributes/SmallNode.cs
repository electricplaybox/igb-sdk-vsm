using UnityEngine;
using VisualStateMachine.States;

namespace Testing.NodeAttributes
{
	public class SmallNode : State
	{
		[SerializeField] private ScriptableObject _myReallyLongSerializedFieldLabel;
		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}