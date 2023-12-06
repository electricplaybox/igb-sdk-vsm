using System;
using UnityEngine;
using VisualStateMachine.Tools;

namespace VisualStateMachine
{
	public class StateMachineController : MonoBehaviour
	{
		public event Action OnComplete;
		
		public StateMachine StateMachine => _stateMachineCore?.StateMachine;
		
		[SerializeField] private StateMachine _stateMachine;
		
		private StateMachineCore _stateMachineCore;

		public void Awake()
		{
			DevLog.Log("StateMachineController.Awake", this);
			CreateCore();
		}

		public void OnValidate()
		{
			DevLog.Log("StateMachineController.OnValidate", this);
			CreateCore();
		}

		private void CreateCore()
		{
			DevLog.Log("StateMachineController.CreateCore", this);
			if(_stateMachine == null) return;
			if (_stateMachineCore != null) return;
			
			_stateMachineCore = new StateMachineCore(_stateMachine, this);
		}

		public void Start()
		{
			DevLog.Log("StateMachineController.Start", this);
			_stateMachineCore.OnComplete += HandleComplete;
			_stateMachineCore.Start();
		}

		public void Update()
		{
			DevLog.Log("StateMachineController.Update", this);
			_stateMachineCore.Update();
		}

		public void Complete()
		{
			DevLog.Log("StateMachineController.Complete", this);
			_stateMachineCore.Complete();
		}

		public void OnDestroy()
		{
			DevLog.Log("StateMachineController.OnDestroy", this);
			_stateMachineCore.OnComplete -= HandleComplete;
		}

		private void HandleComplete()
		{
			DevLog.Log("StateMachineController.HandleComplete", this);
			OnComplete?.Invoke();
		}
	}
}