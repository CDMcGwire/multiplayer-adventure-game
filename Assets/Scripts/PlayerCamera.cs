using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCamera : NetworkBehaviour {
	public override void OnStartAuthority() {
		var cameraDolly = Camera.main.transform.parent;
		Debug.Assert(cameraDolly != null, "Main camera does not have a parent.");

		cameraDolly.position = transform.position;
		cameraDolly.SetParent(transform, true);
	}
}
