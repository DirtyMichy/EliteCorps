using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    private Vector2 min, max;
    private float campaingObjectiveSpawn = 30;
    private int maxRngForWaves = 2;
    public int seconds = 0;
    public GameObject[] bosses;
    public GameObject[] waveEnemies;
    public GameObject[] e1Enemies;
    public GameObject[] e2Enemies;
    public GameObject[] objectives;

    public void Start()
    {
        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
    }

    public void StartSpawnCoroutines()
    {
        StartCoroutine("Spawn");

        campaingObjectiveSpawn = 30;
        maxRngForWaves = 2;
        seconds = 20;
    }

    public void StopSpawnCoroutines()
    {
        StopCoroutine("Spawn");
    }

    void Waves()
    {
        if (seconds % 30 == 0)
            maxRngForWaves = Mathf.Clamp((int)(maxRngForWaves + 1), 0, waveEnemies.Length);
        int rng = Random.Range(0+(maxRngForWaves/4), maxRngForWaves); //-1

        Instantiate(waveEnemies[rng], waveEnemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), waveEnemies[rng].transform.rotation);
    }

    public void spawnBoss(int bossNumber)
    {
        Instantiate(bosses[bossNumber]);
    }

    //Spawns enemies and counts seconds
    public IEnumerator Spawn()
    {
        while (Manager.current.currentMenu == Manager.activeMenu.None)
        {
            if (Manager.current.gameMode == Manager.selectedGameMode.campaign)
            {
                missionSpawns();
                yield return new WaitForSeconds(20/Manager.current.playerCount);
            }

            if (Manager.current.gameMode == Manager.selectedGameMode.survive)
            {
                Waves();
                yield return new WaitForSeconds(10f / Manager.current.playerCount);
            }

            seconds++;
        }
    }
    
    void missionSpawns()
    {
        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float spawnPosition = 0;
        float screenWidth = Mathf.Abs(min.x) + max.x;

        if (Manager.current.missionMode == Manager.missionObjectives.killObjective)
        {
            //Spawning additional Objectives, if you missed the objective another one should spawn faster
            if (Manager.current.objectiveKills < Manager.current.requiredObjectivesToKill && (seconds % campaingObjectiveSpawn == 0))
            {
                Instantiate(objectives[1], objectives[1].transform.position = new Vector2(0f, 10f), objectives[1].transform.rotation);
                campaingObjectiveSpawn = 20;
            }
        }

        //############################################################ EPISODE I ############################################################

        if (Manager.current.currentMissionSelected < 5)
        {
            if (Manager.current.currentMissionSelected == 0)
            {
                int rng = Random.Range(0, 3); //-1
                if (rng == 0)
                    for (int i = 0; i < 7; i++)
                    {
                        spawnPosition = -6f + 2f * i; //FullscreenSpawn

                        Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e1Enemies[0].transform.rotation);
                    }
                if (rng == 1)
                { //7 Planes V Formation
                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(0f, 8f), e1Enemies[0].transform.rotation);

                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(-2f, 8.5f), e1Enemies[0].transform.rotation);
                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(2f, 8.5f), e1Enemies[0].transform.rotation);

                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(-4f, 9f), e1Enemies[0].transform.rotation);
                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(4f, 9f), e1Enemies[0].transform.rotation);

                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(-6f, 9.5f), e1Enemies[0].transform.rotation);
                    Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(6f, 9.5f), e1Enemies[0].transform.rotation);
                }
                if (rng == 2) //3x2 Planes
                    for (int i = 0; i < 3; i++)
                    {
                        spawnPosition = -3f + 3f * i;
                        Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[0].transform.rotation);
                        Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(spawnPosition, 11f), e1Enemies[0].transform.rotation);
                    }
            }

            if (Manager.current.currentMissionSelected == 1)
            {
                int rng = Random.Range(0, 2); //-1
                if (rng == 0) //5 Planes
                    for (int i = 0; i < 5; i++)
                    {
                        Instantiate(e1Enemies[0], e1Enemies[0].transform.position = new Vector2(spawnPosition, 0f + 8f), e1Enemies[0].transform.rotation);

                        if (i % 2 == 0)
                            spawnPosition = (spawnPosition + 2);

                        spawnPosition *= -1;
                    }
                if (rng == 1) //3x2 Boats
                    for (int i = 0; i < 3; i++)
                    {
                        spawnPosition = -3f + 3f * i;
                        Instantiate(e1Enemies[1], e1Enemies[1].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[1].transform.rotation);
                        Instantiate(e1Enemies[1], e1Enemies[1].transform.position = new Vector2(spawnPosition, 11f), e1Enemies[1].transform.rotation);
                    }
            }
            if (Manager.current.currentMissionSelected == 2)
            {
                int rng = Random.Range(0, 3); //-1
                if (rng == 0)
                { //7 enemies
                    rng = Random.Range(0, 3); //-1
                    for (int i = 0; i < 7; i++)
                    {
                        spawnPosition = -6f + 2f * i; //FullscreenSpawn
                        Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[rng].transform.rotation);
                    }
                }
                else if (rng == 1)
                { //3x3 enemies
                    rng = Random.Range(0, 3); //-1
                    for (int i = 0; i < 3; i++)
                    {
                        spawnPosition = -4f + 4f * i;
                        Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[rng].transform.rotation);
                        Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(spawnPosition, 10f), e1Enemies[rng].transform.rotation);
                        Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(spawnPosition, 12f), e1Enemies[rng].transform.rotation);
                    }
                }
                else if (rng == 2)
                { //4 Planes
                    rng = Random.Range(0, 3); //-1
                    for (int i = 0; i < 4; i++)
                    {
                        spawnPosition = -3f + 2f * i;
                        Instantiate(e1Enemies[2], e1Enemies[2].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[rng].transform.rotation);
                    }
                }
            }
            if (Manager.current.currentMissionSelected == 3)
            {
                int rng = Random.Range(0, 3); //-1
                if (rng == 0) // 7 Boats
                    for (int i = 0; i < 7; i++)
                    {
                        spawnPosition = -6f + 2f * i;
                        Instantiate(e1Enemies[1], e1Enemies[1].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[1].transform.rotation);
                    }
                if (rng == 1)
                { //Flakship
                    Instantiate(e1Enemies[4], e1Enemies[4].transform.position = new Vector2(0f, 8f), e1Enemies[4].transform.rotation);
                }
                if (rng == 2)
                { //4 Boats
                    for (int i = 0; i < 2; i++)
                    {
                        spawnPosition = 0f - screenWidth / 3f;
                        Instantiate(e1Enemies[1], e1Enemies[1].transform.position = new Vector2(spawnPosition, 8f + i * 2), e1Enemies[1].transform.rotation);
                        Instantiate(e1Enemies[1], e1Enemies[1].transform.position = new Vector2(spawnPosition * -1, 8f + i * 2), e1Enemies[1].transform.rotation);
                    }
                }

            }
            if (Manager.current.currentMissionSelected == 4)
            {
                int rng = Random.Range(0, 2); //-1

                if (rng == 0)
                {    //6 rng enemies no flakships
                    rng = Random.Range(1, 4);

                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(0f, 8f), e1Enemies[rng].transform.rotation);

                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(-2f, 8.5f), e1Enemies[rng].transform.rotation);
                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(2f, 8.5f), e1Enemies[rng].transform.rotation);

                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(-4f, 9f), e1Enemies[rng].transform.rotation);
                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(4f, 9f), e1Enemies[rng].transform.rotation);

                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(-6f, 9.5f), e1Enemies[rng].transform.rotation);
                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(6f, 9.5f), e1Enemies[rng].transform.rotation);

                }
                else if (rng == 1)
                { //3 flakships
                    rng = 4;
                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(0f, 8f), e1Enemies[rng].transform.rotation);

                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(-2f, 8.5f), e1Enemies[rng].transform.rotation);
                    Instantiate(e1Enemies[rng], e1Enemies[rng].transform.position = new Vector2(2f, 8.5f), e1Enemies[rng].transform.rotation);
                }
            }
        }

        //############################################################ EPISODE II ############################################################

        if (Manager.current.currentMissionSelected > 4 && Manager.current.currentMissionSelected < 10)
        {

            if (Manager.current.currentMissionSelected == 5)
            {//VIP
                int rng = Random.Range(0, 2); //-1
                if (rng == 0) //7 enemies
                    for (int i = 0; i < 7; i++)
                    {
                        spawnPosition = -6f + 2f * i; //FullscreenSpawn

                        Instantiate(e2Enemies[0], e2Enemies[0].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e2Enemies[0].transform.rotation);
                    }
                if (rng == 1) //7 enemies
                    for (int i = 0; i < 3; i++)
                    {
                        spawnPosition = -3f + 3f * i;
                        Instantiate(e1Enemies[3], e1Enemies[3].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e1Enemies[3].transform.rotation);
                    }
            }

            if (Manager.current.currentMissionSelected == 6)
            {//Survive
                for (int i = 0; i < 7; i++)
                {
                    spawnPosition = -6f + 2f * i; //FullscreenSpawn
                    int rng = Random.Range(0, 2);
                    Instantiate(e2Enemies[rng], e2Enemies[rng].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e2Enemies[rng].transform.rotation);
                }
            }

            if (Manager.current.currentMissionSelected == 7)
            {//Rage
                int rng = Random.Range(0, 3);
                for (int i = 0; i < 7; i++)
                {
                    spawnPosition = -6f + 2f * i; //FullscreenSpawn
                    Instantiate(e2Enemies[rng], e2Enemies[rng].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e2Enemies[rng].transform.rotation);
                }
            }
            if (Manager.current.currentMissionSelected == 8)
            {//Objective
                int rng = Random.Range(0, 4); //-1
                if (rng == 0) //5 centered enemies
                    for (int i = 0; i < 5; i++)
                    {
                        Instantiate(e2Enemies[3], e2Enemies[3].transform.position = new Vector2(spawnPosition, 8f), e2Enemies[3].transform.rotation);

                        if (i % 2 == 0)
                            spawnPosition = (spawnPosition + 2);

                        spawnPosition *= -1;
                    }
                if (rng == 1) //3 enemies
                    for (int i = 0; i < 3; i++)
                    {
                        spawnPosition = -3f + 3f * i;
                        Instantiate(e2Enemies[4], e2Enemies[4].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e2Enemies[4].transform.rotation);
                    }
            }
            if (Manager.current.currentMissionSelected == 9)
            {//Boss
                for (int i = 0; i < 5; i++)
                {
                    int rng = Random.Range(0, 5);
                    Instantiate(e2Enemies[rng], e2Enemies[rng].transform.position = new Vector2(spawnPosition, i % 2 + 8f), e2Enemies[rng].transform.rotation);

                    if (i % 2 == 0)
                        spawnPosition = (spawnPosition + 2);

                    spawnPosition *= -1;
                }
            }
            //################################################################################################################################
        }
    }
}