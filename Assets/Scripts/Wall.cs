using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wall : MonoBehaviour {
	private void Update () {
#if UNITY_EDITOR
		var roundedPos = transform.position;
		roundedPos.x = Mathf.Round(roundedPos.x);
		roundedPos.y = Mathf.Round(roundedPos.y);
		roundedPos.z = Mathf.Round(roundedPos.z);
		transform.position = roundedPos;
#endif
	}

	/***************************************************/

	public Vector2 GetXZPos() {
		// Get the position of the child wall mesh
		var pos = GetComponentsInChildren<Transform>()[1].position;
		return new Vector2(pos.x, pos.z);
	}

	/***************************************************/

#if UNITY_EDITOR
	public void PrintDebug() {
		Debug.Log(GetXZPos());
	}
#endif
}
