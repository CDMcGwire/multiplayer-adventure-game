using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TheButton : Useable, IGameManageable {
	public string TriggerName = "Default";
	public float ResetTime = 5f;

	[SerializeField]
	private ParticleSystem _particles;
	[SerializeField]
	private Animator _animator;

	private Moveable _mover;
	
	private bool _triggered = false;

	
	/***************************************************/

	
	private void Start() {
		Debug.AssertFormat(_animator, "No animator component given to '" + name + "'");
		_mover = GetComponentInParent<Moveable>();

		Reset();
	}

	public void Go() {
		Debug.Log("Called 'Go()' on " + name);
	}
	public void Halt() {
		Debug.Log("Called 'Halt()' on " + name);
	}
	public void Reset() {
		if (TriggerName == "Default") _animator.ResetTrigger(0);
		else _animator.ResetTrigger(TriggerName);
		_triggered = false;

		Board.GameBoard.PlaceAtRandom(_mover);
	}



	/***************************************************/


	protected override void UseAction(GameObject actor) {
		if (_triggered) return;

		// WinGame(actor);
		Rpc_Celebrate();
		_triggered = true;

		GameState.Game.EndRound();
	}


	/***************************************************/


	[ClientRpc]
	public void Rpc_Celebrate() {
		if (TriggerName != "Default") _animator.SetTrigger(TriggerName);
		else _animator.SetTrigger(0);
	}

	// ANIMATION EVENTS //

	public void PlayParticles() { 
		Debug.AssertFormat(_particles, "No particle system detected for AnimateOnClick component of '" + name + "'");
		_particles.Play(true);
	}
}
