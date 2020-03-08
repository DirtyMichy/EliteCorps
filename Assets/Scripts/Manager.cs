using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;

//This is the manager for the game
public class Manager : MonoBehaviour
{
    private int maxObjectiveKills = 2;
    private int MAXSECONDS = 128;
    private int maxKills = 64;
    private int startedCampaign = 0; //0 means no saveFile exists   
    
    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Variables ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public int[] playersChosenCharacter;  //Chosen Character by Player 0 = Character01
    public int playerCount = 0;

    public enum missionObjectives
    {
        none,
        killAll,
        reachAndSurvive,
        killObjective,
        escortAndDefend,
    }

    public missionObjectives missionMode;
    
    public enum selectedGameMode
    {
        campaign,
        survive,
        pvp
    };

    public selectedGameMode gameMode;

    public int currentMainMenuSelection = 0;        //Used for iteration with arrow keys oder dpad inside the main menu
    public int currentMissionSelection = 0;         //Used for iteration with arrow keys oder dpad inside the mission menu

    public int kills = 0, secondsLeft = 300, objectiveKills = 0;

    public enum activeMenu
    {
        None,
        TitleScreen,
        ModeSelection,
        MissionSelection,
        CharSelection,
        Highscore
    }

    public activeMenu currentMenu;
    
    public int[] missionProgress; //[0] = E1M1, 0 = Disabled, 1 = Enabled, 2 = Won
       
    public static Manager current; //A public static reference to itself (make's it visible to other objects without a reference)

    public GameObject[] MissionIcons;
    public GameObject[] Characters;
    //All playable Characters
    public GameObject[] missionObject;
    //The player ship
    public GameObject[] player;
    //0 = Player1, ...
    public GameObject characterScreen;
    //The game object containing the title text
    public GameObject startScreen;
    public GameObject mainMenuScreen;
    public GameObject highScoreScreen;
    public GameObject ingameScoreScreen;
    public GameObject missionScreen;
    public GameObject explosion;
    public GameObject[] PlayerChosenChar;
    public GameObject spawnManager;
    public GameObject objective;
    public GameObject Cloud;
    public GameObject isle;
    public GameObject fader;
    //Black Sprite
    public GameObject backGround;
    //Background (Water,...)
    public GameObject bossHealthBar;
    public GameObject menuCam, ingameCam;
    //the menu uses other effects than ingame
    public GameObject escortPlane;
    public GameObject escortShip;
    public GameObject missionMarker;
    public GameObject campaignSieg;
    public GameObject powerUp;

    public RectTransform bossHealth;

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ UI Text Initialization ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public List<Text> ingameScoreGUIText;
    public List<Text> highScoreGUIText;
    public List<Text> playerActiveText;
    public List<Text> mainMenuGuiText;
    public Text objectiveText;

    //ingame status in upper right corner
    public Text missionText;
    public Text QuestText;
    public Text chosenEpisode;

    public bool menuActive = true;
    //True if any Menu is active
    public bool[] playerActive;
    //0 = Player1, ...
    public bool pressedDpad = false;
    //prevend fast menu scrolling
    public bool pressedButton = false;
    //prefend fast menu selection
    bool[] pressedPlayerDpad;
    public bool objectiveComplete = true;
    public bool fadeFinished = false;
    public bool playingCampaign = false;
    //If the player plays the campaign he shall return to the missionMenu after the Highscore
    bool bossSpawned = false;
    bool pressedArrow = false;

    int fadeDirection = -1;
    //-1 fadeIn (transparent), 1 fadeOut (darken)

    public AudioClip[] UImusic, BattleMusic;
    //Music
    public AudioSource[] UIBeeps;
    //Beeps for ButtonFeedBack
    public AudioClip[] DeathSounds;
    //Dying laughter sounds

    Vector2[] playerDpad;

    public string missionName = "";
    //Current Mission like E1M1
    public string[] mainMenuText;
    //0 = StoryMode, 1 = Survival, 2 = Exit

    public Sprite[] CharPreviews;
    //Character Previews
    public Sprite logoSprite, menuSprite, victorySprite, loseSprite, worldMap, MissionSpriteA, MissionSpriteB, MissionSpriteC;

    public int[] playerScore;
    //The player's score 0 = Player1, ...
    int gameTimeSeconds = 0;
    //current playTime in seconds

    void InstaWin()
    {
        kills = 666;
        secondsLeft = 0;
        objectiveKills = 666;
        objectiveComplete = true;
        StartCoroutine("showHighScore");
    }

    public void PlayDeathSound()
    {
        UIBeeps[3].clip = DeathSounds[Random.Range(0, DeathSounds.Length)];
        UIBeeps[3].Play();
    }

