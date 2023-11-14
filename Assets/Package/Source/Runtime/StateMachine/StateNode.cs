using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
	[Serializable]
	public class StateNode
	{
		public string Id;
		public string StateType;
		public bool EntryPoint;
		public Vector2 Position;
		public string Title;
		public IReadOnlyList<StateConnection> Connections => _connections;
		
		[field: NonSerialized]
		public State State { get; set; }
		
		[field: NonSerialized]
		public bool IsActive { get; set; }

		[SerializeField]
		private List<StateConnection> _connections = new ();

		[SerializeField] 
		private string _stateJson;
		
		public StateNode(string id, Type type, bool isEntryPoint, Vector2 position)
		{
			Id = id;
			StateType = type.AssemblyQualifiedName;
			EntryPoint = isEntryPoint;
			Title = type.Name;
			Position = position;
			
			Load();
		}

		public void Load(State state = null)
		{
			if (state != null)
			{
				State = state;
				return;
			}
			
			var type = Type.GetType(StateType);
			State = ScriptableObject.CreateInstance(type) as State;
			State.name = Id;
		}
		
		public void AddConnection(StateConnection connection)
		{
			_connections.Add(connection);
		}

		public void Enter()
		{
			SubscribeToConnections();
			IsActive = true;
			State.Enter();
		}

		public void Update()
		{
			State.Update();
		}

		public void Exit()
		{
			UnsubscribeFromConnections();
			IsActive = false;
			State.Exit();
		}

		private void SubscribeToConnections()
		{
			foreach (var connection in _connections) 
			{
				SubscribeToEventByName(State, connection.FromPortName, connection);
			}
		}
		
		private void UnsubscribeFromConnections()
		{
			foreach (var connection in _connections) 
			{
				UnsubscribeFromEventByName(State, connection.FromPortName, connection);
			}
		}
		
		private void SubscribeToEventByName(object targetObject, string eventName, StateConnection connection)
		{
			var eventInfo = targetObject.GetType().GetEvent(eventName);
			if (eventInfo == null) return;
			
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, connection, nameof(connection.Transition));
			eventInfo.AddEventHandler(targetObject, handler);
		}
		
		private void UnsubscribeFromEventByName(object targetObject, string eventName, StateConnection connection)
		{
			var eventInfo = targetObject.GetType().GetEvent(eventName);
			if (eventInfo == null) return;
			
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, connection, nameof(connection.Transition));
			eventInfo.RemoveEventHandler(targetObject, handler);
		}

		public void Clear()
		{
			_connections.Clear();
		}
	}
}