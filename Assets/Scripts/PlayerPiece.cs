using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Moveable))]
public class PlayerPiece : NetworkBehaviour {
	public override void OnStartAuthority() {
		var cameraDolly = Camera.main.transform.parent;
		Debug.Assert(cameraDolly != null, "Main camera does not have a parent.");

		cameraDolly.position = transform.position;
		cameraDolly.SetParent(transform, true);

		var player = FindObjectOfType<Player>();
		Debug.AssertFormat(player != null, "Player object not found");
		player.boardPiece = GetComponent<Moveable>();
	}
}
