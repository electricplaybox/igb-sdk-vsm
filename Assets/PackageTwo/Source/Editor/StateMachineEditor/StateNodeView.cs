using System;
using System.Reflection;
using StateMachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.StateMachineEditor
{
	public class StateNodeView : Node
	{
		public StateNode Data;

		public StateNodeView()
		{
			DisplayStateProperties();
		}

		public void Update()
		{
			if (Data == null) return;
			
			DrawEntryPoint();
			DrawActiveNode();
		}
		
		private void DisplayStateProperties()
		{
			if (Data == null) return;

			var type = Type.GetType(Data.StateType);
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var field in fields)
			{
				if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null) continue;
				
				var nameLabel = new Label(field.Name);
				Add(nameLabel);

				var valueField = new TextField();
				valueField.value = field.GetValue(type)?.ToString();
				Add(valueField);
			}
		}

		private void DrawActiveNode()
		{
			if (!Application.isPlaying) return;
			
			if (Data.IsActive)
			{
				AddToClassList("active-node");
				this.Query<ProgressBar>("progress-bar").First().value = (Time.time % 1f) * 100f;
			}
			else
			{
				RemoveFromClassList("active-node");
			}
		}

		private void DrawEntryPoint()
		{
			if (Data.EntryPoint)
			{
				AddToClassList("entry-node");
			}
			else
			{
				RemoveFromClassList("entry-node");
			}
		}
	}
}