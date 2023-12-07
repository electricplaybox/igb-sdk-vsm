using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VisualStateMachine.Editor.Utils;
using VisualStateMachine.States;
using VisualStateMachine.Tools;
using Object = System.Object;

namespace VisualStateMachine
{
	[Serializable]
	public class GraphViewState
	{
		public Vector3 Position = Vector3.zero;
		public float Scale = 0f;
	}
	
	[CreateAssetMenu(fileName = "StateMachine", menuName = "StateMachine/StateMachine")]
	public class StateMachine : ScriptableObject
	{
		public GraphViewState GraphViewState => _graphViewState ??= new GraphViewState();
		public State CurrentState => _currentNode?.State ?? null;
		
		public StateMachine Base { get; set; }
		public IReadOnlyCollection<StateNode> Nodes => _nodes;
		public bool IsComplete { get; private set; }

		[SerializeField] 
		private string _entryStateId;

		[SerializeField] 
		private GraphViewState _graphViewState;
		
		[SerializeField]
		private List<StateNode> _nodes = new();
		
		[NonSerialized]
		private Dictionary<string, StateNode> _nodeLookup = new();

		[NonSerialized]
		private StateNode _currentNode;
		
		[SerializeField, HideInInspector]
		private List<UnityEngine.Object> _subAssets = new();

		public static StateMachine CreateInstance(StateMachine stateMachine)
		{
			DevLog.Log("StateMachine.CreateInstance");
			var instance = Instantiate(stateMachine);
			instance.name = instance.name + instance.GetInstanceID();
			instance.Base = stateMachine;
			
			return instance;
		}

		public void UpdateGraphViewState(Vector3 position, float scale)
		{
			_graphViewState ??= new GraphViewState();
			_graphViewState.Position = position;
			_graphViewState.Scale = scale;
		}
		
		public void Initialize(StateMachineCore stateMachineCore)
		{
			DevLog.Log("StateMachine.Initialize");
			
			InitalizeNodes(stateMachineCore);
			CreateNodeLookupTable();
			
			if(_nodeLookup.Count == 0) throw new Exception($"StateMachine {this.name} has 0 nodes.");
			
			_currentNode = _nodeLookup[_entryStateId];
			if (_currentNode == null) return;
			
			SubscribeToNode(_currentNode);
		}

		public void Start()
		{
			DevLog.Log("StateMachine.Start");
			_currentNode.Enter();
		}
		
		public void Update()
		{
			DevLog.Log("StateMachine.Update");
			_currentNode?.Update();
		}

		public void Complete()
		{
			IsComplete = true;
		}
	
		public void AddEntryNode()
		{
			DevLog.Log("StateMachine.AddEntryNode");
			
			#if UNITY_EDITOR
			{
				if (!AssetDatabase.Contains(this)) return;
				if (_nodes.Count > 0) return;

				var stateNode = new StateNode(typeof(EntryState));
				stateNode.SetAsEntryNode(true);
			
				_entryStateId = stateNode.Id;
			
				AddNode(stateNode);
			}
			#endif
		}

		private void InitalizeNodes(StateMachineCore stateMachineCore)
		{
			DevLog.Log("StateMachine.InitalizeNodes");
			
			foreach (var node in _nodes)
			{
				node.Initialize(stateMachineCore);
			}
		}

		private void CreateNodeLookupTable()
		{
			DevLog.Log("StateMachine.CreateNodeLookupTable");
			
			_nodeLookup.Clear();

			foreach (var node in _nodes)
			{
				_nodeLookup.Add(node.Id, node);
			}
		}

		#region StateMachine Management
		
		private void SubscribeToNode(StateNode node)
		{
			DevLog.Log("StateMachine.SubscribeToNode");
			
			var connections = node.Connections;

			foreach (var connection in node.Connections)
			{
				connection.OnTransition += OnTransition;
			}
		}
		
		private void Unsubscribe(StateNode node)
		{
			DevLog.Log("StateMachine.Unsubscribe");
			
			var connections = node.Connections;

			foreach (var connection in node.Connections)
			{
				connection.OnTransition -= OnTransition;
			}
		}

		private void OnDestroy()
		{
			DevLog.Log("StateMachine.OnDestroy");
			if (_currentNode == null) return;
			
			Unsubscribe(_currentNode);
		}

