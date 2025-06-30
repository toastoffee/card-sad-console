using System;
using System.Collections;
using System.Collections.Generic;

public class LiteStateEngine {
	private List<LiteState> m_registerStates = new();
	private Dictionary<Type, LiteState> m_stateDict = new();
	private Stack<LiteState> m_stack = new();
	public LiteState frontState => m_stack.Count > 0 ? m_stack.Peek() : null;

	public LiteStateEngine(List<LiteState> states) {
		m_registerStates = new(states);
		Awake();
	}

	private void Awake() {
		InitStateEngine();
		if (m_registerStates.Count == 0) {
			return;
		}
		var defaultState = m_registerStates[0];
		foreach (var state in m_stateDict.Values) {
			state.SetStateActive(state == defaultState);
		}
		AddTop(defaultState.GetType());
	}

	private void InitStateEngine() {
		foreach (var state in m_registerStates) {
			m_stateDict[state.GetType()] = state;
			state.InitByStateEngine(this);
		}
	}

	public void AddTop<T>() where T : LiteState {
		AddTop(typeof(T));
	}

	public void AddTop(Type type) {
		var prev = frontState;
		if (frontState != null) {
			frontState.OnExit();
			frontState.SetStateActive(false);
		}
		if (m_stateDict.TryGetValue(type, out var state)) {
			m_stack.Push(state);
			state.SetStateActive(true);
			state.OnEnter();
		} else {
			Log.LogError($"State of type {type} not found in registered states.");
		}
	}

	public void ReplaceTop<T>() where T : LiteState {
		ReplaceTop(typeof(T));
	}
	public void ReplaceTop(Type type) {
		var top = m_stack.Peek();
		if (top.GetType() == type) {
			Log.LogError($"State of type {type} is already on top of the stack.");
			return;
		}
		top.OnExit();
		top.SetStateActive(false);
		m_stack.Pop();
		if (m_stateDict.TryGetValue(type, out var state)) {
			m_stack.Push(state);
			state.SetStateActive(true);
			state.OnEnter();
		} else {
			Log.LogError($"State of type {type} not found in registered states.");
		}
	}

	public void RemoveTop() {
		var cnt = m_stack.Count;
		if (cnt <= 1) {
			Log.LogError("not enough state");
			return;
		}
		var top = m_stack.Pop();
		top.OnExit();
		top.SetStateActive(false);
		var nextState = m_stack.Peek();
		if (nextState != null) {
			nextState.SetStateActive(true);
			nextState.OnResume();
		}
	}
}
