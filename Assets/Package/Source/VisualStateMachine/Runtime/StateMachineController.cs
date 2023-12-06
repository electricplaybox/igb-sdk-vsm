using UnityEngine;

namespace VisualStateMachine
{
	public class StateMachineController : MonoBehaviour
	{
		public StateMachine StateMachine => _stateMachineCore?.StateMachine;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachineCore _stateMachineCore;

		public void Awake()
		{
			CreateCore();
		}

		public void OnValidate()
		{
			CreateCore();
		}
		
		public void Start()
		{
			_stateMachineCore.Start();
		}

		public void Update()
		{
			if (StateMachine.IsComplete) return;
			
			_stateMachineCore.Update();
		}

		private void CreateCore()
		{
			if(_stateMachine == null) return;
			if (_stateMachineCore != null) return;
			
			_stateMachineCore = new StateMachineCore(_stateMachine, this);
		}
	}
}