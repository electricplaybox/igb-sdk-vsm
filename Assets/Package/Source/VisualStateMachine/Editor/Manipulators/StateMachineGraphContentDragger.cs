using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor.Manipulators
{
	public class StateMachineGraphContentDragger : ContentDragger
	{
		public event Action<Vector3> OnDrag;
		
		protected override void RegisterCallbacksOnTarget()
		{
			base.RegisterCallbacksOnTarget();
			target.RegisterCallback(new EventCallback<MouseMoveEvent>(HandleMouseMove));
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.UnregisterCallbacksFromTarget();
			target.UnregisterCallback(new EventCallback<MouseMoveEvent>(HandleMouseMove));
		}

		private void HandleMouseMove(MouseMoveEvent evt)
		{
			if (this.target is not GraphView target) return;
			
			OnDrag?.Invoke(target.viewTransform.position);
		}
	}
}