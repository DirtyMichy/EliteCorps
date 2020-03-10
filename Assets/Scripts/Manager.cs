using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{


    public static Manager current;          //A public static reference to itself (make's it visible to other objects without a reference)

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Player ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public int playerCount = 0;
    public int[] playersChosenCharacter;    //Chosen Character by Player 0 = Character01
    public int[] playerScore;               //The player's score 0 = Player1, ...
    public bool[] playerActive;             //0 = Player1, ...
    public GameObject[] PlayerChosenChar;


    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Objective ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public bool objectiveComplete = true;

    private int maxObjectiveKills = 2;
    private int MAXSECONDS = 128;
    private int maxKills = 64;
    private int startedCampaign = 0;        //0 means no saveFile exists   


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


    public int currentMainMenuSelection = 0;        //Used for iteration with arrow keys oder dpad inside the main menu
    public int currentMissionSelection = 0;         //Used for iteration with arrow keys oder dpad inside the mission menu

    public int kills = 0;
    public int secondsLeft = 300;
    public int objectiveKills = 0;
    
    public int[] missionProgress; //[0] = E1M1, 0 = Disabled, 1 = Enabled, 2 = Won       


    public GameObject[] MissionIcons;
    public GameObject[] PlayableCharacters;
    public GameObject[] missionObject;
    public GameObject[] player;

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Canvas ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public GameObject characterScreen;
    public GameObject startScreen;
    public GameObject mainMenuScreen;
    public GameObject highScoreScreen;
    public GameObject ingameScoreScreen;
    public GameObject missionScreen;
    public GameObject[] CanvasScreens;
    public GameObject explosion;
    public GameObject spawnManager;
    public GameObject objective;
    public GameObject Cloud;
    public GameObject isle;
    public GameObject fader;
    //Black Sprite
    public GameObject backGround;
    //Background (Water,...)
    public GameObject bossHealthBar;
    //the menu uses other effects than ingame
    public GameObject escortPlane;
    public GameObject escortShip;
    public GameObject missionMarker;
    public GameObject powerUp;

    public RectTransform bossHealth;

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ UI Text Initialization ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public List<Text> ingameScoreGUIText;
    public List<Text> highScoreGUIText;
    public List<Text> playerActiveText;
    public List<Text> mainMenuGuiText;
    public Text objectiveText;

    //ingame status in upper right corner
    public Text missionText;        //put this into a seperate script???
    public Text QuestText;      //put this into a seperate script???
    public Text chosenEpisode; //put this into a seperate script???
       
    public bool pressedDpad = false;

    public bool pressedButton = false;    //prevend fast menu scrolling

    private bool[] pressedPlayerDpad;    //prevend fast menu selection
    private bool fadeFinished = false;

    //If the player plays the campaign he shall return to the missionMenu after the Highscore
    private bool bossSpawned = false;
    private bool pressedArrow = false;

    private int fadeDirection = -1;  //-1 fadeIn (transparent), 1 fadeOut (darken)



    private Vector2[] playerDpad;

    public string missionName = "";    //Current Mission like E1M1

    public string[] mainMenuText;    //0 = StoryMode, 1 = Survival, 2 = Exit


    public Sprite[] CharPreviews;

    public Sprite logoSprite, menuSprite, victorySprite, loseSprite, worldMap, MissionSpriteA, MissionSpriteB, MissionSpriteC;


    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Audio ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public AudioClip[] UImusic;
    public AudioClip[] BattleMusic;
    public AudioSource[] UIBeeps;
    public AudioClip[] DeathSounds;

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
            //#################################################################################SAVEGAME#################################################################################
            //missionProgress = PlayerPrefsX.GetIntArray("missionProgress");
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
        objectiveComplete = false;

        fadeDirection = -1;
        StartCoroutine("Fade");

        currentMenu = activeMenu.None;

        //Deactivate the title and activate the player
        ingameScoreScreen.SetActive(true);
        mainMenuScreen.SetActive(false);
        characterScreen.SetActive(false);

        //0 = None, 1 = Mission, 2 = Survive
        if (gameMode == selectedGameMode.campaign)
        {
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
                player[i] = (GameObject)Instantiate(PlayableCharacters[Mathf.Abs(playersChosenCharacter[i])], PlayableCharacters[Mathf.Abs(playersChosenCharacter[i])].transform.position = new Vector2(1f, 0f), PlayableCharacters[Mathf.Abs(playersChosenCharacter[i])].transform.rotation);
                player[i].SendMessage("SetPlayer", (i + 1));
                player[i].SendMessage("SetPlaneValue", (playerCount));
                player[i].GetComponent<PlayerMulti>().playerUIUpdate();
            }
        }

        objectiveComplete = false;              //Must be after the players are spawned
        
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

    void CheckWinCondition()
    {
        if (!objectiveComplete)
        {
                //switch (missionMode)
                //{
                //    case missionObjectives.none:
                //        break;
                //    case missionObjectives.killAll:
                //        break;
                //    case missionObjectives.reachAndSurvive:
                //        break;
                //    case missionObjectives.killObjective:
                //        break;
                //    case missionObjectives.escortAndDefend:
                //        break;
                //    default:
                //        break;
                //}

                GameObject[] escortPlane = GameObject.FindGameObjectsWithTag("Escort");

                if ((missionMode == missionObjectives.killAll && kills >= maxKills)
                                || (missionMode == missionObjectives.reachAndSurvive && secondsLeft <= 0)
                                || (missionMode == missionObjectives.killObjective && objectiveKills >= maxObjectiveKills)
                                || (missionMode == missionObjectives.escortAndDefend && secondsLeft <= 0 && escortPlane.Length >= 1))
                {
                    for (int i = 0; i < escortPlane.Length; i++)
                    {
                        //escortPlane[i].GetComponent<Rigidbody2D>().velocity = (transform.up) * 4f;
                        escortPlane[i].GetComponent<PlayerMulti>().currentHP = 666;
                    }


                    if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisMission == 5 && !bossSpawned)
                    {
                        bossSpawned = true;
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

    int CountPlayersAlive()
    {
        GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
        return playerAlive.Length;
    }

    IEnumerator TriggerBossBattle()
    {
        Debug.Log("TriggerBossBattle");

        objectiveText.text = "BOSS";

        GetComponent<AudioSource>().clip = UImusic[0];
        GetComponent<AudioSource>().Play();

        objectiveComplete = false;                                          //Otherwise the winscreeen would load directly

        spawnManager.GetComponent<EnemyManager>().StopSpawnCoroutines();    //Stop spawning props and units

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

        // EXTRA: Controls change during BOSS01 after 10 seconds
        if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1)
        {
            yield return new WaitForSeconds(10f);

            //the player could still die during the 10 second delay, so we have to check again if they are alive
            if (CountPlayersAlive() > 0 && bossSpawned && currentMenu == activeMenu.None)
            {
                if (MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode == 1)
                {
                    GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < playerAlive.Length; i++)
                    {
                        playerAlive[i].GetComponent<PlayerMulti>().wrap = true;
                    }
                }
            }
        }
    }

    public void ShowAndHideCanvas()
    {
        for (int i = 0; i < CanvasScreens.Length; i++)
        {
            CanvasScreens[i].SetActive(false);
        }

        switch (currentMenu)
        {
            case activeMenu.None:
                break;
            case activeMenu.TitleScreen:
                CanvasScreens[0].SetActive(true);
                break;
            case activeMenu.ModeSelection:
                CanvasScreens[1].SetActive(true);
                break;
            case activeMenu.MissionSelection:
                CanvasScreens[2].SetActive(true);
                break;
            case activeMenu.CharSelection:
                CanvasScreens[3].SetActive(true);
                break;
            case activeMenu.Highscore:
                CanvasScreens[4].SetActive(true);
                break;
            default:
                break;
        }
    }


    public void ShowHighScore()
    {
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

        currentMenu = activeMenu.Highscore;

        spawnManager.GetComponent<EnemyManager>().StopSpawnCoroutines();//Stop spawning props and units

        StopCoroutine("CountDown");

        //Remove all gamePlayAssets from Scene
        CleanScene("Player");
        CleanScene("Enemy");
        CleanScene("Props");
        CleanScene("Island");
        CleanScene("Escort");
        CleanScene("Bullet");
        CleanScene("Level");
        CleanScene("PowerUp");
        
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

        ShowAndHideCanvas();

        fadeDirection = -1;
        StartCoroutine("Fade");
    }

    public void CleanScene(string objectsToFind)
    {
        GameObject[] removals = GameObject.FindGameObjectsWithTag(objectsToFind);

        for (int i = 0; i < removals.Length; i++)
        {
            Destroy(removals[i]);
        }
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
        
        kills = 0;
        objectiveKills = 0;
        secondsLeft = MAXSECONDS;
        bossSpawned = false;
    }
    
    public void Save()
    {
        for (int i = 0; i < MissionIcons.Length; i++)
        {
            missionProgress[i] = MissionIcons[i].GetComponent<Mission>().status;
        }
        //#################################################################################SAVEGAME#################################################################################
        //Save the progress to the player prefs
        //PlayerPrefsX.SetIntArray("missionProgress", missionProgress);


        if (startedCampaign == 0)
            PlayerPrefs.SetInt("startedCampaign", (startedCampaign + 1));

        PlayerPrefs.Save();
    }

    //Everything for the menu navigation
    void MenuNavigation()
    {
        //activeMenu.None = playing
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
                        playersChosenCharacter[i] %= PlayableCharacters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playersChosenCharacter[i])];
                        UIBeeps[1].Play();
                    }
                    if ((playerDpad[i].y > 0f) && !pressedPlayerDpad[i])
                    {
                        pressedPlayerDpad[i] = true;
                        playersChosenCharacter[i]--;
                        playersChosenCharacter[i] %= PlayableCharacters.Length;
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
                    playersChosenCharacter[0] %= PlayableCharacters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(playersChosenCharacter[0])];
                }
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow)
                {
                    pressedArrow = true;
                    playersChosenCharacter[0]--;
                    playersChosenCharacter[0] %= PlayableCharacters.Length;
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

            //Menuselection
            if(currentMenu == activeMenu.ModeSelection)
            if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton)
            {
                pressedButton = true;
                UIBoomSound();
                //StoryMode
                if (currentMainMenuSelection == 0)
                {
                    GotoMissionSelection();
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
            if (currentMenu == activeMenu.MissionSelection)
                if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton)
            {
                pressedButton = true;
                UIBoomSound();

                backGround.GetComponent<BackGroundManager>().SetBackground(MissionIcons[currentMissionSelection].GetComponent<Mission>().thisEpisode); //Episodes start with 1,2,...

                GotoSelectionScreen();
            }

            //Reallow button controls
            if (GamePad.GetButtonUp(GamePad.Button.A, GamePad.Index.Any) || GamePad.GetButtonUp(GamePad.Button.Y, GamePad.Index.Any) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Y))
            {
                //Timed invoke?
                pressedButton = false;
            }
        }

        //Continue Mission or try again
        if(currentMenu == activeMenu.Highscore)
        if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton && fadeFinished)
        {
            pressedButton = true;

            if (gameMode == selectedGameMode.campaign)
                GotoMissionSelection();
            else
                GotoSelectionScreen();
        }

        //Get into the MainMenu
        if(currentMenu == activeMenu.TitleScreen)
        if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.A)) && !pressedButton)
        {
            GotoMainMenu();
        }

        //Start the game
        if(currentMenu == activeMenu.CharSelection)
        if (((GamePad.GetButton(GamePad.Button.Start, GamePad.Index.Any) || 
                Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) &&
                (playerActive[0] || playerActive[1] || playerActive[2] || playerActive[3])))
        {
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
        UIBoomSound();

        if (currentMenu == activeMenu.Highscore)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }
        missionMarker.SetActive(false);
        pressedButton = true;                               //Prevend accidental menu navigation
        currentMenu = activeMenu.ModeSelection;
        currentMainMenuSelection = 0;                       //Default is 1 -> Misson

        mainMenuScreen.SetActive(true);                     //Activate GUI Text
        startScreen.SetActive(false);                       //Deactivate old GUI
        characterScreen.SetActive(false);
        highScoreScreen.SetActive(false);
        missionScreen.SetActive(false);
        
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
    void GotoMissionSelection()
    {
        UIBoomSound();

        currentMenu = activeMenu.MissionSelection;

        GetComponent<AudioSource>().clip = UImusic[4];
        GetComponent<AudioSource>().Play();
        
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

        Dpad();
    }

    //Navigating to the selection menu
    void GotoSelectionScreen()
    {
        UIBoomSound();

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