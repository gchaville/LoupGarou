﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour {

	[SerializeField]
	List<Sun> dayNight = new List<Sun>(2);

	[SerializeField]
	FireLightScript fireCamp = null;

	public FireLightScript FireCamp {
		get { return fireCamp; }
	}

	bool isDay = true;

	public IEnumerator SwitchTime() 
	{
		if (IsReady ()) 
		{
			foreach (Sun _s in dayNight)
				_s.switchCycle = true;

			yield return new WaitUntil (IsFire);
			{
				isDay = !isDay;

				if (!isDay) {
					fireCamp.run = false;
					fireCamp.ChangeColor (GameManager.TurnIssue.DEAD);
				} else {
					fireCamp.run = true;
					fireCamp.ChangeColor (GameManager.TurnIssue.NO_TURN);
				}
			}

			yield return new WaitUntil (IsReady);
			{
				if (!isDay) {
					RenderSettings.ambientIntensity = 0f;
				} else {
					RenderSettings.ambientIntensity = 0.5f;
				}
			}
		}
			
	}

	bool IsFire()
	{
		if (!isDay)
			return !(dayNight [0].fire) && !(dayNight [0].fire);
		else
			return dayNight [0].fire && dayNight [1].fire;
	}

	bool IsReady()
	{
		return (!dayNight [0].switchCycle) && (!dayNight [1].switchCycle);
	}

}
