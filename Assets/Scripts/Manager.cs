using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;

//This is the manager for the game
public class Manager : MonoBehaviour
{
    int maxObjectiveKills = 2;
    int MAXSECONDS = 128;
    //int BOSSMISSION = 5;
    int maxKills = 64;
    int startedCampaign = 0;                    //0 means no saveFile exists
    int endlessModeRNGBoss = 666;               //Random bosses spawn in endlessMode, 666 means no Boss, 0 means Boss01,...
    public int[] playerChoice;                  //Chosen Character by Player 0 = Character01

    public int playerCount = 0;                 //PlayerCounter 0 = no active Player
    public int missionMode = 0;                 //0 = None, 1 = Kill Enemies, 2 = Survive x seconds, 3 = Kill Ojectives, 4 = escort
    public int gameMode = 0;                    //0 = None, 1 = Mission, 2 = Survive
    public int currentMainMenuSelection = 0;    //0 = StoryMode, 1 = Survival, 2 = Exit
    public int currentMissionSelection = 0;
    public int kills = 0, seconds = 300, objectiveKills = 0;                                                     //For GameModes
    public int currentMenu = 0;                 //-1 = None, 0 = TitleScreen, 1 = ModeSelection, 2 = MissionSelection, 3 = CharacterSelection, 4 = Highscore

    //public int episode = 1, mission = 1, campaignMission = 0, campaignProgress = 1;

    public int[] missionProgress;     //[0] = E1M1, 0 = Disabled, 1 = Enabled, 2 = Won 
    public int endlessProgress;                 //Every x Mission the enemytypes get mixed again

    public static Manager current;              //A public static reference to itself (make's it visible to other objects without a reference)

    public GameObject[] MissionIcons;
    public GameObject[] Characters;             //All playable Characters
    public GameObject[] missionObject;                                                  //The player ship
    public GameObject[] player;                 //0 = Player1, ...
    public GameObject characterScreen;          //The game object containing the title text
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
    public GameObject fader;                    //Black Sprite
    public GameObject backGround;                    //Background (Water,...)
    public GameObject bossHealthBar;
    public GameObject menuCam, ingameCam;       //the menu uses other effects than ingame
    public GameObject escortPlane;
    public GameObject escortShip;
    public GameObject missionMarker;
    public GameObject cross;
    public GameObject campaignSieg;
    public GameObject powerUp;

    public RectTransform bossHealth;

    public GUIText[] ingameScoreGUIText;        //The score text
    public GUIText[] highScoreGUIText;          //The high score text
    public GUIText[] playerActiveText;
    public GUIText[] mainMenuGuiText;           //Start, Options, Exit...
    public GUIText objectiveText;               //ingame status in upper right corner
    public GUIText missionText;                 //e1m1
    public GUIText QuestText;
    public GUIText chosenEpisode;

    public bool menuActive = true;              //True if any Menu is active
    public bool[] playerActive;                 //0 = Player1, ...
    public bool pressedDpad = false;            //prevend fast menu scrolling
    public bool pressedButton = false;          //prefend fast menu selection
    bool[] pressedPlayerDpad;
    public bool objectiveComplete = true;
    public bool fadeFinished = false;
    public bool playingCampaign = false;        //If the player plays the campaign he shall return to the missionMenu after the Highscore
    bool bossSpawned = false;
    bool pressedArrow = false;

    int fadeDirection = -1;                     //-1 fadeIn (transparent), 1 fadeOut (darken)

    public AudioClip[] UImusic, BattleMusic;    //Music
    public AudioSource[] UIBeeps;               //Beeps for ButtonFeedBack
    public AudioClip[] DeathSounds;           //Dying laughter sounds

    Vector2[] playerDpad;

    public string missionName = "";             //Current Mission like E1M1
    public string[] mainMenuText;               //0 = StoryMode, 1 = Survival, 2 = Exit

    public Sprite[] CharPreviews;               //Character Previews
    public Sprite logoSprite, menuSprite, victorySprite, loseSprite, worldMap, MissionSpriteA, MissionSpriteB, MissionSpriteC;
    Vector2 min;                                //Viewport
    Vector2 max;                                //Viewport

    public int[] playerScore;                   //The player's score 0 = Player1, ...
    int gameTimeSeconds = 0;                    //current playTime in seconds

    void InstaWin()
    {
        kills = 666;
        seconds = 0;
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
        //PlayerPrefs.DeleteAll();

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
        playerChoice = new int[4];
        pressedPlayerDpad = new bool[4];
        playerDpad = new Vector2[4];

        for (int i = 0; i < playerChoice.Length; i++)
        {
            playerChoice[i] = 0;
            pressedPlayerDpad[i] = false;
            playerActive[i] = false;
            playerDpad[i] = new Vector2(0, 0);
        }
    }

