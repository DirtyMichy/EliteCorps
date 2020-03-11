using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    //Viewport
    private Vector2 min, max;

    //float OBJECTIVESPAWN = 20; //Only for endlessMode
    float campaingObjectiveSpawn = 30;
    //Is being resettet in StartSpawnCoroutines()

    public GameObject[] bosses;
    public GameObject[] waveEnemies;
    public GameObject[] e1Enemies;
    public GameObject[] e2Enemies;
    public GameObject[] objectives;
    public GameObject[] extraObjectives;
    public int seconds = 0;
    int maxRngForWaves = 2;
    public GameObject cloud;
    public GameObject isle;

    public void Start()
    {
        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
    }

    public void StartSpawnCoroutines()
    {
        StartCoroutine("Spawn");

        campaingObjectiveSpawn = 30; // resetting
        maxRngForWaves = 2;
        seconds = 20; //20 so the objectives directly spawns
    }

    public void StopSpawnCoroutines()
    {
        StopCoroutine("Spawn"); //Stop spawning props and units
    }

    //Spawns enemies and counts seconds
    public IEnumerator Spawn()
    {
        //Loop indefinitely
        while (Manager.current.currentMenu == Manager.activeMenu.None)
        {

            GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");

            //Menuselection 0 = StoryMode, 1 = Endless, 2 = Survival, 3 = Exit
            //Modes 1 = StoryMode, 2 = Endless, 3 = Survival
            //1 = Kills, 2 = Seconds, 3 = Objectives, 4 = PlaneSeconds, 5 = KM
            if (Manager.current.gameMode == Manager.selectedGameMode.campaign)
            { //1 = CampaignMissions
                missionSpawns();
                yield return new WaitForSeconds(6f);
                //Spawn extraObjective for campaign
                if (Manager.current.missionMode == Manager.missionObjectives.killObjective)
                {
                    //Spawning additional Flaks
                    if (Manager.current.currentMissionSelected == 0 && Manager.current.objectiveKills < 4 && (seconds % campaingObjectiveSpawn) == 0)
                    {
                        Instantiate(extraObjectives[0], extraObjectives[0].transform.position = new Vector2(0f, 10f), extraObjectives[0].transform.rotation);
                        campaingObjectiveSpawn = 5; // For quicker missions
                    }
                    //Spawning additional Objective
                    if (Manager.current.currentMissionSelected == 2 && Manager.current.objectiveKills < 1 && (seconds % campaingObjectiveSpawn == 0))
                    {
                        Instantiate(objectives[0], objectives[0].transform.position = new Vector2(0f, 10f), objectives[0].transform.rotation);
                        campaingObjectiveSpawn = 20; // For quicker missions
                    }
                    //Spawning additional Objective
                    if (Manager.current.currentMissionSelected == 8 && Manager.current.objectiveKills < 3 && (seconds % campaingObjectiveSpawn / 2f == 0))
                    {
                        Instantiate(objectives[1], objectives[1].transform.position = new Vector2(0f, 10f), objectives[1].transform.rotation);
                        campaingObjectiveSpawn = 20; // For quicker missions
                    }
                }
            }

            if (Manager.current.gameMode == Manager.selectedGameMode.survive)
            {
                Waves();
                yield return new WaitForSeconds(10f / playerAlive.Length);
            }

            seconds++;
        }
    }

    void EndlessMissions()
    {
        int rng = Random.Range(0, 5); //-1

        GameObject[] enemies = new GameObject[10];

        for (int i = 0; i < 5; i++)
        {
            enemies[i] = e1Enemies[i];
        }
        for (int i = 5; i < 10; i++)
        {
            enemies[i] = e2Enemies[i - 5];
        }

        Instantiate(enemies[rng], enemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), enemies[rng].transform.rotation);
        Instantiate(enemies[rng], enemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), enemies[rng].transform.rotation);
        Instantiate(enemies[rng], enemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), enemies[rng].transform.rotation);
        Instantiate(enemies[rng], enemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), enemies[rng].transform.rotation);
    }

    void Waves()
    {
        if (seconds % 30 == 0)
            maxRngForWaves = Mathf.Clamp((int)(maxRngForWaves + 1), 0, waveEnemies.Length);
        int rng = Random.Range(0, maxRngForWaves); //-1

        Instantiate(waveEnemies[rng], waveEnemies[rng].transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 8f), waveEnemies[rng].transform.rotation);
    }

    public void spawnBoss01()
    {
        Instantiate(bosses[0], bosses[0].transform.position = new Vector2((max.x + min.x) / 2, 6f), bosses[0].transform.rotation);
    }

    public void spawnBoss02()
    {
        Instantiate(bosses[1], bosses[1].transform.position = new Vector2((max.x + min.x) / 2, -9f), bosses[1].transform.rotation);
    }

    void missionSpawns()
    {
        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float spawnPosition = 0; //FullscreenSpawn

        //############################################################ EPISODE I ############################################################

        //Debug.Log(maxRng);                    
        if (Manager.current.currentMissionSelected < 5)
        {
            //0-4 EasyPlane,AttackBoat,SmallPlane,SpyPlane,FlakShip
            float screenWidth = Mathf.Abs(min.x) + max.x;
            //            Debug.Log(screenWidth); 

            if (Manager.current.currentMissionSelected == 0)
            {
                int rng = Random.Range(0, 3); //-1
                if (rng == 0) //7 Planes
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