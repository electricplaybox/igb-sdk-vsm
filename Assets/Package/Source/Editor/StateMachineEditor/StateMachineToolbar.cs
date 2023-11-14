using System;
using StateMachine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor.StateMachineEditor
{
	public class StateMachineToolbar : Toolbar
	{
		public event Action OnSave;
		public event Action<StateMachineGraph> OnGraphChanged;
		
		private ObjectField _graphDataField;
		private Button _saveButton;

		public StateMachineToolbar(StateMachineGraph graph)
		{
			this.RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			_graphDataField = new ObjectField("State machine graph:");
			_graphDataField.objectType = typeof(StateMachineGraph);
			_graphDataField.SetValueWithoutNotify(graph);
			_graphDataField.MarkDirtyRepaint();
			_graphDataField.RegisterValueChangedCallback(HandleGraphChanged);
			Add(_graphDataField);
			
			_saveButton = new Button();
			_saveButton.text = "Save";
			_saveButton.clicked += HandleSave;
			Add(_saveButton);
		}

		private void HandleSave()
		{
			OnSave?.Invoke();
		}

		private void HandleGraphChanged(ChangeEvent<Object> evt)
		{
			OnGraphChanged?.Invoke(evt.newValue as StateMachineGraph);
		}

		private void OnDestroy()
		{
			_graphDataField.UnregisterValueChangedCallback(HandleGraphChanged);
			_saveButton.clicked -= HandleSave;
		}
	}
}