using System.Linq;
using StateMachine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Editor.StateMachineEditor
{
	public class StateMachineReadWrite
	{
		public static void SaveGraph(StateMachineGraph data, GraphView graph)
		{
			data.Clear();
			
			foreach (var node in graph.nodes)
			{
				if (node is not StateNodeView) continue;
				
				var stateNodeView = node as StateNodeView;
				stateNodeView.Data.Position = node.GetPosition().position;

				if (stateNodeView.Data.EntryPoint)
				{
					data.EntryNodeId = stateNodeView.Data.Id;
				}
				
				var edges = graph.edges.Where(edge => edge.output.node == node);
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
				
				data.AddNode(stateNodeView.Data);
			}
			
			EditorUtility.SetDirty(data);
			AssetDatabase.SaveAssets();
		}
	}
}