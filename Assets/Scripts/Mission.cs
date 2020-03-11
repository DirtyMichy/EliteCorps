﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the mission gameObjects in the MissionScreen
public class Mission : MonoBehaviour
{

	public int thisEpisode = 0;
	public int thisMission = 0;
	public enum missionStatus
	{
		locked,
		unlocked,
		finished
	}

	//public missionStatus status = missionStatus.locked;

	public int status = 0;
}