    void Awake()
    {
        PlayerPrefs.DeleteAll();

        missionProgress = new int[MissionIcons.Length];

        //If this isn't the first play, then there is a saveFile to load
        if (PlayerPrefs.GetInt("startedCampaign") > 0)
        {
            missionProgress = PlayerPrefsX.GetIntArray("missionProgress");
            //Debug.Log(missionProgress.Length);
            for (int i = 0; i < MissionIcons.Length; i++)
            {
                MissionIcons[i].GetComponent<Mission>().status = missionProgress[i];
            }
        }

        fadeDirection = -1;     //-1 fadeIn (transparent), 1 fadeOut (darken)
        StartCoroutine("Fade");

        UIBeeps = GetComponents<AudioSource>();
        //Raise Volume
        StartCoroutine("VolumeOn");

        //Ensure that there is only one manager
        if (current == null)
            current = this;
        else
            Destroy(gameObject);

        //Avoiding index out of bounds
        playerActive = new bool[4];
        playerScore = new int[4];
        player = new GameObject[4];
        playersChosenCharacter = new int[4];
        pressedPlayerDpad = new bool[4];
        playerDpad = new Vector2[4];

        for (int i = 0; i < playersChosenCharacter.Length; i++)
        {
            playersChosenCharacter[i] = 0;
            pressedPlayerDpad[i] = false;
            playerActive[i] = false;
            playerDpad[i] = new Vector2(0, 0);
        }
    }

    void Update()
    {
        //Check if all players are dead
        if (currentMenu == activeMenu.None)
        {
            //Count players alive
            if (CountPlayersAlive() == 0 && (!objectiveComplete || bossSpawned))
            {
                currentMenu = activeMenu.Highscore;
                StartCoroutine("showHighScore");
            }

            if (missionMode == missionObjectives.escortAndDefend)
            {
                GameObject[] escortPlaneCount = GameObject.FindGameObjectsWithTag("Escort");
                if (escortPlaneCount.Length == 0 && !objectiveComplete && !bossSpawned)
                {
                    currentMenu = activeMenu.Highscore;
                    StartCoroutine("showHighScore");
                }
            }

            UpdateIngameUI();
        }
        else
            MenuNavigation();

        //Back to MainMenu
        if (currentMenu != activeMenu.None && (GamePad.GetButton(GamePad.Button.Y, GamePad.Index.Any) || Input.GetKey(KeyCode.Y)) && !pressedButton && fadeFinished)
        {
            GotoMainMenu();
        }
    }

