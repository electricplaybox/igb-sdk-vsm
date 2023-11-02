using System;
using System.Collections.Generic;
using System.Reflection;
using StateMachine.Attributes;
using UnityEngine;

namespace StateMachine
{
	[Serializable]
	public class StateNode
	{
		public string Id;
		public string StateType;
		public bool EntryPoint;
		public IReadOnlyList<StateConnection> Connections => _connections;
		
		[SerializeField]
		private List<StateConnection> _connections = new ();
		private State _state;
		
		public void Initialize()
		{
			var type = Type.GetType(StateType);
			if (type == null) return;
			
			_state = Activator.CreateInstance(type) as State;
		}
		
		public void AddConnection(StateConnection connection)
		{
			_connections.Add(connection);
		}

		public void Enter()
		{
			SubscribeToConnections();
			_state.Enter();
		}

		public void Update()
		{
			_state.Update();
		}

		public void Exit()
		{
			UnsubscribeFromConnections();
			_state.Exit();
		}

		private void SubscribeToConnections()
		{
			foreach (var connection in _connections) 
			{
				SubscribeToEventByName(_state, connection.FromEventName, connection);
			}
		}
		
		private void UnsubscribeFromConnections()
		{
			foreach (var connection in _connections) 
			{
				UnsubscribeFromEventByName(_state, connection.FromEventName, connection);
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