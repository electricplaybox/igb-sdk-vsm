using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualStateMachine.Attributes;
using VisualStateMachine.States;

namespace VisualStateMachine.Editor
{
	public class CustomNodeView : StateNodeView
	{
		public CustomNodeView(StateNode stateNode, string stateTitle, string stateName, StateMachineGraphView graphView) : base(stateNode, stateTitle, stateName, graphView)
		{
		}
	}
}