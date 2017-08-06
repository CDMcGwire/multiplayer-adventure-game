using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tile : MonoBehaviour {
	[SerializeField]
	private Transform underside;
	[SerializeField]
	private float height = 0.0f;


	private IntVec3 gridPos = new IntVec3(0, 0, 0);
	public IntVec3 GridPos {
		get { return gridPos; }
		set { gridPos = value; }
	}

	/***************************************************/

	private void Update() {
#if UNITY_EDITOR
		var roundedPos = transform.position;
		roundedPos.x = Mathf.Round(roundedPos.x);
		roundedPos.y = height;
		roundedPos.z = Mathf.Round(roundedPos.z);
		transform.position = roundedPos;

		if (underside) underside.localScale = new Vector3(1, height + 1, 1);
#endif
	}

	/***************************************************/

	public Vector2 GetXZPos() {
		var pos = transform.position;
		return new Vector2(pos.x, pos.z);
	}

	public TileInfo Info {
		get { return new TileInfo(gridPos, transform.position, height); }
	}

	/***************************************************/

#if UNITY_EDITOR
	public void PrintDebug() {
		Debug.Log("Tile Grid Coordinates: " + GetXZPos());
		Debug.Log("Tile Height: " + height);
	}
#endif
}



/// <summary>
/// Class for passing tile information to other objects.
/// </summary>
[System.Serializable]
public class TileInfo {
	[SerializeField]
	private bool valid = false;
	public bool isValid { get { return valid; } }
	[SerializeField]
	private IntVec3 gridPos;
	public IntVec3 GridPos { get { return gridPos; } }
	[SerializeField]
	private Vector3 worldPos;
	public Vector3 WorldPos { get { return worldPos; } }
	[SerializeField]
	private float height;
	public float Height { get { return height; } }

	public TileInfo(IntVec3 gp, Vector3 wp, float h) {
		valid = true; gridPos = gp; worldPos = wp; height = h;
	}

	public TileInfo() {
		valid = false; gridPos = new IntVec3(0, 0, -1); worldPos = Vector3.zero; height = 0;
	}

	public static TileInfo empty {
		get { return new TileInfo(); }
	}
}