    void Update()
    {
        //Check if all players are dead
        if (currentMenu == -1)
        {
            //Count players alive
            if (CountPlayersAlive() == 0 && (!objectiveComplete || bossSpawned))
            {
                currentMenu = 4;
                StartCoroutine("showHighScore");
            }

            if (missionMode == 4)
            {
                GameObject[] escortPlaneCount = GameObject.FindGameObjectsWithTag("Escort");
                if (escortPlaneCount.Length == 0 && !objectiveComplete && !bossSpawned)
                {
                    currentMenu = 4;
                    StartCoroutine("showHighScore");
                }
            }
        }

        //Back to MainMenu
        if (currentMenu != -1 && (GamePad.GetButton(GamePad.Button.Y, GamePad.Index.Any) || Input.GetKey(KeyCode.Y)) && !pressedButton && fadeFinished)
        {
            GotoMainMenu();
        }

        UpdateGUI();
        MenuNavigation();

        if (Input.GetKey(KeyCode.P) && !objectiveComplete)
        {
            InstaWin();
        }
    }

    void UpdateGUI()
    {
        if (bossSpawned)
        {
            objectiveText.text = "BOSS";
        }
        else
        {
            //Update objective GUI, 1 = Kills, 2 = Seconds, 3 = Objectives, 4 = PlaneSeconds, 5 = KM
            if (gameMode == 1)
            {
                if (missionMode == 1)
                    objectiveText.text = kills + "/" + maxKills;
                if (missionMode == 2 || missionMode == 4)
                    objectiveText.text = "" + seconds;
                if (missionMode == 3)
                    objectiveText.text = objectiveKills + "/" + maxObjectiveKills;
                if (missionMode == 5)
                    objectiveText.text = "" + (seconds * 10) + "KM";
            }
            if (gameMode == 2)
            {
                if (missionMode == 1)
                    objectiveText.text = kills + "/" + maxKills;
                /*
                if (missionMode == 2 || missionMode == 4)
                    objectiveText.text = "" + seconds;
                if (missionMode == 3)
                    objectiveText.text = objectiveKills + "/" + maxObjectiveKills;
                */
            }
        }

        //updating ingame score
        ingameScoreGUIText[0].text = "";
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i] != null)
                ingameScoreGUIText[0].text += "\n" + (i + 1) + ": " + playerScore[i];
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
        if (currentMenu == 1)
        {
            currentMainMenuSelection %= mainMenuText.Length;                //Avoid numbers bigger than the menu options
            if (currentMainMenuSelection < 0)                               //and check if the numbers get negativ
            {
                currentMainMenuSelection = mainMenuText.Length - 1;         //if so set the number to the last index
            }
            UIBeeps[2].Play();

            for (int i = 0; i < mainMenuGuiText.Length; i++)
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

        if (currentMenu == 2)
        {
            UIBeeps[1].Play();

            setMissionMarker(currentMissionSelection);
        }

        if (currentMenu == 3)
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
        GetComponent<SpriteRenderer>().sprite = null;

        menuActive = false;

        //Deactivate the title and activate the player
        ingameScoreScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        characterScreen.SetActive(false);

        //0 = None, 1 = Mission, 2 = Survive, 3 Endless Mission
        if (gameMode == 1)
        {
            playingCampaign = true;

            //Spawn level
            Instantiate(missionObject[currentMissionSelection], missionObject[currentMissionSelection].transform.position, missionObject[currentMissionSelection].transform.rotation);

            //missionMode 1 = Kills, 2 = Seconds, 3 = Objectives
            if (currentMissionSelection == 0)
            {
                maxObjectiveKills = 4;
                missionMode = 3;
                QuestText.text = "Zerstöre alle Flaks";
            }
            if (currentMissionSelection == 1)
            {
                missionMode = 4;
                QuestText.text = "Eskortiere das Schiff";

                //seconds /= 2;     //Escortmission have less seconds than survivemissions
                StartCoroutine("CountDown");
                Instantiate(escortShip, escortShip.transform.position = new Vector2(0f, -3f), escortShip.transform.rotation);
            }
            if (currentMissionSelection == 2)
            {
                maxObjectiveKills = 1;
                missionMode = 3;
                QuestText.text = "Zerstöre die Bohrinsel";
            }
            if (currentMissionSelection == 3)
            {
                missionMode = 1;
                QuestText.text = "Zerstöre genug Schiffe";
            }
            if (currentMissionSelection == 4)
            {
                missionMode = 5;
                QuestText.text = "Erreiche das Ziel";
                StartCoroutine("CountDown");
            }
            if (currentMissionSelection == 5)
            {
                missionMode = 4;
                QuestText.text = "Eskortiere die Flugzeuge";

                StartCoroutine("CountDown");
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(-3f, -3f), escortPlane.transform.rotation);
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(3f, -3f), escortPlane.transform.rotation);
            }
            if (currentMissionSelection == 6)
            {
                missionMode = 2;
                QuestText.text = "Überlebe bis zum Ende";
                StartCoroutine("CountDown");
            }
            if (currentMissionSelection == 7)
            {
                missionMode = 1;
                QuestText.text = "Zerstöre die Gegner";
            }
            if (currentMissionSelection == 8)
            {
                maxObjectiveKills = 3;
                missionMode = 3;
                QuestText.text = "Zerstöre alle Transporter";
            }
            if (currentMissionSelection == 9)
            {
                missionMode = 5;
                QuestText.text = "Erreiche das Ziel";
                StartCoroutine("CountDown");
            }

            UpdateGUI();

            StartCoroutine("ShowQuest");
        }

        if (gameMode == 2)
        {
            playingCampaign = false;

            //Spawn some clouds
            min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            for (int i = 0; i < 10; i++)
            {
                Instantiate(Cloud, Cloud.transform.position = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y)), Cloud.transform.rotation);
            }

            //maxObjectiveKills = 2;
            //0 = None, 1 = Kill 100 Enemies, 2 = Survive 300 Seconds, 3 = Kill 3 Ojectives 
            missionMode = 1;
            missionText.text = "Mission: " + endlessProgress+1;
            //missionMode = 4;
            if (missionMode == 1)
            {
                QuestText.text = "Zerstöre alles";
            }
            /*
            if (missionMode == 2)
            {
                StartCoroutine("CountDown");
                QuestText.text = "Überlebe";
            }

            if (missionMode == 3)
            {
                QuestText.text = "Zerstöre die Ziele";
            }

            if (missionMode == 4)
            {
                seconds /= 2;     //Escortmission have less seconds than survivemissions
                StartCoroutine("CountDown");
                QuestText.text = "Eskortiere";
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(0f, -3f), escortPlane.transform.rotation);
            }
            */
            UpdateGUI();

            StartCoroutine("ShowQuest");
        }

        if (gameMode == 3)//3 = Survive
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
                player[i] = (GameObject)Instantiate(Characters[Mathf.Abs(playerChoice[i])], Characters[Mathf.Abs(playerChoice[i])].transform.position = new Vector2(1f, 0f), Characters[Mathf.Abs(playerChoice[i])].transform.rotation);
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
        for (; seconds != 0;)
        {
            yield return new WaitForSeconds(1f);
            seconds--;
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
            if (gameMode == 1)
            {
                GameObject[] escortPlane = GameObject.FindGameObjectsWithTag("Escort");

                if ((missionMode == 1 && kills >= maxKills)
                    || (missionMode == 2 && seconds <= 0)
                    || (missionMode == 5 && seconds <= 0)
                    || (missionMode == 3 && objectiveKills >= maxObjectiveKills)
                    || (missionMode == 4 && seconds <= 0 && escortPlane.Length >= 1))
                {
                    for (int i = 0; i < escortPlane.Length; i++)
                    {
                        escortPlane[i].GetComponent<Rigidbody2D>().velocity = (transform.up) * 4f;
                        escortPlane[i].GetComponent<PlayerMulti>().currentHP = 99999999;
                    }


                    if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisMission == 5 && !bossSpawned)
                    {
                        bossSpawned = true;

                        GetComponent<AudioSource>().clip = UImusic[0];
                        GetComponent<AudioSource>().Play();

                        StartCoroutine("TriggerBossBattle");
                    }

                    GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                    if (playerAlive.Length > 0 && currentMenu == -1 && !bossSpawned)
                    {
                        objectiveComplete = true;
                        StartCoroutine("showHighScore");
                    }
                }
            }
            if (gameMode == 2)
            {
                if (missionMode == 1 && kills >= maxKills)
                { 
                    int rng = Random.Range(0, 4);
                    if (rng == 0)
                    {
                        objectiveComplete = true;
                        bossSpawned = true;

                        GetComponent<AudioSource>().clip = UImusic[0];
                        GetComponent<AudioSource>().Play();

                        StartCoroutine("TriggerBossBattle");
                    }
                    else
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

        endlessModeRNGBoss = 666; // for endlessMode

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



            if (gameMode == 1)
            {
                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1)
                    spawnManager.GetComponent<EnemyManager>().spawnBoss01();

                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 2)
                    spawnManager.GetComponent<EnemyManager>().spawnBoss02();
            }
            else
            {
                endlessModeRNGBoss = Random.Range(0, 2);

                if (endlessModeRNGBoss == 0)
                    spawnManager.GetComponent<EnemyManager>().spawnBoss01();

                if (endlessModeRNGBoss == 1)
                    spawnManager.GetComponent<EnemyManager>().spawnBoss02();
            }

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
            if (bossSpawned && currentMenu == -1)
            {
                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1 || endlessModeRNGBoss == 0)//BOSS01
                {
                    GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < playerAlive.Length; i++)
                    {
                        playerAlive[i].GetComponent<PlayerMulti>().wrap = true;
                    }

                    backGround.GetComponentInChildren<WaterAnimation>().moveOffset = false;
                }
                //BOSS02 doesnt need extra settings                
            }

            while (bossSpawned)
            {
                yield return new WaitForSeconds(10f);
                if(currentMenu == -1)
                Instantiate(powerUp, powerUp.transform.position = new Vector2(Random.Range(min.x + 1, max.x - 1), 6f), powerUp.transform.rotation);
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
            if (gameMode == 2)
                endlessProgress++;
            if (gameMode == 1)
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
                }
                while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);

                Dpad();

                Save();
            }

            if (!bossSpawned)
            {
                GetComponent<AudioSource>().clip = UImusic[5];
                GetComponent<AudioSource>().Play();
            }
            GetComponent<SpriteRenderer>().sprite = victorySprite;
        }
        else
        {
            GetComponent<AudioSource>().clip = UImusic[6];
            GetComponent<AudioSource>().Play();

            GetComponent<SpriteRenderer>().sprite = loseSprite;
        }

        bossHealthBar.SetActive(false);
        menuActive = true;

        currentMenu = 4; //4 = Highscore

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

        float screenWidth = Mathf.Abs(min.x) + max.x;

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

        cross.transform.position = new Vector3(screenWidth / (playerCount + 1f) * highestPlayer, -2.4f, 0f);

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
        seconds = MAXSECONDS;
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
        if (currentMenu > -1)
        {
            GamepadInput.GamePad.Index[] gamePadIndex;
            gamePadIndex = new GamepadInput.GamePad.Index[4];
            gamePadIndex[0] = GamePad.Index.One;
            gamePadIndex[1] = GamePad.Index.Two;
            gamePadIndex[2] = GamePad.Index.Three;
            gamePadIndex[3] = GamePad.Index.Four;

            //CharacterSelection
            if (currentMenu == 3)
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
                        playerChoice[i]++;
                        playerChoice[i] %= Characters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playerChoice[i])];
                        UIBeeps[1].Play();
                    }
                    if ((playerDpad[i].y > 0f) && !pressedPlayerDpad[i])
                    {
                        pressedPlayerDpad[i] = true;
                        playerChoice[i]--;
                        playerChoice[i] %= Characters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playerChoice[i])];
                        UIBeeps[1].Play();
                    }
                    if (playerDpad[i].y == 0f)
                    {
                        pressedPlayerDpad[i] = false;
                    }
                }

                //Keyboard
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
                    playerChoice[0]++;
                    playerChoice[0] %= Characters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playerChoice[0])];
                }
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow)
                {
                    pressedArrow = true;
                    playerChoice[0]--;
                    playerChoice[0] %= Characters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playerChoice[0])];
                }
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    pressedArrow = false;
                }
            }


            //PlayerAny          
            Vector2 playerAnyDpad = GamePad.GetAxis(GamePad.Axis.Dpad, GamePad.Index.Any);

            //################################Navigate down the MainMenu################################
            if (currentMenu == 1 || currentMenu == 2)
            {
                if ((playerAnyDpad.y < 0f) && !pressedDpad) //&& currentMenu==1 ?
                {
                    /*
                                 
                    while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status != 1 && MissionIcons[currentMissionSelection].GetComponent<Mission>().status != 2)
            { 
            if (currentMissionSelection < MissionIcons.Length-1)
                currentMissionSelection++;
            else
                currentMissionSelection = 0;
            }

                     */

                    if (currentMenu == 1)
                        currentMainMenuSelection++;
                    if (currentMenu == 2)
                        do
                        {
                            if (currentMissionSelection < MissionIcons.Length - 1)
                                currentMissionSelection++;
                            else
                                currentMissionSelection = 0;
                            //Debug.Log("Searching: " + currentMissionSelection);
                        }
                        while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);
                    Dpad();
                }
                //Navigate up the MainMenu
                if ((playerAnyDpad.y > 0f) && !pressedDpad)
                {
                    if (currentMenu == 1)
                        currentMainMenuSelection--;
                    if (currentMenu == 2)
                        do
                        {
                            if (currentMissionSelection > 0)
                                currentMissionSelection--;
                            else
                                currentMissionSelection = MissionIcons.Length - 1;
                        }
                        while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);

                    Dpad();
                }
                if ((playerAnyDpad.y == 0f) && pressedDpad)
                {
                    pressedDpad = false;
                }

                //################################Keyboardsupport################################
                if (Input.GetKey(KeyCode.DownArrow) && !pressedArrow)
                {
                    if (currentMenu == 1)
                        currentMainMenuSelection++;
                    if (currentMenu == 2)
                        do
                        {
                            if (currentMissionSelection > 0)
                                currentMissionSelection--;
                            else
                                currentMissionSelection = MissionIcons.Length - 1;
                        }
                        while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);

                    Dpad();
                }
                //Keyboardsupport
                //Navigate up the MainMenu
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow) //&& currentMenu==1 ?
                {
                    if (currentMenu == 1)
                        currentMainMenuSelection--;
                    if (currentMenu == 2)
                        do
                        {
                            if (currentMissionSelection < MissionIcons.Length - 1)
                                currentMissionSelection++;
                            else
                                currentMissionSelection = 0;
                            //Debug.Log("Searching: " + currentMissionSelection);
                        }
                        while (MissionIcons[currentMissionSelection].GetComponent<Mission>().status < 1);
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
            if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && currentMenu == 1 && !pressedButton)
            {
                pressedButton = true;
                UIBoomSound();
                //StoryMode
                if (currentMainMenuSelection == 0)
                {
                    GotoMissionMenu();
                    gameMode = 1;
                }
                //Endless
                if (currentMainMenuSelection == 1)
                {
                    GotoSelectionScreen();
                    gameMode = 2;
                    backGround.GetComponent<BackGroundManager>().SetBackground(1);
                }
                //SurvivalMode
                if (currentMainMenuSelection == 2)
                {
                    GotoSelectionScreen();
                    gameMode = 3;
                    backGround.GetComponent<BackGroundManager>().SetBackground(1);
                }
                //Exit to desktop
                if (currentMainMenuSelection == 3)
                {
                    Application.Quit();
                }
            }

            //missionSelection
            if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && currentMenu == 2 && !pressedButton)
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
        if (currentMenu == 4 && (GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton && fadeFinished)
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
        if (((GamePad.GetButton(GamePad.Button.Start, GamePad.Index.Any) || Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) && currentMenu == 3 && menuActive && (playerActive[0] || playerActive[1] || playerActive[2] || playerActive[3])))
        {
            currentMenu = -1;
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
        if (currentMenu == 4)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }
        missionMarker.SetActive(false);
        pressedButton = true;   //Prefend accidental menu navigation
        currentMenu = 1;        //-1 = None, 0 = TitleScreen, 1 = ModeSelection, 2 = MissionSelection, 3 = CharacterSelection, 4 = Highscore
        currentMainMenuSelection = 0; //Default is 1 -> Misson

        mainMenuScreen.SetActive(true); //Activate GUI Text
        startScreen.SetActive(false);   //Deactivate old GUI
        characterScreen.SetActive(false);
        highScoreScreen.SetActive(false);
        missionScreen.SetActive(false);

        GetComponent<SpriteRenderer>().sprite = menuSprite;

        UIBoomSound();

        //Reset menu colorization
        for (int i = 0; i < mainMenuGuiText.Length; i++)
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

        if (currentMenu == 4 && !sieg)
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
        currentMenu = 2;



        //setMissionMarker(currentMissionSelection); //Last unlocked mission shall be the default chosen
        Dpad();

        GetComponent<SpriteRenderer>().sprite = worldMap;
    }

    //Navigating to the selection menu
    void GotoSelectionScreen()
    {
        if (currentMenu == 4)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }
        highScoreScreen.SetActive(false);
        missionMarker.SetActive(false);
        mainMenuScreen.SetActive(false);
        missionScreen.SetActive(false);

        characterScreen.SetActive(true);

        currentMenu = 3;

        GetComponent<SpriteRenderer>().sprite = menuSprite;
    }

}