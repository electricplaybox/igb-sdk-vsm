using System;
using UnityEngine;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace Testing.NodeAttributes
{
	public class SmallNode : State
	{
		[Transition] public event Action OnComplete;
		
		[SerializeField] private ScriptableObject _myReallyLongSerializedFieldLabel;
		
		public override void EnterState()
		{
			
		}

		public override void ExitState()
		{
			
		}
	}
}