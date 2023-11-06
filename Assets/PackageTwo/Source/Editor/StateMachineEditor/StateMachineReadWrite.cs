using System.Linq;
using StateMachine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Editor.StateMachineEditor
{
	public class StateMachineReadWrite
	{
		public static void SaveGraph(StateMachineGraph graph, GraphView graphView)
		{
			graph.Clear();
			
			foreach (var node in graphView.nodes)
			{
				if (node is not StateNodeView) continue;
				
				var stateNodeView = node as StateNodeView;
				stateNodeView.Data.Position = node.GetPosition().position;

				if (stateNodeView.Data.EntryPoint)
				{
					graph.EntryNodeId = stateNodeView.Data.Id;
				}
				
				var edges = graphView.edges.Where(edge => edge.output.node == node);
				foreach (var edge in edges)
				{
					var connection = new StateConnection()
					{
						FromPortName = edge.output.portName,
						ToPortName = edge.input.portName,
						FromNodeId = stateNodeView.Data.Id,
						ToNodeId = (edge.output.connections.First().input.node as StateNodeView).Data.Id
					};
						
					stateNodeView.Data.AddConnection(connection);
				}
				
				graph.AddNode(stateNodeView.Data);
			}
			
			EditorUtility.SetDirty(graph);
			AssetDatabase.SaveAssets();
			
			graph.Save();
		}
	}
}