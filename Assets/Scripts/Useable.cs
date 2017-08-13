using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// The usable class hides its overridable logic behind a network checked interface.
// This is to ensure that any implementation is consistently interfaced on the server only.
// An implementation of 'UseAction' may then send calls back to clients as needed.
public abstract class Useable : NetworkBehaviour {
	public void Use(GameObject actor) {
		if (!isServer) {
			Debug.LogError("ERROR: Useable.Use() should only be called on the server. Calling object: '" + name + "'");
			return;
		}
		else UseAction(actor);
	}

	protected abstract void UseAction(GameObject actor); // Should only be called server side
}
