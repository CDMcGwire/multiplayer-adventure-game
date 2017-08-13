using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	Registers the owning game object to the GameManager and
//	finds all components implementing the IGameManageable interface,
//	so that all objects can be updated appropriately when the game state changes.
public class Piece : MonoBehaviour {
	private IGameManageable[] _managedComponents;

	private void Start() {
		_managedComponents = GetComponentsInChildren<IGameManageable>();
		Debug.Assert(_managedComponents.Length > 0, "No managed components found on Piece '" + name + "'");

		GameState.Game.OnStartRound.AddListener(_OnStartRound);
		GameState.Game.OnEndRound.AddListener(_OnEndRound);
		GameState.Game.OnResetRound.AddListener(_OnResetRound);

		switch (GameState.Game.CurrentState) {
			case GameState.State.PRE:
				//  Components should be responsible for first time setup individually.
				break;
			case GameState.State.RUN:
				//	If the game is already started, give components the run signal.
				_OnStartRound();
				break;
			case GameState.State.END:
				//	If the game is already over, give components the end signal.
				_OnEndRound();
				break;
			default:
				break;
		}
	}

	private void OnDestroy() {
		GameState.Game.OnStartRound.RemoveListener(_OnStartRound);
		GameState.Game.OnEndRound.RemoveListener(_OnEndRound);
		GameState.Game.OnResetRound.RemoveListener(_OnResetRound);
	}


	private void _OnStartRound() {
		foreach (var component in _managedComponents) component.Go();
	}
	private void _OnEndRound() {
		foreach (var component in _managedComponents) component.Halt();
	}
	private void _OnResetRound() {
		foreach (var component in _managedComponents) component.Reset();
	}
}
