using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {
	None = -1,
	Idle,
	Move,
	Attack,
	Jump,
	Spiderweb,
	Death
}

public struct StateAction {
	public Action m_EnterAct;
	public Action m_UpdateAct;
	public Action m_ExitAct;

	public void AddAct(Action _action, int _number) {
		switch (_number) {
			case 0:
				m_EnterAct += _action;
				break;
			case 1:
				m_UpdateAct += _action;
				break;
			case 2:
				m_ExitAct += _action;
				break;
		}
	}

	public void RemoveAct(Action _action, int _number) {
		switch (_number) {
			case 0:
				m_EnterAct -= _action;
				break;
			case 1:
				m_UpdateAct -= _action;
				break;
			case 2:
				m_ExitAct -= _action;
				break;
		}
	}

	public Action GetEnterAct() { return m_EnterAct; }
	public bool AddEnterAct(Action _action) { m_EnterAct += _action; return m_EnterAct == null ? true : false; }
	public void RemoveEnterAct(Action _action) { m_EnterAct -= _action; }

	public Action GetUpdateAct() { return m_UpdateAct; }
	public bool AddUpdateAct(Action _action) { m_UpdateAct += _action; return m_UpdateAct == null ? true : false; }
	public void RemoveUpdateAct(Action _action) { m_UpdateAct -= _action; }

	public Action GetExitAct() { return m_ExitAct; }
	public bool AddExitAct(Action _action) { m_ExitAct += _action; return m_ExitAct == null ? true : false; }
	public void RemoveExitAct(Action _action) { m_ExitAct -= _action; }
}

public class FiniteStateMachine {
	private State m_State;
	private Action m_EndAction;
	private bool m_IsEnd = true;

	private Dictionary<State, StateAction> m_StateAct = new Dictionary<State, StateAction>();

	public FiniteStateMachine() {
		try {
			CoroutineManager.Instance.StartCrt(StateLoop());
		} catch {

		}
		m_EndAction += NullAction;
	}
	public void AddState(State _state, Action _enterAct, Action _updateAct, Action _exitAct) {
		StateAction _stateAction = new StateAction();
		if (_stateAction.AddEnterAct(_enterAct)) _stateAction.AddEnterAct(NullAction);
		if (_stateAction.AddUpdateAct(_updateAct)) _stateAction.AddUpdateAct(NullAction);
		if (_stateAction.AddExitAct(_exitAct)) _stateAction.AddExitAct(NullAction);
		m_StateAct.Add(_state, _stateAction);
	}
	public void AddAction(State _state, Action _action, int _number) { m_StateAct[_state].AddAct(_action, _number); }
	public bool GetEnd() { return m_IsEnd; }
	public void SetEnd(bool _isEnd) { m_IsEnd = _isEnd; }
	public void SetState(State _state) { if (m_State != State.Death) m_State = _state; }
	public State GetState() { return m_State; }
	public void AddEndAction(Action _action) { m_EndAction += _action; }
	public void RemoveEndAction(Action _action) { m_EndAction -= _action; }

	public IEnumerator StateLoop() {
		State _state;
		StateAction _stateAction;
		WaitWhile _waitWhile = new WaitWhile(() => m_State == State.None);
		yield return _waitWhile;
		do {
			_state = m_State;
			_stateAction = m_StateAct[_state];
			_stateAction.GetEnterAct()();
			do {
				_stateAction.GetUpdateAct()();
				yield return null;
			} while (m_IsEnd && _state == m_State);
			_stateAction.GetExitAct()();
		} while (m_IsEnd);
		m_EndAction();
	}

	private void NullAction() {}
}