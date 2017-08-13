using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;



public class GameState : NetworkBehaviour {
	public UnityEvent OnStartRound;
	public UnityEvent OnEndRound;
	public UnityEvent OnResetRound;

	private static GameState _globalState;
	public static GameState Game { get { return _globalState; } }

	public enum State {
		PRE,
		RUN,
		END
	};
	[SyncVar(hook = "Sync_OnStateChange")]
	private State _state = State.PRE;

	public State CurrentState { get { return _state; } }


	private void Awake() {
		if (_globalState == null) _globalState = this;
		else {
			Debug.LogError("More than one board detected.");
			return;
		}
	}

	private void Start() {
		// TESTING CODE
		if (!isServer) return;
		Invoke("StartRound", 0.5f);
	}


	public void StartRound() {
		if (!isServer) return;
		if (_state == State.PRE) _state = State.RUN;
	}
	public void EndRound() {
		if (!isServer) return;
		if (_state == State.RUN) _state = State.END;

		// TESTING CODE
		Invoke("ResetRound", 2.0f);
	}
	public void ResetRound() {
		if (!isServer) return;
		if (_state == State.END) _state = State.PRE;

		// TESTING CODE
		Invoke("StartRound", 1.0f);
	}


	private void Sync_OnStateChange(State newState) {
		// This may not need to be called on all clients
		switch (newState) {
			case State.PRE:
				OnResetRound.Invoke();
				break;
			case State.RUN:
				OnStartRound.Invoke();
				break;
			case State.END:
				OnEndRound.Invoke();
				break;
			default:
				break;
		}
	}
}

