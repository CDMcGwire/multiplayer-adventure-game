using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// NOTE: Could use hash table on InstanceID to track placement of objects

public class Board : NetworkBehaviour {
	private static Board globalBoard = null;
	public static Board GameBoard { get { return globalBoard; } }


	/***************************************************/


	public bool allowMovement = true;


	/***************************************************/


	private class Cell {
		public Mover occupant;
		public Tile tile;
		public Wall north, east, south, west;
		public Cell above;

		public IntVec3 GridPos { get { return tile.GridPos; } }

		public Cell(Tile t) { tile = t; }
#if UNITY_EDITOR
		public void PrintDebug() {
			if (tile) {
				Debug.Log("Active Tile - Walls: " +
					"N = " + ( (north)	? "Y" : "N") +
					"E = " + ( (east)	? "Y" : "N") +
					"S = " + ( (south)	? "Y" : "N") +
					"W = " + ( (west)	? "Y" : "N"));
			}
			else {
				Debug.Log("Inactive Tile - Walls: " +
					"N = " + ( (north)	? "Y" : "N") +
					"E = " + ( (east)	? "Y" : "N") +
					"S = " + ( (south)	? "Y" : "N") +
					"W = " + ( (west)	? "Y" : "N"));
			}
		}
#endif
	}


	/***************************************************/


	// Inspector set variables
	public GameObject mapRoot;

	// Private Data

	int xOffset = 0;
	int yOffset = 0;
	
	private Tile[] tiles;
	private Wall[] walls;
	private Cell[,] grid;
	private Cell[] validCells;
	private List<Cell> emptyCells;
	private Dictionary<int, IntVec3> placedPieces;


	/***************************************************/


	private void Awake() {
		Debug.Log(name + " called AWAKE");
		Debug.Assert(mapRoot, "No map root assigned.");

		if (!globalBoard) globalBoard = this;
		else {
			Debug.LogError("More than one board detected.");
			return;
		}

		placedPieces = new Dictionary<int, IntVec3>();

		AssembleGrid();
	}

	//private void Start() {
	//	Debug.Log(name + " called START");
	//}

	//public override void OnStartServer() {
	//	Debug.Log(name + " called OnStartServer");
	//}

	//public override void OnStartClient() {
	//	Debug.Log(name + " called OnStartClient");
	//}


	/***************************************************/


	private void AssembleGrid() {
		tiles = mapRoot.GetComponentsInChildren<Tile>();
		Debug.Assert(tiles.Length > 0, "NO TILES FOUND");
		walls = mapRoot.GetComponentsInChildren<Wall>();

		// Find min and max tile coordinates
		int xmin = 0,xmax = 0,ymin = 0,ymax = 0;
		foreach (var tile in tiles) {
			var pos = tile.transform.position.ToInt2();

			if (pos.x < xmin) xmin = pos.x;
			else if (pos.x > xmax) xmax = pos.x;

			if (pos.y < ymin) ymin = pos.y;
			else if (pos.y > ymax) ymax = pos.y;
		}

		// Setup grid parameters
		var width = (xmax + 1) - xmin;
		var height = (ymax + 1) - ymin;
		xOffset = -xmin;
		yOffset = -ymin;

		grid = new Cell[width, height];
		validCells = new Cell[tiles.Length];
		emptyCells = new List<Cell>();

		// Populate tiles
		int validCount = 0;
		foreach (var tile in tiles) {
			var pos = tile.GetXZPos();
			var x = Mathf.RoundToInt(pos.x) + xOffset;
			var y = Mathf.RoundToInt(pos.y) + yOffset;
			Debug.AssertFormat(x < width, "Mathed wrong: " + x + " >= " + width + "; Min = " + xmin);
			Debug.AssertFormat(y < height, "Mathed wrong: " + y + " >= " + height + "; Min = " + ymin);

			// No previously existing tile
			if (grid[x, y] == null) {
				tile.GridPos = new IntVec3(x, y, 0);
				var cell = new Cell(tile);
				grid[x, y] = cell;

				validCells[validCount] = cell;
				validCount++;

				emptyCells.Add(cell);
			}
			else { // For stacking tiles
				var z = 1;
				var lowerCell = grid[x, y];
				while (lowerCell.above != null) {
					lowerCell = lowerCell.above;
					z++;
				}
				tile.GridPos = new IntVec3(x, y, z);
				var newCell = new Cell(tile);
				lowerCell.above = newCell;

				validCells[validCount] = newCell;
				validCount++;

				emptyCells.Add(newCell);
			}
			// NOTE: Stack order has no bearance on pathing, only on search methods.
		}


		// Determine wall positions
		foreach (var wall in walls) {
			var pos = wall.GetXZPos();
			var northAligned = Mathf.RoundToInt(10 * pos.x) != Mathf.RoundToInt(pos.x) * 10;

			if (northAligned) {
				var x1 = Mathf.FloorToInt(pos.x) + xOffset; // Left tile
				var x2 = Mathf.CeilToInt(pos.x) + xOffset;  // Right tile
				var y = Mathf.RoundToInt(pos.y) + yOffset;
				if (x1 >= 0 && x1 < width) grid[x1, y].east = wall; // TODO: Add layer support
				if (x2 >= 0 && x2 < width) grid[x2, y].west = wall;
			}
			else {
				var y1 = Mathf.FloorToInt(pos.y) + yOffset; // bottom tile
				var y2 = Mathf.CeilToInt(pos.y) + yOffset;  // top tile
				var x = Mathf.RoundToInt(pos.x) + xOffset;
				if (y1 >= 0 && y1 < height) grid[x, y1].north = wall;
				if (y2 >= 0 && y2 < height) grid[x, y2].south = wall;
			}
		}
	}


