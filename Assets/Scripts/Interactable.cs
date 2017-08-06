using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum Interactions {
	USE,
	SPEAK,
	MOVE
}

public class Interactable : NetworkBehaviour {

	[SerializeField]
	private Useable _useable;

	[SerializeField]
	private float range = 1f;
	private const float rangeCheckBuffer = 0.01f;
	public float InteractRange { get { return range + rangeCheckBuffer; } }

	private NetworkIdentity parentNetID;


	private void Start() {
		parentNetID = GetComponentInParent<NetworkIdentity>();
		Debug.AssertFormat(parentNetID, "Interactable component does not have a parent network ID");
	}


	public bool Interact(Interactions type, NetworkInstanceId actorNetID) {
		switch (type) {
			case Interactions.USE:
				if (_useable) {
					_useable.Use(actorNetID);
					return true;
				}
				else {
					return false;
				}
			case Interactions.SPEAK:
				return true;
			case Interactions.MOVE:
				return true;
			default:
				return false;
		}
	}
}
