using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnWinHandler(object Sender, OnWinArgs e);
public class OnWinArgs : EventArgs { 
	public int PlayerID { get { return PlayerID; } private set { PlayerID = value; } }

	public OnWinArgs(int id) { PlayerID = id; }
}


public class GameState : NetworkBehaviour {
	private static GameState globalState;
	public static GameState Game { get { return globalState; } }


	private Dictionary<uint, int> scores;
	
	public void AddPlayer(NetworkInstanceId playerId) {
		scores.Add(playerId.Value, 0);
	}
	public void RemovePlayer(NetworkInstanceId playerId) {
		scores.Remove(playerId.Value);
	}


	private void Awake() {
		if (globalState == null) globalState = this;
		else {
			Debug.LogError("More than one board detected.");
			return;
		}
	}


	public override void OnStartServer() {
		scores = new Dictionary<uint, int>();

	}

	public void Win(NetworkInstanceId winner) {
		
	}
}

