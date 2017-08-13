using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum Interactions {
	USE,
	SPEAK,
	MOVE
}

public class Interactable : MonoBehaviour {

	[SerializeField]
	private Useable _useable;

	private NetworkIdentity _identity;
	public NetworkInstanceId NetID { get { return _identity.netId; } }


	private void Start() {
		_identity = GetComponentInParent<NetworkIdentity>();
		Debug.AssertFormat(_identity, "ERROR: Interactable object '" + name + "' has no network identity.");
	}


	public void Interact(Interactions type, GameObject actor) {
		switch (type) {
			case Interactions.USE:
				if (_useable) _useable.Use(actor);
				break;
			case Interactions.SPEAK:
				break;
			case Interactions.MOVE:
				break;
			default:
				break;
		}
	}
}
