﻿using UnityEngine;

public class Cupidon : BaseRole {

    GameObject secondSelected;

    public override void Start() {
        secondSelected = null;
        base.Start();
    }

    public override void PlayTurn() {
        ready = false;

        int rand = Random.Range(0, players.Count);
        int rand2 = rand;

        while (rand == rand2)
            rand2 = Random.Range(0, players.Count);

        selectedPlayer = players[rand];
        secondSelected = players[rand2];

        selectedPlayer.GetComponent<BaseRole>().SetLover(secondSelected);
        secondSelected.GetComponent<BaseRole>().SetLover(selectedPlayer);

        Debug.Log("[CUPIDON] Le couple est " + selectedPlayer.GetComponent<Player>().ID() + " et " + secondSelected.GetComponent<Player>().ID());

        ready = true;
    }
}