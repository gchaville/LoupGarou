﻿using UnityEngine;
using System.Collections;

public class Voyante : BaseRole {

	public override void Start () {
        base.Start();
	}

    public override void PlayTurn() {
        ready = false;
        selectedPlayer = null;

        StartCoroutine(WaitForChoice());
    }

    public override string ToString () {
		return string.Format ("[Voyante]");
	}

    IEnumerator WaitForChoice() {
        while(selectedPlayer == null) {
            Debug.Log(selectedPlayer);
            yield return new WaitForSeconds(0.5f);
        }

        ready = true;
    }
}
