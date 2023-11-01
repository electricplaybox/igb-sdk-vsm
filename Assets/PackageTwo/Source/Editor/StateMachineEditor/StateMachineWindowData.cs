using StateMachine;
using UnityEngine;

namespace Editor.StateMachineEditor
{
	public class StateMachineWindowData : ScriptableObject
	{
		public StateMachineGraph StateMachineGraph => _stateMachineGraph = _stateMachineController != null 
			? _stateMachineController.GraphData 
			: _stateMachineGraph;
		
		public void SetStateMachineController(StateMachineController controller)
		{
			_stateMachineController = controller;
		}

		public void SetStateMachineGraph(StateMachineGraph graph)
		{
			_stateMachineGraph = graph;

			if (_stateMachineController == null) return;
			if (_stateMachineController.GraphData == _stateMachineGraph) return;

			_stateMachineController = null;
		}
		
		private StateMachineGraph _stateMachineGraph;
		private StateMachineController _stateMachineController;
	}
}