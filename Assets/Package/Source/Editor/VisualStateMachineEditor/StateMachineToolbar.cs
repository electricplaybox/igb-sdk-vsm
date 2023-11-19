using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VisualStateMachine;
using Object = UnityEngine.Object;

namespace Editor.VisualStateMachineEditor
{
	public class StateMachineToolbar : Toolbar
	{
		public event Action OnSave;
		public event Action<StateMachine> OnStateMachineChanged;
		
		private ObjectField _graphDataField;
		private Button _saveButton;

		public StateMachineToolbar(StateMachine stateMachine)
		{
			this.RegisterCallback<DetachFromPanelEvent>(evt => OnDestroy());
			
			_graphDataField = new ObjectField("State machine graph:");
			_graphDataField.objectType = typeof(StateMachine);
			_graphDataField.SetValueWithoutNotify(stateMachine);
			_graphDataField.MarkDirtyRepaint();
			_graphDataField.RegisterValueChangedCallback(HandleGraphChanged);
			Add(_graphDataField);
			
			_saveButton = new Button();
			_saveButton.text = "Save";
			_saveButton.clicked += HandleSave;
			Add(_saveButton);
		}
		
		public void Update(StateMachine stateMachine)
		{
			_graphDataField.SetValueWithoutNotify(stateMachine);
		}

		private void HandleSave()
		{
			OnSave?.Invoke();
		}

		private void HandleGraphChanged(ChangeEvent<Object> evt)
		{
			OnStateMachineChanged?.Invoke(evt.newValue as StateMachine);
		}

		private void OnDestroy()
		{
			_graphDataField.UnregisterValueChangedCallback(HandleGraphChanged);
			_saveButton.clicked -= HandleSave;
		}
	}
}