﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

abstract public class BaseRole : NetworkBehaviour {

    protected bool ready;
    protected GameObject selectedPlayer;

	[SyncVar]
    public GameObject lover;

    public List<GameObject> players = new List<GameObject>();

    public virtual void Start () {
		players =  GameManager.instance.GetPlayers();

        lover = null;
        selectedPlayer = null;
	}

    public abstract void PlayTurn();

    public virtual void Die() {
		CmdRemovePlayer (gameObject);
        GetComponent<Player>().death = true;
    }

	[Command]
	void CmdRemovePlayer(GameObject _p) {
		if (GetComponent<Loup>())
			GameManager.instance.RemoveWolf (_p);

		GameManager.instance.RemovePlayer (_p);
	}

    public bool IsReady() {
        return ready;
    }

    public void SetLover(GameObject l) {
        lover = l;
    }

    public GameObject GetSelectedPlayer() {
        return selectedPlayer;
    }

    public void SetSelectedPlayer(GameObject g) {
        selectedPlayer = g;
    }
}
