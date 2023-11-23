using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualStateMachine.Editor
{
	public class StateMachineGraphContentDragger : ContentDragger
	{
		public event Action<Vector3> OnDrag;
		
		protected override void RegisterCallbacksOnTarget()
		{
			Debug.Log("TARGET GRABBED");
			base.RegisterCallbacksOnTarget();
			this.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(HandleMouseMove));
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			base.UnregisterCallbacksFromTarget();
			this.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(HandleMouseMove));
		}

		private void HandleMouseMove(MouseMoveEvent evt)
		{
			if (this.target is not UnityEditor.Experimental.GraphView.GraphView target) return;
			
			OnDrag?.Invoke(target.viewTransform.position);
		}
	}
}