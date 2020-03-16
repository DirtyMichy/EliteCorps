using UnityEngine;
using System.Collections;

[System.Serializable]
public class SaveFile
{
    public static SaveFile current;
    public int MAXPLAYERS = 4;
    public int MAXMISSIONS = 100;
    public string test;
    public bool firstTimeEntering = true;
    public int playerCount;
    public int[] campaignMissionStatus;    //0 locked, 1 unlocked, 2 won
    public int[] playerChosenCharacter;
    public bool[] playerActive;

    public SaveFile()
    {
        campaignMissionStatus = new int[MAXMISSIONS];

        //every first mission of each chapter is unlocked by default
        for (int i = 0; i < MAXMISSIONS; i++)
        {
            campaignMissionStatus[i%5] = 1;
        }

        playerChosenCharacter = new int[MAXPLAYERS];
        playerActive = new bool[MAXPLAYERS];
    }
}