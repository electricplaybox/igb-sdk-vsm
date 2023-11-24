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

			propertyContainer.AddToClassList("relay");
			
			var label = new Label();
			
			switch (relayState.Direction)
			{
				case RelayDirection.Right:
					label.text = ">>";
					break;
				case RelayDirection.Left:
					label.text = "<<";
					var output = this.Q<VisualElement>("output");
					output.parent.Insert(0,output);

					var outputPort = output.Q<Port>();
					outputPort.style.flexDirection = FlexDirection.Row;
					
					var input = this.Q<VisualElement>("input");
					input.parent.Add(input);

					var inputPort = input.Q<Port>();
					inputPort.style.flexDirection = FlexDirection.RowReverse;
					break;
			}
			
			label.style.unityTextAlign = TextAnchor.MiddleCenter;
			label.style.unityFontStyleAndWeight = FontStyle.Bold;
			propertyContainer.Add(label);
		}

		private void CreateCustomIcon()
		{
			var stateType = Data.State.GetType();
			var image = titleContainer.Q<Image>("title-icon");
			
			var nodeIcon = stateType.GetCustomAttribute<NodeIconAttribute>();
			if (nodeIcon == null)
			{
				if(image != null) image.parent.Remove(image);
				return;
			}

			var icon = nodeIcon.FetchTexture();
			if (icon == null)
			{
				if(image != null) image.parent.Remove(image);
				return;
			}
			
			if (image == null) return;
			image.image = icon;
		}

		private void SetCustomLabelText()
		{
			var stateType = Data.State.GetType();
			
			var nodeLabel = stateType.GetCustomAttribute<NodeLabelAttribute>();
			if (nodeLabel == null) return;
			
			titleContainer.Q<Label>().text = nodeLabel.Text;
		}

		private void DrawCustomNodeColor()
		{
			var stateType = Data.State.GetType();
			
			var nodeColor = AttributeUtils.GetInheritedCustomAttribute<NodeColorAttribute>(stateType);
			var color = ColorUtils.HexToColor(NodeColor.Tundora);
			
			if (nodeColor != null)
			{
				color = ColorUtils.HexToColor(nodeColor.HexColor);
			}

			color.a = 0.8f;
			
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