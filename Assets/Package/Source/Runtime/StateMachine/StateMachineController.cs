using UnityEngine;

namespace Package.Source.Runtime.StateMachine
{
	public class StateMachineController : MonoBehaviour
	{
		public StateMachineGraph GraphData
		{
			get => _stateMachineInstance != null ? _stateMachineInstance : _stateMachine;
			set => _stateMachine = value;
		}
		
		[SerializeField, HideInInspector]
		private StateMachineGraph _stateMachine;
		private StateMachineGraph _stateMachineInstance;

		public void Start()
		{
			_stateMachineInstance = Instantiate(_stateMachine);
			_stateMachineInstance.name = _stateMachine.name + _stateMachineInstance.GetInstanceID();
			_stateMachineInstance.Initialize();
		}

		public void Update()
		{
			if (_stateMachineInstance == null) return;
			
			_stateMachineInstance.Update();
		}
	}
}