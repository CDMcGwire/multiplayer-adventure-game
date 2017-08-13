// Connor McGwire - Mover Component

using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// The Mover handles logic for *how* to move. Decision making processes, like
/// player and AI components supply it with destinations.
/// </summary>
public class Moveable : NetworkBehaviour, IGameManageable {

	// Inspector set fields

	[SerializeField]
	private bool _blocking = true;
	public bool Blocking { get { return _blocking; } }

	[SerializeField]
	private float _speed = 1;
	public float Speed { get { return _speed; } }

	[SerializeField]
	public int startLayer = 0;

	[SerializeField]
	private bool _randomStart = false;
	public bool RandomStart { get { return _randomStart; } }


	// Private data
	
	[SyncVar]
	private Vector3 _targetPos;
	public Vector3 TargetWorldPosition {
		get { return _targetPos; }
	}
	public void SetTargetWorldPosition(Vector3 pos, bool shouldTeleport) {
		if (!isServer) return;
		_targetPos = pos;
		RpcUpdateClientTargetPosition(pos, shouldTeleport);
	}

	[SyncVar]
	private bool _moving = false;

	public readonly float InputDelay = 0.01f;
	private float _inputCooldown = 0f;

	public readonly float NetworkSyncDelay = 0.03f;
	private float _networkSyncTimer = 0f;

	private bool _stopMovement = true;


	// Accessor Properties
	
	public int ID { get { return GetInstanceID(); } }
	public bool isMoving { get { return _moving; } }


	/***************************************************/
	

	private void Start() {
		Reset();
	}

	private void Update() {
		if (!isClient) return;

		if (_networkSyncTimer <= 0) {
			if (Vector3.Distance(transform.position, _targetPos) > 0.01) _moving = true;
			_networkSyncTimer = NetworkSyncDelay;
		}
		else _networkSyncTimer -= Time.deltaTime;

		if (_inputCooldown > 0) {
			_inputCooldown -= Time.deltaTime;
			if (_inputCooldown <= 0) _moving = false;
		}
		else if (_moving) {
			transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * _speed);
			if (Vector3.Distance(transform.position, _targetPos) < 0.01) {
				transform.position = _targetPos;
				_inputCooldown = InputDelay;
			}
		}
	}


	/***************************************************/

	
	public void Go() {
		_stopMovement = false;
	}
	public void Halt() {
		Debug.Log("Called 'Halt()' on " + name);
		_stopMovement = true;
	}
	public void Reset() {
		if (_randomStart) {
			if (Board.GameBoard.PlaceAtRandom(this)) {
				transform.position = _targetPos;
			}
		}
		else {
			var gridPos = Board.GameBoard.WorldToGrid(transform.position);
			if (Board.GameBoard.Place(gridPos, this)) {
				transform.position = _targetPos;
			}
		}
	}


	/***************************************************/


	public bool Move(IntVec3 target, bool teleport = false) {
		if (_moving || _stopMovement) return false;

		var tile = Board.GameBoard.GetTileInfo(target);
		if (!tile.isValid) return false;

		if (Board.GameBoard.Move(target, this)) {
			if (teleport) {
				transform.position = _targetPos;
			}
			else _moving = true;
			return true;
		}

		return false;
	}

	[ClientRpc]
	public void RpcUpdateClientTargetPosition(Vector3 pos, bool shouldTeleport) {
		_targetPos = pos;
		if (shouldTeleport) transform.position = pos;
		else _moving = true;
	}
}
