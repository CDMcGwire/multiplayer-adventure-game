using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Useable : NetworkBehaviour {
	public abstract void Use(NetworkInstanceId actor);
}