    void UpdateIngameUI()
    {
            //Update objective UI
            if (gameMode == selectedGameMode.campaign)
            {
                if (missionMode == missionObjectives.killAll)
                    objectiveText.text = kills + "/" + maxKills;
                if (missionMode == missionObjectives.escortAndDefend)
                    objectiveText.text = "" + secondsLeft;
                if (missionMode == missionObjectives.killObjective)
                    objectiveText.text = objectiveKills + "/" + maxObjectiveKills;
                if (missionMode == missionObjectives.reachAndSurvive)
                    objectiveText.text = "" + (secondsLeft * 10) + "KM";
            }
        
        //updating ingame score
        ingameScoreGUIText[0].text = "";
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i] != null)
                ingameScoreGUIText[i].text += "\n" + (i + 1) + ": " + playerScore[i];
        }
    }

    void UIBeepSounds()
    {
        UIBeeps[1].Play();
    }

    void UIBoomSound()
    {
        Instantiate(explosion, explosion.transform.position = new Vector2(-32f, -32f), explosion.transform.rotation);
    }

    //Makes navigation through the main menu possible
    void Dpad()
    {
        pressedArrow = true;             //for keyboardsupport
        pressedDpad = true;

        //-1 = None, 0 = TitleScreen, 1 = ModeSelection, 2 = MissionSelection, 3 = CharacterSelection, 4 = Highscore
        if (currentMenu == activeMenu.ModeSelection)
        {
            currentMainMenuSelection %= mainMenuText.Length;                //Avoid numbers bigger than the menu options
            if (currentMainMenuSelection < 0)
            {                               //and check if the numbers get negativ
                currentMainMenuSelection = mainMenuText.Length - 1;         //if so set the number to the last index
            }
            UIBeeps[2].Play();

            for (int i = 0; i < mainMenuGuiText.Count; i++)
            {
                mainMenuGuiText[i].color = new Color(255f, 255f, 255f, 255f);
                string tempString = mainMenuGuiText[i].text;
                tempString = tempString.Replace("✠", " ");
                mainMenuGuiText[i].text = tempString;
            }

            mainMenuGuiText[currentMainMenuSelection].color = new Color(255f, 0f, 0f, 255f);
            mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Remove(0, 1);
            mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Remove(mainMenuGuiText[currentMainMenuSelection].text.Length - 1, 1);
            mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Insert(0, "✠");
            mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Insert(mainMenuGuiText[currentMainMenuSelection].text.Length, "✠");
        }

        if (currentMenu == activeMenu.MissionSelection)
        {
            UIBeeps[1].Play();

            setMissionMarker(currentMissionSelection);
        }

        if (currentMenu == activeMenu.CharSelection)
        {
            UIBeeps[2].Play();
        }
    }
    //Setting the missionMode if gameMode is 1 and start spawncoroutines
    void GameStart()
    {
        menuCam.SetActive(false);
        ingameCam.SetActive(true);

        objectiveComplete = false;

        fadeDirection = -1;
        StartCoroutine("Fade");

        menuActive = false;

        //Deactivate the title and activate the player
        ingameScoreScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        characterScreen.SetActive(false);

        //0 = None, 1 = Mission, 2 = Survive
        if (gameMode == selectedGameMode.campaign)
        {
            playingCampaign = true;

            //Spawn level
            Instantiate(missionObject[currentMissionSelection], missionObject[currentMissionSelection].transform.position, missionObject[currentMissionSelection].transform.rotation);

            //missionMode 1 = Kills, 2 = Seconds, 3 = Objectives
            if (currentMissionSelection == 0)
            {
                maxObjectiveKills = 4;
                missionMode = missionObjectives.killAll;
                QuestText.text = "Zerstöre alle Flaks";
            }
            if (currentMissionSelection == 1)
            {
                missionMode = missionObjectives.escortAndDefend;
                QuestText.text = "Eskortiere das Schiff";

                //seconds /= 2;     //Escortmission have less seconds than survivemissions
                StartCoroutine("CountDown");
                Instantiate(escortShip, escortShip.transform.position = new Vector2(0f, -3f), escortShip.transform.rotation);
            }
            if (currentMissionSelection == 2)
            {
                maxObjectiveKills = 1;
                missionMode = missionObjectives.killObjective;
                QuestText.text = "Zerstöre die Bohrinsel";
            }
            if (currentMissionSelection == 3)
            {
                missionMode = missionObjectives.killAll;
                QuestText.text = "Zerstöre genug Schiffe";
            }
            if (currentMissionSelection == 4)
            {
                missionMode = missionObjectives.reachAndSurvive;
                QuestText.text = "Erreiche das Ziel";
                StartCoroutine("CountDown");
            }
            if (currentMissionSelection == 5)
            {
                missionMode = missionObjectives.escortAndDefend;
                QuestText.text = "Eskortiere die Flugzeuge";

                StartCoroutine("CountDown");
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(-3f, -3f), escortPlane.transform.rotation);
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(3f, -3f), escortPlane.transform.rotation);
            }
            if (currentMissionSelection == 6)
            {
                missionMode = missionObjectives.reachAndSurvive;
                QuestText.text = "Überlebe bis zum Ende";
                StartCoroutine("CountDown");
            }
            if (currentMissionSelection == 7)
            {
                missionMode = missionObjectives.killAll;
                QuestText.text = "Zerstöre die Gegner";
            }
            if (currentMissionSelection == 8)
            {
                maxObjectiveKills = 3;
                missionMode = missionObjectives.killObjective;
                QuestText.text = "Zerstöre alle Transporter";
            }
            if (currentMissionSelection == 9)
            {
                missionMode = missionObjectives.reachAndSurvive;
                QuestText.text = "Erreiche das Ziel";
                StartCoroutine("CountDown");
            }

            UpdateIngameUI();

            StartCoroutine("ShowQuest");
        }
        
        if (gameMode == selectedGameMode.survive)
        {
            playingCampaign = false;

            missionText.text = "";
            objectiveText.text = "";
            missionMode = 0;
        }

        GetComponent<AudioSource>().clip = BattleMusic[Random.Range(1, BattleMusic.Length)];
        GetComponent<AudioSource>().Play();
        StartCoroutine("VolumeOn");

        //Count player
        int playerCount = 0;
        for (int i = 0; i < player.Length; i++)
        {
            if (playerActive[i])
            {
                playerCount++;
            }
        }
        //Spawn the playerPlanes
        for (int i = 0; i < player.Length; i++)
        {
            if (playerActive[i])
            {
                player[i] = (GameObject)Instantiate(Characters[Mathf.Abs(playersChosenCharacter[i])], Characters[Mathf.Abs(playersChosenCharacter[i])].transform.position = new Vector2(1f, 0f), Characters[Mathf.Abs(playersChosenCharacter[i])].transform.rotation);
                player[i].SendMessage("SetPlayer", (i + 1));
                player[i].SendMessage("SetPlaneValue", (playerCount));
                player[i].GetComponent<PlayerMulti>().playerUIUpdate();
            }
        }

        objectiveComplete = false;              //Must be after the players are spawned

        StartCoroutine("AmmoRefill");

        spawnManager.GetComponent<EnemyManager>().StartSpawnCoroutines();
    }

    //Show the missionObjective at start
    IEnumerator ShowQuest()
    {
        QuestText.fontSize = 100;
        yield return new WaitForSeconds(2f);
        for (int i = 100; i > 0; i--)
        {
            QuestText.fontSize = i;
            yield return new WaitForSeconds(0.001f);
        }
        QuestText.text = "";
        StopCoroutine("ShowQuest");
    }

    //Fade in and out
    IEnumerator Fade()
    {
        fadeFinished = false;
        for (float i = 0; i != 100; i++)
        {
            yield return new WaitForSeconds(0.025f);

            if (fadeDirection == -1)
            {
                fader.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f - i / 100f);
            }
            else
            {
                fader.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, i / 100f);
            }
        }
        fadeFinished = true;
    }

    //Refilling players ammo
    IEnumerator AmmoRefill()
    {
        //Loop indefinitely
        while (true)
        {
            gameTimeSeconds++;
            GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < playerAlive.Length; i++)
            {
                if (playerAlive[i].GetComponent<PlayerMulti>())
                {
                    playerAlive[i].GetComponent<PlayerMulti>().ammo++;
                    if (gameTimeSeconds % 60 == 0)
                        playerAlive[i].GetComponent<PlayerMulti>().specialAmmo++;
                    playerAlive[i].GetComponent<PlayerMulti>().AmmoUIUpdate();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    //For objectiveMode 2
    IEnumerator CountDown()
    {
        for (; secondsLeft != 0;)
        {
            yield return new WaitForSeconds(1f);
            secondsLeft--;
        }
        CheckWinCondition();

    }

    IEnumerator VolumeOn()
    {
        for (float i = 0f; i <= 100f; i++)
        {
            GetComponent<AudioSource>().volume = i / 100f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator VolumeOff()
    {
        for (float i = 100f; i >= 0f; i--)
        {
            GetComponent<AudioSource>().volume = i / 100f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    //0 = None, 1 = Kill 100 Enemies, 2 = Survive 300 Seconds, 3 = Kill 3 Ojectives
    void CheckWinCondition()
    {
        if (!objectiveComplete)
        {
            if (gameMode == selectedGameMode.campaign)
            {
                GameObject[] escortPlane = GameObject.FindGameObjectsWithTag("Escort");

                if ((missionMode == missionObjectives.killAll && kills >= maxKills)
                                || (missionMode == missionObjectives.reachAndSurvive && secondsLeft <= 0)
                                || (missionMode == missionObjectives.killObjective && objectiveKills >= maxObjectiveKills)
                                || (missionMode == missionObjectives.escortAndDefend && secondsLeft <= 0 && escortPlane.Length >= 1))
                {
                    for (int i = 0; i < escortPlane.Length; i++)
                    {
                        escortPlane[i].GetComponent<Rigidbody2D>().velocity = (transform.up) * 4f;
                        escortPlane[i].GetComponent<PlayerMulti>().currentHP = 99999999;
                    }


                    if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisMission == 5 && !bossSpawned)
                    {
                        bossSpawned = true;
                        objectiveText.text = "BOSS";

                        GetComponent<AudioSource>().clip = UImusic[0];
                        GetComponent<AudioSource>().Play();

                        StartCoroutine("TriggerBossBattle");
                    }

                    GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                    if (playerAlive.Length > 0 && currentMenu == activeMenu.None && !bossSpawned)
                    {
                        objectiveComplete = true;
                        StartCoroutine("showHighScore");
                    }
                }
            }
        }
    }

    int CountPlayersAlive()
    {
        GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
        return playerAlive.Length;
    }

    IEnumerator TriggerBossBattle()
    {
        Debug.Log("TriggerBossBattle");

        objectiveComplete = false; //Otherwise the winscreeen would load upon death

        spawnManager.GetComponent<EnemyManager>().StopSpawnCoroutines();//Stop spawning props and units

        yield return new WaitForSeconds(5f);
        
        if (CountPlayersAlive() > 0)
        {
            for (float i = 100f; i >= 0f; i--)
            {
                GetComponent<AudioSource>().volume = i / 100f;
                yield return new WaitForSeconds(0.01f);
            }

            GetComponent<AudioSource>().clip = BattleMusic[0];
            GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().volume = 100f;

                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1)
                    spawnManager.GetComponent<EnemyManager>().spawnBoss01();

                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 2)
                    spawnManager.GetComponent<EnemyManager>().spawnBoss02();
  
            bossHealthBar.GetComponent<CanvasScaler>().scaleFactor = 0f;
            bossHealthBar.SetActive(true);

            for (int i = 0; i <= 100; i++)
            {
                bossHealthBar.GetComponent<CanvasScaler>().scaleFactor = i / 100f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        if (CountPlayersAlive() > 0)
        {
            yield return new WaitForSeconds(10f);
        }
        //the player can die within the last 10 seconds, so we have to check for bosses
        if (CountPlayersAlive() > 0)
        {
            if (bossSpawned && currentMenu == activeMenu.None)
            {
                //Controls change during BOSS01
                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1 )
                {//BOSS01
                    GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < playerAlive.Length; i++)
                    {
                        playerAlive[i].GetComponent<PlayerMulti>().wrap = true;
                    }

                    backGround.GetComponentInChildren<WaterAnimation>().moveOffset = false;
                }
                //BOSS02 doesnt need extra settings                
            }
        }
    }

    public void BossKilled()
    {
        if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisMission == 5)
        {
            objectiveComplete = true;
            StartCoroutine("showHighScore");

            //Boss gets instant winmusic
            GetComponent<AudioSource>().clip = UImusic[5];
            GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator showHighScore()
    {
        fadeFinished = false;
        fadeDirection = 1;
        for (float i = 0; i != 100; i++)
        {
            yield return new WaitForSeconds(0.025f);
            if (fadeDirection == -1)
            {
                fader.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f - i / 100f);
            }
            else
            {
                fader.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, i / 100f);
            }
        }

        menuCam.SetActive(true);
        ingameCam.SetActive(false);

        //0 = None, 1 = Mission, 2 = Survive
        if (objectiveComplete)
        {
            //Update missionProgress
            if (gameMode == selectedGameMode.campaign)
            {
                MissionIcons[currentMissionSelection].GetComponent<Mission>().status = 2;

                if (currentMissionSelection < MissionIcons.Length - 1)
                    if (MissionIcons[currentMissionSelection + 1].GetComponent<Mission>().status == 0)
                        MissionIcons[currentMissionSelection + 1].GetComponent<Mission>().status = 1;

                //Autoselect next Mission
                do
                {
                    if (currentMissionSelection < MissionIcons.Length - 1)
                        currentMissionSelection++;
                    else
                        currentMissionSelection = 0;
                } while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);

                Dpad();

                Save();
            }

            if (!bossSpawned)
            {
                GetComponent<AudioSource>().clip = UImusic[5];
                GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            GetComponent<AudioSource>().clip = UImusic[6];
            GetComponent<AudioSource>().Play();
        }

        bossHealthBar.SetActive(false);
        menuActive = true;

        currentMenu = activeMenu.Highscore;

        spawnManager.GetComponent<EnemyManager>().StopSpawnCoroutines();//Stop spawning props and units

        StopCoroutine("AmmoRefill");
        StopCoroutine("CountDown");

        //Remove all gamePlayAssets from Scene
        GameObject[] playerRemoval = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemyRemoval = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] propsRemoval = GameObject.FindGameObjectsWithTag("Props");
        GameObject[] islandRemoval = GameObject.FindGameObjectsWithTag("Island");
        GameObject[] escortRemoval = GameObject.FindGameObjectsWithTag("Escort");
        GameObject[] bulletRemoval = GameObject.FindGameObjectsWithTag("Bullet");
        GameObject[] levelRemoval = GameObject.FindGameObjectsWithTag("Level");
        GameObject[] powerUpRemoval = GameObject.FindGameObjectsWithTag("PowerUp");

        for (int i = 0; i < powerUpRemoval.Length; i++)
        {
            Destroy(powerUpRemoval[i]);
        }
        for (int i = 0; i < playerRemoval.Length; i++)
        {
            Destroy(playerRemoval[i]);
        }
        for (int i = 0; i < enemyRemoval.Length; i++)
        {
            Destroy(enemyRemoval[i]);
        }
        for (int i = 0; i < propsRemoval.Length; i++)
        {
            Destroy(propsRemoval[i]);
        }
        for (int i = 0; i < islandRemoval.Length; i++)
        {
            Destroy(islandRemoval[i]);
        }
        for (int i = 0; i < escortRemoval.Length; i++)
        {
            Destroy(escortRemoval[i]);
        }
        for (int i = 0; i < bulletRemoval.Length; i++)
        {
            Destroy(bulletRemoval[i]);
        }
        for (int i = 0; i < levelRemoval.Length; i++)
        {
            Destroy(levelRemoval[i]);
        }

        ingameScoreScreen.SetActive(false);
        highScoreScreen.SetActive(true);

        int highestScore = 0;
        int highestPlayer = 0;

        for (int i = 0; i < 4; i++)
        {
            if (playerActive[i])
                highScoreGUIText[i].text = " Spieler " + (i + 1) + " \n Punkte: \n" + playerScore[i];
            else
                highScoreGUIText[i].text = "";

            if (playerScore[i] > highestScore)
            {
                highestScore = playerScore[i];
                highestPlayer = i;
            }

            highScoreGUIText[i].transform.position = new Vector3(1f / (playerCount + 1f) * (i + 1), 0.6f, 0f);
        }

        resetScore();

        fadeDirection = -1;
        for (float i = 0; i != 100; i++)
        {
            yield return new WaitForSeconds(0.025f);
            if (fadeDirection == -1)
            {
                fader.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f - i / 100f);
            }
            else
            {
                fader.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, i / 100f);
            }
        }
        fadeFinished = true;

    }

    //Adding points to the score of the specific player
    public void AddPoint(int point, int owner, bool isObjective)
    {
        //Add points to the player's score
        if (owner != 0)
            playerScore[(owner - 1)] += point;

        if (isObjective)
            objectiveKills++;

        kills++;
        CheckWinCondition();
    }

    //Resets the score for every mission
    void resetScore()
    {
        for (int i = 0; i < 4; i++)
        {
            playerScore[i] = 0;
        }

        gameTimeSeconds = 0;

        kills = 0;
        objectiveKills = 0;
        secondsLeft = MAXSECONDS;
        bossSpawned = false;

        if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1 && backGround.GetComponentInChildren<WaterAnimation>())
        {
            backGround.GetComponentInChildren<WaterAnimation>().moveOffset = true;
        }
    }
    
    public void Save()
    {
        for (int i = 0; i < MissionIcons.Length; i++)
        {
            missionProgress[i] = MissionIcons[i].GetComponent<Mission>().status;
        }
        //Save the progress to the player prefs
        PlayerPrefsX.SetIntArray("missionProgress", missionProgress);


        if (startedCampaign == 0)
            PlayerPrefs.SetInt("startedCampaign", (startedCampaign + 1));

        PlayerPrefs.Save();
    }

    //Everything for the menu navigation
    void MenuNavigation()
    {
        //Menu Navigations -1 = playing
        if (currentMenu != activeMenu.None)
        {
            GamepadInput.GamePad.Index[] gamePadIndex;
            gamePadIndex = new GamepadInput.GamePad.Index[4];
            gamePadIndex[0] = GamePad.Index.One;
            gamePadIndex[1] = GamePad.Index.Two;
            gamePadIndex[2] = GamePad.Index.Three;
            gamePadIndex[3] = GamePad.Index.Four;

            //CharacterSelection
            if (currentMenu == activeMenu.CharSelection)
            {
                for (int i = 0; i < player.Length; i++)
                {
                    playerDpad[i] = GamePad.GetAxis(GamePad.Axis.Dpad, gamePadIndex[i]);

                    if ((GamePad.GetButton(GamePad.Button.A, gamePadIndex[i])) && !playerActive[i])
                    {
                        playerCount++;
                        playerActive[i] = true;
                        playerActiveText[i].text = "Spieler " + (i + 1) + ": \n Aktiv";
                        PlayerChosenChar[i].SetActive(true);
                        UIBeepSounds();
                    }
                    if ((GamePad.GetButton(GamePad.Button.B, gamePadIndex[i])) && playerActive[i])
                    {
                        playerCount--;
                        playerActive[i] = false;
                        playerActiveText[i].text = "Spieler " + (i + 1) + ": \n Inaktiv";
                        PlayerChosenChar[i].SetActive(false);
                        UIBeepSounds();
                    }
                    if ((playerDpad[i].y < 0f) && !pressedPlayerDpad[i])
                    {
                        pressedPlayerDpad[i] = true;
                        playersChosenCharacter[i]++;
                        playersChosenCharacter[i] %= Characters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playersChosenCharacter[i])];
                        UIBeeps[1].Play();
                    }
                    if ((playerDpad[i].y > 0f) && !pressedPlayerDpad[i])
                    {
                        pressedPlayerDpad[i] = true;
                        playersChosenCharacter[i]--;
                        playersChosenCharacter[i] %= Characters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playersChosenCharacter[i])];
                        UIBeeps[1].Play();
                    }
                    if (playerDpad[i].y == 0f)
                    {
                        pressedPlayerDpad[i] = false;
                    }
                }

                //Keyboard for Player 01
                if ((Input.GetKey(KeyCode.A)) && !playerActive[0])
                {
                    playerCount++;
                    playerActive[0] = true;
                    playerActiveText[0].text = "Spieler " + (1) + ": \n Aktiv";
                    PlayerChosenChar[0].SetActive(true);
                    UIBeepSounds();
                }
                if ((Input.GetKey(KeyCode.B)) && playerActive[0])
                {
                    playerCount--;
                    playerActive[0] = false;
                    playerActiveText[0].text = "Spieler " + (1) + ": \n Inaktiv";
                    PlayerChosenChar[0].SetActive(false);
                    UIBeepSounds();
                }
                if (Input.GetKey(KeyCode.DownArrow) && !pressedArrow)
                {
                    pressedArrow = true;
                    playersChosenCharacter[0]++;
                    playersChosenCharacter[0] %= Characters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playersChosenCharacter[0])];
                }
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow)
                {
                    pressedArrow = true;
                    playersChosenCharacter[0]--;
                    playersChosenCharacter[0] %= Characters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playersChosenCharacter[0])];
                }
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    pressedArrow = false;
                }
            }


            //PlayerAny          
            Vector2 playerAnyDpad = GamePad.GetAxis(GamePad.Axis.Dpad, GamePad.Index.Any);

            //################################Navigate down the MainMenu################################
            if (currentMenu == activeMenu.ModeSelection || currentMenu == activeMenu.MissionSelection)
            {
                if ((playerAnyDpad.y < 0f) && !pressedDpad)
                { //&& currentMenu==1 ?
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection++;
                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            if (currentMissionSelection < MissionIcons.Length - 1)
                                currentMissionSelection++;
                            else
                                currentMissionSelection = 0;
                        } while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);
                    Dpad();
                }
                //Navigate up the MainMenu
                if ((playerAnyDpad.y > 0f) && !pressedDpad)
                {
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection--;
                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            if (currentMissionSelection > 0)
                                currentMissionSelection--;
                            else
                                currentMissionSelection = MissionIcons.Length - 1;
                        } while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);

                    Dpad();
                }
                if ((playerAnyDpad.y == 0f) && pressedDpad)
                {
                    pressedDpad = false;
                }

                //################################Keyboardsupport################################
                if (Input.GetKey(KeyCode.DownArrow) && !pressedArrow)
                {
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection++;
                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            if (currentMissionSelection > 0)
                                currentMissionSelection--;
                            else
                                currentMissionSelection = MissionIcons.Length - 1;
                        } while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);

                    Dpad();
                }
                //Keyboardsupport
                //Navigate up the MainMenu
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow)
                { //&& currentMenu==1 ?
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection--;
                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            if (currentMissionSelection < MissionIcons.Length - 1)
                                currentMissionSelection++;
                            else
                                currentMissionSelection = 0;
                            //Debug.Log("Searching: " + currentMissionSelection);
                        } while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);
                    Dpad();
                }
            }

            //Keyboardsupport
            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                pressedArrow = false;
            }

            //Menuselection 0 = StoryMode, 1 = Endless, 2 = Survival, 3 = Exit
            //Modes 1 = StoryMode, 2 = Endless, 3 = Survival
            if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && currentMenu == activeMenu.ModeSelection && !pressedButton)
            {
                pressedButton = true;
                UIBoomSound();
                //StoryMode
                if (currentMainMenuSelection == 0)
                {
                    GotoMissionMenu();
                    gameMode = selectedGameMode.campaign;
                }
                //SurvivalMode
                if (currentMainMenuSelection == 1)
                {
                    GotoSelectionScreen();
                    gameMode = selectedGameMode.survive;
                    backGround.GetComponent<BackGroundManager>().SetBackground(1);
                }
                //Exit to desktop
                if (currentMainMenuSelection == 2)
                {
                    Application.Quit();
                }
            }

            //missionSelection
            if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && currentMenu == activeMenu.MissionSelection && !pressedButton)
            {
                pressedButton = true;
                UIBoomSound();

                backGround.GetComponent<BackGroundManager>().SetBackground(MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode); //Episodes start with 1,2,...

                GotoSelectionScreen();
            }

            //Setting to false after Button pressed to prefend fast menu scrolling
            if (GamePad.GetButtonUp(GamePad.Button.A, GamePad.Index.Any) || GamePad.GetButtonUp(GamePad.Button.Y, GamePad.Index.Any) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Y))
            {
                pressedButton = false;
            }
        }

        //Continue Mission or try again
        if (currentMenu == activeMenu.Highscore && (GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton && fadeFinished)
        {
            pressedButton = true;
            UIBoomSound();

            if (playingCampaign)
                GotoMissionMenu();
            else
                GotoSelectionScreen();
        }

        //Get into the MainMenu
        if (currentMenu == 0 && !pressedButton && (GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.A)))
        {
            GotoMainMenu();
        }

        //Start the game if it isn't already going and the player presses the key
        if (((GamePad.GetButton(GamePad.Button.Start, GamePad.Index.Any) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && currentMenu == activeMenu.CharSelection && menuActive && (playerActive[0] || playerActive[1] || playerActive[2] || playerActive[3])))
        {
            currentMenu = activeMenu.None;
            menuActive = false;
            GameStart();
        }
    }

    //Setting the selected Mission in campaign
    void setMissionMarker(int selectedCampaignMission)
    {
        iTween.MoveTo(missionMarker, iTween.Hash("position", MissionIcons[selectedCampaignMission].transform.position, "easeType", "linear", "time", .5f));

        int selectedCampaignEpisode = selectedCampaignMission / 5 + 1;

        chosenEpisode.text = "Episode: " + selectedCampaignEpisode + "   Mission: " + ((selectedCampaignMission % 5) + 1);
        missionText.text = "e" + selectedCampaignEpisode + "m" + ((selectedCampaignMission % 5) + 1);
    }

    //Navigating to the main menu
    void GotoMainMenu()
    {
        playingCampaign = false;
        if (currentMenu == activeMenu.Highscore)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }
        missionMarker.SetActive(false);
        pressedButton = true;   //Prefend accidental menu navigation
        currentMenu = activeMenu.ModeSelection;
        currentMainMenuSelection = 0; //Default is 1 -> Misson

        mainMenuScreen.SetActive(true); //Activate GUI Text
        startScreen.SetActive(false);   //Deactivate old GUI
        characterScreen.SetActive(false);
        highScoreScreen.SetActive(false);
        missionScreen.SetActive(false);
        
        UIBoomSound();

        //Reset menu colorization
        for (int i = 0; i < mainMenuGuiText.Count; i++)
        {
            mainMenuGuiText[i].color = new Color(255f, 255f, 255f, 255f);
            string tempString = mainMenuGuiText[i].text;
            tempString = tempString.Replace("✠", " ");
            mainMenuGuiText[i].text = tempString;
        }

        mainMenuGuiText[currentMainMenuSelection].color = new Color(255f, 0f, 0f, 255f);
        mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Remove(0, 1);
        mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Remove(mainMenuGuiText[currentMainMenuSelection].text.Length - 1, 1);
        mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Insert(0, "✠");
        mainMenuGuiText[currentMainMenuSelection].text = mainMenuGuiText[currentMainMenuSelection].text.Insert(mainMenuGuiText[currentMainMenuSelection].text.Length, "✠");
    }

    //Navigating to the campaign menu
    void GotoMissionMenu()
    {
        bool sieg = true;

        for (int i = 0; i < MissionIcons.Length; i++)
        {
            if (MissionIcons[i].GetComponent<Mission>().status != 2)
                sieg = false;
        }

        if (currentMenu == activeMenu.Highscore && !sieg)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }
        if (sieg)
        {
            campaignSieg.SetActive(true);
            GetComponent<AudioSource>().clip = UImusic[1];
            GetComponent<AudioSource>().Play();
        }

        for (int i = 0; i < MissionIcons.Length; i++)
        {
            if (MissionIcons[i].GetComponent<Mission>().status == 1)
                MissionIcons[i].GetComponent<SpriteRenderer>().sprite = MissionSpriteC;
            if (MissionIcons[i].GetComponent<Mission>().status == 2)
                MissionIcons[i].GetComponent<SpriteRenderer>().sprite = MissionSpriteB;
        }



        missionMarker.SetActive(true);
        mainMenuScreen.SetActive(false);
        highScoreScreen.SetActive(false);
        missionScreen.SetActive(true);
        currentMenu = activeMenu.MissionSelection;

        Dpad();
    }

    //Navigating to the selection menu
    void GotoSelectionScreen()
    {
        if (currentMenu == activeMenu.Highscore)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }
        highScoreScreen.SetActive(false);
        missionMarker.SetActive(false);
        mainMenuScreen.SetActive(false);
        missionScreen.SetActive(false);

        characterScreen.SetActive(true);

        currentMenu = activeMenu.CharSelection;
    }

}