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

		public StateNodeView(StateNode stateNode, string stateTitle, string stateName, StateMachineGraphView graphView)
		{
			Data = stateNode;
			this.title = stateTitle;
			this.name = stateNode.Id;
			
			if(stateNode.State is not EntryState) StateMachineNodeFactory.CreateInputPort(this, graphView);
			StateMachineNodeFactory.CreateOutputPorts(this, graphView);
			
			this.RefreshPorts();
			this.RefreshExpandedState();
			this.SetPosition(new Rect(stateNode.Position, Vector2.one));
			
			var container = new VisualElement();
			container.name = "title-container";
			
			var title = this.Query<VisualElement>("title").First();
			var titleLabel = title.Query<VisualElement>("title-label").First();
			var titleButton = title.Query<VisualElement>("title-button-container").First();
			title.Remove(titleButton);
			
			var icon = new Image();
			icon.name = "title-icon";
			icon.scaleMode = ScaleMode.ScaleToFit;
			
			title.Add(container);
			container.Add(icon);
			container.Add(titleLabel);

			var progressBar = new ProgressBar();
			progressBar.name = "progress-bar";
			title.Add(progressBar);
			
			var contents = this.Query<VisualElement>("contents").First();
			var propertyContainer = new VisualElement()
			{
				name = "property-container"
			};
			
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);

			if (stateNode.State != null)
			{
				var stateInspector = StateMachineNodeFactory.CreateUIElementInspector(stateNode.State);
				stateInspector.name = "state-inspector";
				propertyContainer.Add(stateInspector);
				
				if (stateInspector.childCount > 0)
				{
					propertyContainer.AddToClassList("has-properties");
				}
				
				//adjust the node size
				var stateType = stateNode.State.GetType();
				var nodeWidth = stateType.GetCustomAttribute<NodeWidthAttribute>();
				if (nodeWidth != null)
				{
					propertyContainer.style.width = nodeWidth.Width;
				}
			}
		}

		public virtual void Update()
		{
			if (Data == null) return;
			
			DrawEntryPoint();
			DrawCustomNodeColor();
			DrawActiveNode();
			SetCustomLabelText();
			CreateCustomIcon();
			CreateRelayNode();
		}

		private void CreateVerticalRelayNode()
		{
			if (Data.State is not Relay) return;
			var relayState = Data.State as Relay;
			
			this.AddToClassList("relay-node");
			
			var title = this.Q<VisualElement>("title");
			if (title == null) return;
			
			title.parent.Remove(title);
			
			var propertyContainer = this.Q<VisualElement>("property-container");
			if(propertyContainer == null) return;
			
			propertyContainer.parent.Remove(propertyContainer);

			var contents = this.Q("contents");
			var top = contents.Q("top");
			
			var input = contents.Q("input");
			var inputPort = input.Q<Port>();
			inputPort.style.flexDirection = FlexDirection.Column;
			
			var output = contents.Q("output");
			var outputPort = output.Q<Port>();
			outputPort.style.flexDirection = FlexDirection.ColumnReverse;
			
			var contentDivider = contents.Q("divider");
			contentDivider.parent.Remove(contentDivider);
			
			var topDivider = top.Q("divider");
			topDivider.parent.Remove(topDivider);
			
			var bottom = new VisualElement();
			bottom.name = "bottom";
			bottom.Insert(0, inputPort);
			contents.Insert(0, bottom);
			
			var topInput = top.Q("input");
			top.Remove(topInput);
		}
		
		private void CreateRelayNode()
		{
			


			//var label = new Label();

			// switch (relayState.Direction)
			// {
			// 	case RelayDirection.Right:
			// 		label.text = ">>";
			// 		break;
			// 	case RelayDirection.Left:
			// 		label.text = "<<";
			// 		var output = this.Q<VisualElement>("output");
			// 		output.parent.Insert(0,output);
			//
			// 		var outputPort = output.Q<Port>();
			// 		outputPort.style.flexDirection = FlexDirection.Row;
			// 		
			// 		var input = this.Q<VisualElement>("input");
			// 		input.parent.Add(input);
			//
			// 		var inputPort = input.Q<Port>();
			// 		inputPort.style.flexDirection = FlexDirection.RowReverse;
			// 		break;
			// }
			//
			// label.style.unityTextAlign = TextAnchor.MiddleCenter;
			// label.style.unityFontStyleAndWeight = FontStyle.Bold;
			// propertyContainer.Add(label);
		}

		private void CreateCustomIcon()
		{
			var stateType = Data.State.GetType();
			var image = titleContainer.Q<Image>("title-icon");
			
			var nodeIcon = AttributeUtils.GetInheritedCustomAttribute<NodeIconAttribute>(stateType);
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
			var color = ColorUtils.HexToColor(NodeColor.Grey);
			
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
		
		public override Port InstantiatePort(
			Orientation orientation,
			Direction direction,
			Port.Capacity capacity,
			System.Type type)
		{
			return StatePort.Create<Edge>(orientation, direction, capacity, type);
		}
	}
}