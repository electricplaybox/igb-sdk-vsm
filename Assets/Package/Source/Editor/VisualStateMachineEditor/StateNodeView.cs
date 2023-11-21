using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine;
using System.Reflection;
using UnityEditor;
using VisualStateMachine.Attributes;

namespace Editor.VisualStateMachineEditor
{
	public class StateNodeView : Node
	{
		public StateNode Data;
		public StateMachine StateMachine;

		public void Update()
		{
			if (Data == null) return;
			
			DrawEntryPoint();
			DrawCustomNodeColor();
			DrawActiveNode();
			SetCustomLabelText();
			CreateCustomIcon();
		}

		private void CreateCustomIcon()
		{
			var stateType = Data.State.GetType();
			
			var nodeIcon = stateType.GetCustomAttribute<NodeIcon>();
			if (nodeIcon == null) return;

			var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(nodeIcon.Path);
			if (icon == null) return;

			var image = titleContainer.Q<Image>("title-icon");
			if (image == null) return;
			
			image.image = icon;
		}

		private void SetCustomLabelText()
		{
			var stateType = Data.State.GetType();
			
			var nodeLabel = stateType.GetCustomAttribute<NodeLabel>();
			if (nodeLabel == null) return;
			
			titleContainer.Q<Label>().text = nodeLabel.Text;
		}

		private void DrawCustomNodeColor()
		{
			var stateType = Data.State.GetType();
			
			var nodeColor = stateType.GetCustomAttribute<NodeColor>();
			if (nodeColor == null) return;

			var color = ColorUtils.HexToColor(nodeColor.HexColor);
			titleContainer.style.backgroundColor = color;

			var selectionBorder = this.Q("selection-border");
			selectionBorder.style.borderBottomColor = color;
			selectionBorder.style.borderTopColor = color;
			selectionBorder.style.borderLeftColor = color;
			selectionBorder.style.borderRightColor = color;
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
			if (Data.IsEntryNode)
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