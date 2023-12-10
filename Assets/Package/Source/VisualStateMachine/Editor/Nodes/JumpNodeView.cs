using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor.Nodes
{
	public class JumpNodeView : NodeView
	{
		private readonly JumpState _jumpState;
		private PopupField<string> _idDropdown;

		public JumpNodeView(StateNode stateNode, StateMachineGraphView graphView) : base(stateNode, graphView)
		{
			if (stateNode.State == null) return;

			_jumpState = stateNode.State as JumpState;
			
			RemoveTitle();
			AddProperties();
			
			this.AddToClassList("jump-node");
		}
		
		private void AddProperties()
		{
			if (Data.State == null) return;
			
			var top = this.Q<VisualElement>("top");
			top.style.flexDirection = Data.State is JumpOutState ? FlexDirection.Row : FlexDirection.RowReverse;

			var ids = GetIds();
			var currentId = _jumpState.JumpId.ToString();
			if (!ids.Contains(currentId))
			{
				if (ids.Count > 0)
				{
					_jumpState.JumpId = (JumpId) Enum.Parse(typeof(JumpId), ids[0]);
					currentId = ids[0];
				}
			}
			
			_idDropdown = new PopupField<string>("", ids, currentId);
			_idDropdown.AddToClassList("center-aligned-text");
			_idDropdown.RegisterValueChangedCallback(evt =>
			{
				HandleIdValueChanged(evt.newValue);
			});

			HandleIdValueChanged(_idDropdown.value);
			
			top.Add(_idDropdown);
		}

		private JumpId GetIdAtIndex(int index)
		{
			var values = Enum.GetValues(typeof(JumpId));
			if (index >= 0 && index < values.Length)
			{
				return (JumpId) values.GetValue(index);
			}
			else
			{
				throw new StateMachineException( "Maximum number of JumpOut states in use.");
			}
		}

		private List<string> GetIds()
		{
			var ids = Enum.GetNames(typeof(JumpId)).ToList();
			var isOutput = Data.State is JumpOutState;
			if (isOutput) return ids;
			
			//filter out used ids;
			var jumpNodes = GraphView.Query<JumpNodeView>().ToList();
			foreach(var node in jumpNodes)
			{
				if (node.Data == null) continue;
				if (node.Data.State is JumpInState)
				{
					var jumpIn = node.Data.State as JumpInState;
					ids.Remove(jumpIn.JumpId.ToString());
				}
			}
			
			return ids;
		}

		private void HandleIdValueChanged(string value)
		{
			_jumpState.JumpId = (JumpId) Enum.Parse(typeof(JumpId), value);
			
			var jumpIndex = (int)_jumpState.JumpId;
			var maxIndex = Enum.GetValues(typeof(JumpId)).Length;
			var color = ColorUtils.GetColorFromValue(jumpIndex, maxIndex, 0.5f);
			_idDropdown.Children().First().style.backgroundColor = color;
			
			AssetDatabase.SaveAssets();
		}
	}
}