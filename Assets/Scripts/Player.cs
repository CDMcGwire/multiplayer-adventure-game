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

	// Local input functions

	
	public GameObject playerPiecePrefab;
	private Mover mover;

	public int ID { get { return GetInstanceID(); } }


	private void Start() {
		if (isServer) GameState.Game.AddPlayer(netId);
	}

	private void Update() {
		if (!isLocalPlayer) return;

		CheckClick();
		CheckChangeDirection();
		MoveInCurrentDirection();
	}

	private void OnDestroy() {
		if (isServer) {
			GameState.Game.RemovePlayer(netId);
			Board.GameBoard.Remove(mover);
		}
	}


	/***************************************************/


	private void MoveInCurrentDirection() {
		switch (heading) {
			case Direction.IDLE:
				break;
			case Direction.NORTH:
				CmdMoveRelative(0, 1);
				if (Input.GetButtonUp("Up")) CheckForHeldButton();
				break;
			case Direction.SOUTH:
				CmdMoveRelative(0, -1);
				if (Input.GetButtonUp("Down")) CheckForHeldButton();
				break;
			case Direction.EAST:
				CmdMoveRelative(1, 0);
				if (Input.GetButtonUp("Right")) CheckForHeldButton();
				break;
			case Direction.WEST:
				CmdMoveRelative(-1, 0);
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
			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				var interactable = hitInfo.transform.gameObject.GetComponent<Interactable>();
				if (interactable != null) interactable.Interact(Interactions.USE, mover.netId);
			}
		}
	}


	/***************************************************/

	// Networking functions

	[Command]
	public void CmdSpawnPlayerPiece() {
		var piece = Instantiate(playerPiecePrefab) as GameObject;
		piece.name = "Player: " + GetInstanceID();
		NetworkServer.SpawnWithClientAuthority(piece, connectionToClient);

		mover = piece.GetComponent<Mover>();
	}

	[Command]
	public void CmdMoveRelative(int x, int y) {
		IntVec3 pos;
		Board.GameBoard.GetPiecePos(mover, out pos);

		pos.x += x;
		pos.y += y;

		mover.Move(pos);
	}

	public override void OnStartLocalPlayer() {
		// Spawn player piece on server
		CmdSpawnPlayerPiece();
	}
}
