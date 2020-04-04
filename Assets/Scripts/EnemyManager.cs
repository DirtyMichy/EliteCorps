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
    public GameObject objectiveToKill;

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
        seconds = 0;
        timeBetweenSpawns = 20 / Manager.current.playerCount;

        //Level specific settings
        switch (Manager.current.currentMissionSelected)
        {
            case 0:
                campaingObjectiveSpawn = 15;
                objectiveToKill = objectives[Random.Range(0,4)];
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
        Debug.Log("StartCoroutineSpawn");
        while (Manager.current.currentMenu == Manager.activeMenu.None)
        {
            Debug.Log("StartCoroutineSpawn");
            Debug.Log("Spawning");

            if (Manager.current.gameMode == Manager.selectedGameMode.campaign && seconds % timeBetweenSpawns == 0)
            {
                missionSpawns();
            }

            if (Manager.current.gameMode == Manager.selectedGameMode.survive)
            {
                Waves();
            }

            yield return new WaitForSeconds(1f);

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
            if (Manager.current.objectiveKills < Manager.current.requiredObjectivesToKill && (seconds % campaingObjectiveSpawn == 0) && (seconds > 60))
            {
                int episode = Manager.current.currentMissionSelected;
                Instantiate(objectiveToKill);
                campaingObjectiveSpawn = 10;
            }
        }

        //############################################################ EPISODE I ############################################################

        if (Manager.current.currentMissionSelected < 5)
        {
            //Destroy 4 bunkers
            if (Manager.current.currentMissionSelected == 0)
            {
                Instantiate(enemyBlobs[4]);
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
                //Escort 2 Planes
                if (Manager.current.currentMissionSelected == 5)
                {
                    Instantiate(enemyBlobs[0], enemyBlobs[0].transform.position, enemyBlobs[0].transform.rotation);
                }

                //Survive 
                if (Manager.current.currentMissionSelected == 6)
                {
                    Instantiate(enemyBlobs[Random.Range(0, 2)], enemyBlobs[1].transform.position, enemyBlobs[1].transform.rotation);
                }

                //Kill all
                if (Manager.current.currentMissionSelected == 7)
                {
                    Instantiate(enemyBlobs[Random.Range(0, 3)], enemyBlobs[1].transform.position, enemyBlobs[1].transform.rotation);
                }

                //Destroy 3 fortresses
                if (Manager.current.currentMissionSelected == 8)
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
                if (Manager.current.currentMissionSelected == 9)
                {
                    int rng = Random.Range(0, 2); //-1

                    if (rng == 0)
                    {
                        Instantiate(enemyBlobs[Random.Range(0, 2)], enemyBlobs[1].transform.position, enemyBlobs[1].transform.rotation);
                    }
                }

                //################################################################################################################################
            }
        }
    }
}