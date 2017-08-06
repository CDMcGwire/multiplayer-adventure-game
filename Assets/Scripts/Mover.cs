// Connor McGwire - Mover Component

using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// The Mover handles logic for *how* to move. Decision making processes, like
/// player and AI components supply it with destinations.
/// </summary>
public class Mover : NetworkBehaviour {

	// Inspector set fields

	public bool controllable = true;

	[SerializeField]
	private bool blocking = true;
	[SerializeField]
	private float speed = 1;
	[SerializeField]
	private int startLayer = 0;
	[SerializeField]
	private bool randomStart = false;
	public bool RandomStart { get { return randomStart; } }


	// Private data
	
	[SyncVar]
	private Vector3 targetWorldPos;
	public Vector3 TargetWorldPosition {
		get { return targetWorldPos; }
		set { targetWorldPos = value; }
	}

	[SyncVar]
	private bool moving = false;
	private bool canMove = true;

	private const float newInputDelay = 0.02f;
	private float inputCooldown = 0;


	// Accessor Properties
	
	public int ID { get { return GetInstanceID(); } }
	public bool isMoving { get { return moving; } }


	/***************************************************/
	

	private void Start() {
		if (!isServer) {
			Invoke("SyncStartPosition", 0.5f);
			return;
		}

		if (randomStart) {
			if (Board.GameBoard.PlaceAtRandom(this)) {
				transform.position = targetWorldPos;
			}
		}
		else {
			var gridPos = Board.GameBoard.WorldToGrid(transform.position);
			if (Board.GameBoard.Place(gridPos, this)) {
				transform.position = targetWorldPos;
			}
		}
	}

	private void Update() {
		if (!isClient) return;

#if UNITY_EDITOR
		//if (!moving) {
		//	var roundedPos = transform.position;
		//	roundedPos.x = Mathf.Round(roundedPos.x);
		//	// Determine how to make it snap to tile
		//	roundedPos.z = Mathf.Round(roundedPos.z);
		//	transform.position = roundedPos;
		//}
#endif

		if (inputCooldown > 0) {
			inputCooldown -= Time.deltaTime;
			if (inputCooldown <= 0) moving = false;
		}
		else if (moving) {
			transform.position = Vector3.Lerp(transform.position, targetWorldPos, Time.deltaTime * speed);
			if (Vector3.Distance(transform.position, targetWorldPos) < 0.01) {
				transform.position = targetWorldPos;
				inputCooldown = newInputDelay;
			}
		}

		canMove = !moving && speed > 0;
	}


	/***************************************************/

	
	public bool Move(IntVec3 target, bool teleport = false) {
		if (!controllable || !canMove) return false;

		var tile = Board.GameBoard.GetTileInfo(target);
		if (!tile.isValid) return false;

		if (Board.GameBoard.Move(target, this)) {
			if (teleport) {
				transform.position = targetWorldPos;
			}
			else moving = true;
			return true;
		}

		return false;
	}


	private void SyncStartPosition() {
		transform.position = targetWorldPos;
	}
}
