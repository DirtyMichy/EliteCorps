using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    private Vector2 min, max;
    private float campaingObjectiveSpawn = 30;
    private float timeBetweenSpawns = 20;
    private int maxRngForWaves = 2;
    public int seconds = 0;
    public GameObject[] bosses;
    public GameObject[] enemies;
    public GameObject[] enemyBlobs;
    public GameObject[] e1Enemies;
    public GameObject[] e2Enemies;
    public GameObject[] objectives;
    private GameObject objectiveToKill;

    public void Start()
    {
        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
    }

    public void StartSpawnCoroutines()
    {
        StartCoroutine("Spawn");

        //global seetings
        maxRngForWaves = 2;
        seconds = 20;
        timeBetweenSpawns = 20 / Manager.current.playerCount;

        //Level specific settings
        switch (Manager.current.currentMissionSelected)
        {
            case 0:
                campaingObjectiveSpawn = 15;
                objectiveToKill = objectives[2];
                break;
            case 2:
                campaingObjectiveSpawn = 30;
                objectiveToKill = objectives[0];
                break;
            case 3:
                timeBetweenSpawns = 4 / Manager.current.playerCount;
                break;
            case 8:
                campaingObjectiveSpawn = 20;
                objectiveToKill = objectives[1];
                break;
            default:
                break;
        }
    }

    public void StopSpawnCoroutines()
    {
        StopCoroutine("Spawn");
    }

    void Waves()
    {
        if (seconds % 30 == 0)
            maxRngForWaves = Mathf.Clamp((int)(maxRngForWaves + 1), 0, enemies.Length);
        int rng = Random.Range(0 + (maxRngForWaves / 4), maxRngForWaves); //-1

        Instantiate(enemies[rng], enemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), enemies[rng].transform.rotation);
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
                yield return new WaitForSeconds(timeBetweenSpawns);
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
            //Destroy 4 bunkers
            if (Manager.current.currentMissionSelected == 0)
            {
                Instantiate(enemyBlobs[0], enemyBlobs[0].transform.position, enemyBlobs[0].transform.rotation);
            }

            //Escort boat
            if (Manager.current.currentMissionSelected == 1)
            {
                Instantiate(enemyBlobs[Random.Range(0, 2)], enemyBlobs[1].transform.position, enemyBlobs[1].transform.rotation);
            }

            //Destroy drillingPit
            if (Manager.current.currentMissionSelected == 2)
            {
                Instantiate(enemyBlobs[Random.Range(0, 3)], enemyBlobs[1].transform.position, enemyBlobs[1].transform.rotation);
            }

            //Destroy all ships
            if (Manager.current.currentMissionSelected == 3)
            {
                GameObject[] enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy");
                spawnPosition = Random.Range(min.x + 1, max.x - 1);
                int rng = Random.Range(0, 8);

                if (enemiesAlive.Length < Manager.current.requiredKillsToWin - Manager.current.kills)
                    if (rng == 0)
                        Instantiate(e1Enemies[4], e1Enemies[4].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[4].transform.rotation);
                    else
                        Instantiate(e1Enemies[1], e1Enemies[1].transform.position = new Vector2(spawnPosition, 8f), e1Enemies[1].transform.rotation);

            }

            //Boss
            if (Manager.current.currentMissionSelected == 4)
            {
                int rng = Random.Range(0, 2); //-1

                if (rng == 0)
                {
                    Instantiate(enemyBlobs[Random.Range(0, 2)], enemyBlobs[1].transform.position, enemyBlobs[1].transform.rotation);
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
}