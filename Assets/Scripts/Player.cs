using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// NOTE: Should have an actor component that wraps all the other components

public class Player : NetworkBehaviour {

	// Directional indicator

	private enum Direction {
		IDLE,
		NORTH,
		SOUTH,
		EAST,
		WEST
	};

	private Direction heading = Direction.IDLE;


	/***************************************************/


	[SerializeField]
	private int _useRange = 1;
	[SerializeField]
	private float _rangeCheckOffset = 0.1f; // Needs to be adjusted to something that scales to object size.

	public GameObject playerPiecePrefab;

	private Moveable _mover;
	public Moveable boardPiece { get { return _mover; } set { _mover = value; } }

	public int ID { get { return GetInstanceID(); } }


	private void Update() {
		if (!isLocalPlayer) return;

		CheckClick();
		CheckChangeDirection();
		MoveInCurrentDirection();
	}

	private void OnDestroy() {
		if (isServer) {
			Board.GameBoard.Remove(_mover);
		}
	}

	public override void OnStartLocalPlayer() {
		// Spawn player piece on server
		Cmd_SpawnPlayerPiece();
	}


	/***************************************************/


	private void MoveInCurrentDirection() {
		switch (heading) {
			case Direction.IDLE:
				break;
			case Direction.NORTH:
				Cmd_MoveRelative(0, 1);
				if (Input.GetButtonUp("Up")) CheckForHeldButton();
				break;
			case Direction.SOUTH:
				Cmd_MoveRelative(0, -1);
				if (Input.GetButtonUp("Down")) CheckForHeldButton();
				break;
			case Direction.EAST:
				Cmd_MoveRelative(1, 0);
				if (Input.GetButtonUp("Right")) CheckForHeldButton();
				break;
			case Direction.WEST:
				Cmd_MoveRelative(-1, 0);
				if (Input.GetButtonUp("Left")) CheckForHeldButton();
				break;
			default:
				break;
		}
	}

	private void CheckChangeDirection() {
		if (Input.GetButtonDown("Up")) {
			heading = Direction.NORTH;
		}
		else if (Input.GetButtonDown("Down")) {
			heading = Direction.SOUTH;
		}
		else if (Input.GetButtonDown("Right")) {
			heading = Direction.EAST;
		}
		else if (Input.GetButtonDown("Left")) {
			heading = Direction.WEST;
		}
	}

	private void CheckForHeldButton() {
		if (Input.GetButton("Up")) {
			heading = Direction.NORTH;
		}
		else if (Input.GetButton("Down")) {
			heading = Direction.SOUTH;
		}
		else if (Input.GetButton("Right")) {
			heading = Direction.EAST;
		}
		else if (Input.GetButton("Left")) {
			heading = Direction.WEST;
		}
		else {
			heading = Direction.IDLE;
		}
	}


	private void CheckClick() {
		if (Input.GetMouseButtonDown(0)) {
			var hitInfo = new RaycastHit();
			var hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (!RangeCheck(hitInfo.collider)) return;

				var interactable = hitInfo.collider.GetComponent<Interactable>();
				if (interactable != null) Cmd_UseNetworkTarget(interactable.NetID);
			}
		}
	}

	
	private bool RangeCheck(Collider target) {
		var fromPos = _mover.transform.position;
		fromPos.z += _rangeCheckOffset;
		var toPos = target.transform.position;
		toPos.z += _rangeCheckOffset;
		var ray = new Ray(fromPos, toPos - fromPos);

		Debug.DrawRay(fromPos, toPos - fromPos, Color.blue, 2f);

		var hitInfo = new RaycastHit();
		var hit = Physics.Raycast(ray, out hitInfo);
		// Can the player see the object and is it the same object?
		if (!hit || hitInfo.collider.GetInstanceID() != target.GetInstanceID()) return false;

		Debug.DrawLine(fromPos, hitInfo.point, Color.red, 0.5f);

		var hitGridPos = target.transform.position.ToInt3();
		// Is the object close enough in grid space?
		if (hitGridPos.DistanceTo(_mover.transform.position.ToInt3()) > _useRange) return false;
		
		Debug.DrawLine(fromPos, hitInfo.point, Color.green, 2f);
		return true;
	}


	/***************************************************/

	// Networking functions

	[Command]
	public void Cmd_SpawnPlayerPiece() {
		var piece = Instantiate(playerPiecePrefab) as GameObject;
		piece.name = "Player: " + GetInstanceID();
		NetworkServer.SpawnWithClientAuthority(piece, connectionToClient);

		_mover = piece.GetComponent<Moveable>();
	}

	[Command]
	public void Cmd_MoveRelative(int x, int y) {
		IntVec3 pos;
		if (!Board.GameBoard.GetPiecePos(_mover, out pos)) {
			Debug.LogError("Tried moving an object that was not on the board.");
			return;
		}

		pos.x += x;
		pos.y += y;

		_mover.Move(pos);
	}

	[Command]
	public void Cmd_UseNetworkTarget(NetworkInstanceId targetId) {
		var target = NetworkServer.FindLocalObject(targetId);
		if (target == null) {

			Debug.Log("Could not find target object on server.");

			return;
		}

		// Range check on server side
		var targetCollider = target.GetComponentInChildren<Collider>();
		if (RangeCheck(targetCollider)) {

			Debug.Log("Range check success on server");

			var targetInteractable = targetCollider.GetComponent<Interactable>();
			Debug.AssertFormat(targetInteractable != null, "Target interactable not found on server.");
			targetInteractable.Interact(Interactions.USE, gameObject);
		}
	}
}
