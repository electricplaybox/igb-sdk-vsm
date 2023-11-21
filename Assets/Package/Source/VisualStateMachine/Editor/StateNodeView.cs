using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor
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
			CreateRelayNode();
		}

		private void CreateRelayNode()
		{
			if (Data.State is not Relay) return;
			var relayState = Data.State as Relay;
			
			var title = this.Q<VisualElement>("title");
			if (title == null) return;
			
			title.parent.Remove(title);
			
			var propertyContainer = this.Q<VisualElement>("property-container");
			if(propertyContainer == null) return;

			// propertyContainer.style.width = 120;
			
			var label = new Label();
			// label.text = ">>";
			
			switch (relayState.Direction)
			{
				case RelayDirection.Left:
					label.text = ">>";
					break;
				case RelayDirection.Right:
					label.text = "<<";
					var output = this.Q<VisualElement>("output");
					output.parent.Insert(0,output);
					
					var input = this.Q<VisualElement>("input");
					input.parent.Add(input);
					break;
			}
			
			label.style.unityTextAlign = TextAnchor.MiddleCenter;
			label.style.unityFontStyleAndWeight = FontStyle.Bold;
			propertyContainer.Add(label);
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