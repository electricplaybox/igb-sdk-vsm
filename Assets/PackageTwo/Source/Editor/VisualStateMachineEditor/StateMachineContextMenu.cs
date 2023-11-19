using System;
using Package.Source.Editor.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine;

namespace Editor.VisualStateMachineEditor
{
	public class StateMachineContextMenu
	{
		public event Action<StateNodeView> OnDeleteStateNode;
		public event Action<Type, Vector2> OnCreateNewStateNode;
		public event Action<StateNodeView> OnSetAsEntryNode;
		
		private readonly StateMachineGraphView _graphView;

		public StateMachineContextMenu(StateMachineGraphView graphView)
		{
			_graphView = graphView;
			_graphView.RegisterCallback((ContextualMenuPopulateEvent evt) =>
			{
				switch (evt.target.GetType().Name)
				{
					case "StateMachineGraphView":
						BuildGraphViewContext(evt);
						break;
					case "StateNodeView":
						BuildNodeContext(evt);
						break;
				}
			});
		}

		private void BuildNodeContext(ContextualMenuPopulateEvent evt)
		{
			var node = evt.target as StateNodeView;

			evt.menu.AppendAction("Set as Entry Node", x =>
			{
				OnSetAsEntryNode?.Invoke(node);
			});

			evt.menu.AppendAction("Delete", x =>
			{
				OnDeleteStateNode?.Invoke(node);
			});
		}

		private void BuildGraphViewContext(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendSeparator();
			var states = AssetUtils.GetAllDerivedTypes<State>();

			foreach (var stateType in states)
			{
				evt.menu.AppendAction(stateType.Name, action =>
				{
					var state = stateType;
					OnCreateNewStateNode?.Invoke(state, action.eventInfo.mousePosition);
				});
			}
		}
	}
}