	/***************************************************/

	
	public TileInfo GetTileInfo(IntVec3 pos) {
		var cell = getCell(pos);
		// If an invalid z layer is passed in, algorithm passes back the highest found.
		if (cell != null) return cell.tile.Info;
		else return TileInfo.empty;
	}


	public IntVec3 WorldToGrid (Vector3 worldPos) {
		// Add code to get layer
		var gridPos = worldPos.ToInt3(0);
		gridPos.x += xOffset;
		gridPos.y += yOffset;
		return gridPos;
	}


	public bool GetPiecePos(Mover m, out IntVec3 pos) {
		if (placedPieces.TryGetValue(m.ID, out pos)) {
			return true;
		}
		else return false;
	}


	public bool Move(IntVec3 to, Mover m) {
		if (allowMovement && placedPieces.ContainsKey(m.GetInstanceID())) {
			var fromCell = getCell(placedPieces[m.ID]);
			var toCell = getCell(to);

			if (toCell != null && toCell.occupant == null) {
				placedPieces[m.ID] = to;
				fromCell.occupant = null;
				toCell.occupant = m;
				m.TargetWorldPosition = toCell.tile.transform.position;

				emptyCells.Add(fromCell);
				emptyCells.Remove(toCell);

				return true;
			}
		}
		return false;
	}


	// Places piece into the dictionary for tile ownership validation
	public bool Place(IntVec3 on, Mover m) {
		if (placedPieces.ContainsKey(m.ID)) return false;

		var cell = getCell(on);
		Debug.Log(cell != null);
		if (cell != null && cell.occupant == null) {
			cell.occupant = m;
			placedPieces.Add(m.ID, on);
			emptyCells.Remove(cell);
			m.TargetWorldPosition = cell.tile.transform.position;
			Debug.Log("Target placed at " + on.x + " " + on.y);

			return true;
		}
		else return false;
	}

	
	public bool PlaceAtRandom(Mover m) {
		Debug.Assert(emptyCells != null, "Tried placing before board spawned!!");
		if (emptyCells.Count < 1) return false;
		
		var index = Random.Range(0, emptyCells.Count);
		var cell = emptyCells[index];

		placedPieces.Add(m.ID, cell.GridPos);
		cell.occupant = m;
		emptyCells.RemoveAt(index);
		m.TargetWorldPosition = cell.tile.transform.position;

		return true;
	}


	// Removes piece from the dictionary for tile ownership validation
	public bool Remove(Mover m) {
		var cell = getCell(placedPieces[m.ID]);
		if (placedPieces.Remove(m.ID)) {
			cell.occupant = null;
			emptyCells.Add(cell);
			return true;
		}
		return false;
	}


	public bool RangeCheck(Mover from, Mover to, int maxRange) {
		// THE FUCK AM I DOING
	}


	/***************************************************/

	
	private Cell getCell (IntVec3 pos) {
		if (pos.x >= grid.GetLength(0) || pos.x < 0) return null;
		if (pos.y >= grid.GetLength(1) || pos.y < 0) return null;

		var cell = grid[pos.x, pos.y];
		if (cell == null || pos.z <= 0) return cell;

		var i = 0;
		// Loop ends when either the requested layer is found, or a null entry is reached
		while (cell != null && i < pos.z) {
			cell = cell.above;
			i++;
		}

		return cell;
	}
}