		private void OnTransition(StateConnection connection)
		{
			DevLog.Log("StateMachine.OnTransition");
			var nextNode = _nodeLookup[connection.ToNodeId];
			
			Unsubscribe(_currentNode);
			_currentNode.Exit();
			
			_currentNode = nextNode;
			if (_currentNode == null) return;
			
			SubscribeToNode(_currentNode);
			_currentNode.Enter();
		}
		
		#endregion

		#region Node Management
		
		public void AddNode(StateNode node)
		{
			DevLog.Log("StateMachine.AddNode");
			_nodes.Add(node);
			
			#if UNITY_EDITOR
			{
				_subAssets.Add(node.State);
				AssetDatabase.AddObjectToAsset(node.State, this);
				AssetDatabase.SaveAssets();
			}
			#endif
		}
		
		public void RemoveNodeById(string nodeId)
		{
			var node = _nodes.FirstOrDefault(node => node.Id == nodeId);
			if (node == null) return;

			RemoveNode(node);
		}
		
		public void RemoveNode(StateNode node)
		{
			Debug.Log($"StateMachine.RemoveNode {node.Id}");
			var subAsset = _subAssets.FirstOrDefault(subAsset => subAsset.name == node.Id);
			if (subAsset == null) return;
			
			#if UNITY_EDITOR
			{
				var isEntryNode = _entryStateId == node.Id;
				
				_subAssets.Remove(subAsset);
				AssetDatabase.RemoveObjectFromAsset(subAsset);
				
				RemoveConnectionsToNode(node.Id);
				
				_nodes.Remove(node);
				Save();
				
				if(isEntryNode) SelectNextEntryNode();
			}
			#endif
		}

		public void RemoveConnectionsToNode(string nodeId)
		{
			DevLog.Log("StateMachine.RemoveConnectionsToNode");
			foreach (var node in _nodes)
			{
				node.RemoveAll(connection => connection.ToNodeId == nodeId);
			}
		}

		public void RemoveConnection(string fromNodeId, string toNodeId)
		{
			DevLog.Log("StateMachine.RemoveConnection");
			
			var fromNode = _nodes.FirstOrDefault(node => node.Id == fromNodeId);
			if (fromNode == null)
			{
				Debug.LogError($"Attempting to remove connection between node {fromNodeId} to {toNodeId} failed as the fromNode is not present");
				return;
			}
			
			fromNode.RemoveAll(connection => connection.ToNodeId == toNodeId);
		}

		public void SetEntryNode(StateNode node)
		{
			DevLog.Log("StateMachine.SetEntryNode");
			
			if (!_nodes.Contains(node)) throw new Exception("Node is not part of this state machine");
			
			SetEntryNodeId(node.Id);
		}

		private void SetEntryNodeId(string entryNodeId)
		{
			DevLog.Log("StateMachine.SetEntryNodeId");
			_entryStateId = entryNodeId;

			foreach (var node in _nodes)
			{
				node.SetAsEntryNode(node.Id == entryNodeId);
			}
			
			Save();
		}

		public void Save()
		{
			DevLog.Log("StateMachine.Save");
			ValidateSubAssets();
			
			#if UNITY_EDITOR
			{
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssetIfDirty(this);
			}
			#endif
		}
		
		private void SelectNextEntryNode()
		{
			DevLog.Log("StateMachine.SelectNextEntryNode");
			
			_entryStateId = _nodes.Count > 0 ? _nodes[0].Id : null;
			Save();
		}
		
		public void ValidateSubAssets() 
		{
			var thisPath = AssetDatabase.GetAssetPath(this);
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(thisPath);
 
			for (var i = subAssets.Length - 1; i >= 0; i--) 
			{
				if(subAssets[i] != null) continue;
             
				AssetDatabase.RemoveObjectFromAsset(_subAssets[i]);
				_subAssets.RemoveAt(i);
			}
         
			AssetDatabase.SaveAssets();
		}

		public void RemoveAllNodes()
		{
			DevLog.Log("StateMachine.RemoveAllNodes");
			
			#if UNITY_EDITOR
			{
				foreach (var subAsset in _subAssets)
				{
					AssetDatabase.RemoveObjectFromAsset(subAsset);
				}
				
				_nodes.Clear();
				_subAssets.Clear();
				
				Save();
			}
			#endif
		}
		
		#endregion
	}
}