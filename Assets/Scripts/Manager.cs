using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;
using System.Collections.Generic;
using System.IO;

public class Manager : MonoBehaviour
{
    public static Manager current;                  //A public static reference to itself (make's it visible to other objects without a reference)

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Player ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public int playerCount = 0;
    public int[] chosenCharacterIndex;            //Chosen Character by Player 0 = Character01
    public int[] playerScore;                       //The player's score 0 = Player1, ...
    public bool[] playerActive;                     //0 = Player1, ...
    public GameObject[] PlayerChosenChar;
    private bool pressedDpad = false;
    private bool pressedArrow = false;
    private bool pressedButton = false;
    private bool[] pressedPlayerDpad;
    private Vector2[] playerDpad;
    private GamepadInput.GamePad.Index[] gamePadIndex = { GamePad.Index.One, GamePad.Index.Two, GamePad.Index.Three, GamePad.Index.Four };

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Objective ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    private bool bossSpawned = false;
    public bool objectiveComplete = true;
    public int requiredObjectivesToKill = 2;
    private int MAXSECONDS = 128;
    public int MAXPLAYERS = 4;
    public int requiredKillsToWin = 64;
    public int currentMainMenuSelection = 0;        //Used for iteration with arrow keys oder dpad inside the main menu
    public int currentMissionSelected = 0;          //Used for iteration with arrow keys oder dpad inside the mission menu
    public int kills = 0;
    public int secondsLeft = 300;
    public int objectiveKills = 0;
    public List<GameObject> Missions;
    public GameObject[] PlayableCharacters;
    public GameObject[] missionObject;
    public GameObject[] player;

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

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Canvas ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public GameObject[] CanvasScreens;
    private int fadeDirection = -1;             //-1 fadeIn (transparent), 1 fadeOut (darken)
    private bool fadeFinished = true;
    public GameObject fader;
    public GameObject missionMarker;
    public GameObject backGround;
    public GameObject gameOver;

    public List<Text> highScoreGUIText;
    public List<Text> mainMenuGuiText;

    public Text objectiveText;
    public Text missionText;                    //put this into a seperate script???
    public Text ObjectiveIntroText;             //put this into a seperate script???
    public Text chosenEpisode;                  //put this into a seperate script???

    public string missionName = "";
    private string[] mainMenuText = { "✠ Überleben ✠", "✠ Mission ✠", "✠ Beenden ✠" };
    public Sprite[] CharPreviews;
    public Sprite logoSprite, menuSprite, victorySprite, loseSprite, worldMap;
    public Sprite[] MissionSprites;

    [Header("✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠ Gameplay ✠ - ✠ - ✠ - ✠ - ✠ - ✠ - ✠")]

    public GameObject explosion;
    public GameObject spawnManager;
    public GameObject Background;
    public GameObject escortPlane;
    public GameObject escortShip;
    public GameObject powerUp;
    public Material[] BackgroundMaterials;

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
        StartCoroutine(ShowHighScore());
    }

    public void PlayDeathSound()
    {
        UIBeeps[3].clip = DeathSounds[Random.Range(0, DeathSounds.Length)];
        UIBeeps[3].Play();
    }

    void Awake()
    {
        fader.SetActive(true);

        if (current == null)
            current = this;
        else
            Destroy(gameObject);

        for (int i = 0; i < CanvasScreens[2].transform.GetChild(0).childCount; i++)
        {
            Missions.Add(CanvasScreens[2].transform.GetChild(0).GetChild(i).gameObject);
        }

        if (!File.Exists(Application.dataPath + "/save.corpse"))
        {
            SaveFile.current = new SaveFile();
            SaveLoad.Save();
        }
        else
        {
            Debug.Log("Loading File");
            SaveLoad.Load();
            SaveFile.current = SaveLoad.savedGames[0];

            //0 locked, 1 unlocked, 2 won
            for (int i = 0; i < Missions.Count; i++)
            {
                Missions[i].GetComponent<Mission>().status = SaveFile.current.campaignMissionStatus[i];
                Missions[i].GetComponent<SpriteRenderer>().sprite = MissionSprites[Missions[i].GetComponent<Mission>().status];
            }
        }

        //-1 fadeIn (transparent), 1 fadeOut (darken)
        fadeDirection = -1;
        StartCoroutine("Fade");

        UIBeeps = GetComponents<AudioSource>();

        StartCoroutine("VolumeOn");

        //Avoiding index out of bounds
        playerActive = new bool[MAXPLAYERS];
        playerScore = new int[MAXPLAYERS];
        player = new GameObject[MAXPLAYERS];
        chosenCharacterIndex = new int[MAXPLAYERS];
        pressedPlayerDpad = new bool[MAXPLAYERS];
        playerDpad = new Vector2[MAXPLAYERS];

        for (int i = 0; i < chosenCharacterIndex.Length; i++)
        {
            chosenCharacterIndex[i] = 0;
            pressedPlayerDpad[i] = false;
            playerActive[i] = false;
            playerDpad[i] = new Vector2(0, 0);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.I))
            InstaWin();

        if (currentMenu == activeMenu.None)
        {
            //Count players alive
            if (CountPlayersAlive() == 0 && (!objectiveComplete || bossSpawned))
            {
                Debug.Log("GameOver");
                currentMenu = activeMenu.Highscore;
                StartCoroutine(ShowHighScore());
            }

            if (missionMode == missionObjectives.escortAndDefend)
            {
                GameObject[] escortPlaneCount = GameObject.FindGameObjectsWithTag("Escort");
                if (escortPlaneCount.Length == 0 && !objectiveComplete && !bossSpawned && currentMenu != activeMenu.Highscore)
                {
                    Debug.Log("GameOver");
                    currentMenu = activeMenu.Highscore;
                    StartCoroutine(ShowHighScore());
                }
            }

            UpdateIngameUI();
        }
        else
            MenuNavigation();

        //Back to MainMenu
        if (currentMenu != activeMenu.None && (GamePad.GetButton(GamePad.Button.Y, GamePad.Index.Any) || Input.GetKey(KeyCode.Y)) && !pressedButton && fadeFinished)
        {
            pressedButton = true;
            GotoMainMenu();
        }
    }

    void UpdateIngameUI()
    {
        //Update objective UI
        if (gameMode == selectedGameMode.campaign)
        {
            if (missionMode == missionObjectives.killAll)
                objectiveText.text = kills + "/" + requiredKillsToWin;
            if (missionMode == missionObjectives.escortAndDefend)
                objectiveText.text = "" + secondsLeft;
            if (missionMode == missionObjectives.killObjective)
                objectiveText.text = objectiveKills + "/" + requiredObjectivesToKill;
            if (missionMode == missionObjectives.reachAndSurvive)
                objectiveText.text = "" + (secondsLeft * 10) + "KM";
        }
    }

    void UINavigationAudio()
    {
        UIBeeps[2].Play();
    }

    void UISelectionAudio()
    {
        Instantiate(explosion, explosion.transform.position = new Vector2(-32f, -32f), explosion.transform.rotation);
    }

    void Dpad()
    {
        pressedArrow = true;
        pressedDpad = true;

        UINavigationAudio();

        if (currentMenu == activeMenu.ModeSelection)
        {
            currentMainMenuSelection %= mainMenuText.Length;
            if (currentMainMenuSelection < 0)
            {
                currentMainMenuSelection = mainMenuText.Length - 1;
            }

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
            setMissionMarker();
        }
    }

    void GameStart()
    {
        objectiveComplete = false;

        fadeDirection = -1;
        StartCoroutine("Fade");

        //Spawn the playerPlanes
        for (int i = 0; i < playerCount; i++)
        {
            if (playerActive[i])
            {
                player[i] = (GameObject)Instantiate(PlayableCharacters[Mathf.Abs(chosenCharacterIndex[i])], new Vector2((1 - playerCount) + (i * 2), -4f), PlayableCharacters[Mathf.Abs(chosenCharacterIndex[i])].transform.rotation);
                player[i].SendMessage("SetPlayer", (i + 1));
                player[i].SendMessage("SetPlaneValue", (playerCount));
            }
        }

        objectiveComplete = false;

        //activeMenu.None check will be used to check inside of Update() if all players are alive, so it needs to be set AFTER the players are spawned
        currentMenu = activeMenu.None;
        ShowAndHideCanvas();
        if (gameMode == selectedGameMode.campaign)
        {
            Background.GetComponent<MeshRenderer>().material = BackgroundMaterials[currentMissionSelected / 5];

            //Spawn level if assigned
            if (missionObject[currentMissionSelected] != null)
                Instantiate(missionObject[currentMissionSelected], missionObject[currentMissionSelected].transform.position, missionObject[currentMissionSelected].transform.rotation);

            if (currentMissionSelected == 0)
            {
                requiredObjectivesToKill = 4;
                missionMode = missionObjectives.killObjective;
                ObjectiveIntroText.text = "Zerstöre alle Bunker";
            }
            if (currentMissionSelected == 1)
            {
                missionMode = missionObjectives.escortAndDefend;
                ObjectiveIntroText.text = "Eskortiere das Schiff";

                StartCoroutine("CountDown");
                Instantiate(escortShip, escortShip.transform.position = new Vector2(0f, -3f), escortShip.transform.rotation);
            }
            if (currentMissionSelected == 2)
            {
                requiredObjectivesToKill = 1;
                missionMode = missionObjectives.killObjective;
                ObjectiveIntroText.text = "Zerstöre die Bohrinsel";
            }
            if (currentMissionSelected == 3)
            {
                missionMode = missionObjectives.killAll;
                ObjectiveIntroText.text = "Zerstöre genug Schiffe";
            }
            if (currentMissionSelected == 4)
            {
                missionMode = missionObjectives.reachAndSurvive;
                ObjectiveIntroText.text = "Erreiche das Ziel";
                StartCoroutine("CountDown");
            }
            if (currentMissionSelected == 5)
            {
                missionMode = missionObjectives.escortAndDefend;
                ObjectiveIntroText.text = "Eskortiere die Flugzeuge";

                StartCoroutine("CountDown");
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(-3f, -3f), escortPlane.transform.rotation);
                Instantiate(escortPlane, escortPlane.transform.position = new Vector2(3f, -3f), escortPlane.transform.rotation);
            }
            if (currentMissionSelected == 6)
            {
                missionMode = missionObjectives.reachAndSurvive;
                ObjectiveIntroText.text = "Überlebe bis zum Ende";
                StartCoroutine("CountDown");
            }
            if (currentMissionSelected == 7)
            {
                missionMode = missionObjectives.killAll;
                ObjectiveIntroText.text = "Zerstöre die Gegner";
            }
            if (currentMissionSelected == 8)
            {
                requiredObjectivesToKill = 3;
                missionMode = missionObjectives.killObjective;
                ObjectiveIntroText.text = "Zerstöre alle Transporter";
            }
            if (currentMissionSelected == 9)
            {
                missionMode = missionObjectives.reachAndSurvive;
                ObjectiveIntroText.text = "Erreiche das Ziel";
                StartCoroutine("CountDown");
            }

            UpdateIngameUI();

            StartCoroutine("ShowQuest");
        }

        if (gameMode == selectedGameMode.survive)
        {
            Background.GetComponent<MeshRenderer>().material = BackgroundMaterials[0];
            missionText.text = "";
            objectiveText.text = "";
            missionMode = missionObjectives.none;
        }

        GetComponent<AudioSource>().clip = BattleMusic[Random.Range(1, BattleMusic.Length)];
        GetComponent<AudioSource>().Play();
        StartCoroutine("VolumeOn");

        spawnManager.GetComponent<EnemyManager>().StartSpawnCoroutines();
    }

    //Show the missionObjective at start
    IEnumerator ShowQuest()
    {
        ObjectiveIntroText.fontSize = 100;
        yield return new WaitForSeconds(2f);
        for (int i = 100; i > 0; i--)
        {
            ObjectiveIntroText.fontSize = i;
            yield return new WaitForSeconds(0.001f);
        }
        ObjectiveIntroText.text = "";
        StopCoroutine("ShowQuest");
    }

    //Fade in and out
    IEnumerator Fade()
    {
        if (fadeFinished)
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
            if (
                               (missionMode == missionObjectives.killAll && kills >= requiredKillsToWin)
                            || (missionMode == missionObjectives.reachAndSurvive && secondsLeft <= 0)
                            || (missionMode == missionObjectives.killObjective && objectiveKills >= requiredObjectivesToKill)
                            || (missionMode == missionObjectives.escortAndDefend && secondsLeft <= 0)
               )
            {
                GameObject[] escortPlane = GameObject.FindGameObjectsWithTag("Escort");
                for (int i = 0; i < escortPlane.Length; i++)
                {
                    escortPlane[i].GetComponent<Player>().isInvincible = true;
                }

                if (Missions[currentMissionSelected].GetComponent<Mission>().thisMission == 5 && !bossSpawned)
                {
                    bossSpawned = true;
                    StartCoroutine("TriggerBossBattle");
                }

                GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                if (playerAlive.Length > 0 && currentMenu == activeMenu.None && !bossSpawned)
                {
                    objectiveComplete = true;
                    StartCoroutine(ShowHighScore());
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

            CanvasScreens[5].SetActive(true);

            GetComponent<AudioSource>().clip = BattleMusic[0];
            GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().volume = 100f;

            spawnManager.GetComponent<EnemyManager>().spawnBoss(Missions[currentMissionSelected].GetComponent<Mission>().thisEpisode);
        }

        // EXTRA: Controls change during BOSS01 after 10 seconds
        if (Missions[currentMissionSelected].GetComponent<Mission>().thisEpisode == 1)
        {
            yield return new WaitForSeconds(10f);

            //the player could still die during the 10 second delay, so we have to check again if they are alive
            if (CountPlayersAlive() > 0 && bossSpawned && currentMenu == activeMenu.None)
            {
                if (Missions[currentMissionSelected].GetComponent<Mission>().thisEpisode == 1)
                {
                    GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < playerAlive.Length; i++)
                    {
                        playerAlive[i].GetComponent<Player>().wrap = true;
                    }
                }
            }
        }
    }

    public void ShowAndHideCanvas()
    {
        //0 Title, 1 Mission
        for (int i = 0; i < CanvasScreens.Length; i++)
        {
            CanvasScreens[i].SetActive(false);
        }

        switch (currentMenu)
        {
            case activeMenu.None:
                CanvasScreens[5].SetActive(true);
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

    public IEnumerator ShowHighScore()
    {
        if(gameMode == selectedGameMode.survive)
            objectiveComplete = true;

        gameOver.SetActive(!objectiveComplete);

        fadeDirection = 1;
        StartCoroutine("Fade");

        yield return new WaitForSeconds(1);

        currentMenu = activeMenu.Highscore;
        ShowAndHideCanvas();

        if (objectiveComplete)
        {
            //Update missionProgress
            if (gameMode == selectedGameMode.campaign)
            {
                Missions[currentMissionSelected].GetComponent<Mission>().status = 2;

                //Save progress to saveFile
                SaveFile.current.campaignMissionStatus[currentMissionSelected] = 2;

                //unlock next misson
                if (currentMissionSelected < Missions.Count - 1)
                    if (Missions[currentMissionSelected + 1].GetComponent<Mission>().status == 0)
                    {
                        Missions[currentMissionSelected + 1].GetComponent<Mission>().status = 1;
                        SaveFile.current.campaignMissionStatus[currentMissionSelected + 1] = 1;
                    }

                //Autoselect next Mission
                do
                {
                    if (currentMissionSelected < Missions.Count - 1)
                        currentMissionSelected++;
                    else
                        currentMissionSelected = 0;
                }
                while (Missions[currentMissionSelected].GetComponent<Mission>().status < 1);

                Dpad();

                Save();

                for (int i = 0; i < Missions.Count; i++)
                {
                    Missions[i].GetComponent<SpriteRenderer>().sprite = MissionSprites[Missions[i].GetComponent<Mission>().status];
                }
            }

            GetComponent<AudioSource>().clip = UImusic[5];
            GetComponent<AudioSource>().Play();
        }
        else
        {
            GetComponent<AudioSource>().clip = UImusic[6];
            GetComponent<AudioSource>().Play();
        }

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

        float pos = 256, posY = 0.64f;
        
        for (int j = 0; j < playerCount; j++)
        {
            //highScoreGUIText[j].transform.localPosition = new Vector3((0 - pos * (playerCount - 1)) + j * (pos * 2), posY, 0f);
        }

        for (int i = 0; i < 4; i++)
        {
            if (playerActive[i])
                highScoreGUIText[i].text = " Spieler " + (i + 1) + " \n Punkte: \n" + playerScore[i];
            else
                highScoreGUIText[i].text = "";

            highScoreGUIText[highestPlayer].transform.GetChild(0).gameObject.SetActive(false);

            if (playerScore[i] > highestScore)
            {
                highestScore = playerScore[i];
                highestPlayer = i;
            }
            highScoreGUIText[i].transform.position = new Vector3((0 - pos * (playerCount - 1)) + i * (pos * 2), posY, 0f);
        }

        highScoreGUIText[highestPlayer].transform.GetChild(0).gameObject.SetActive(true);

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
        SaveLoad.Save();
    }

    //Everything for the menu navigation
    void MenuNavigation()
    {
        //activeMenu.None means we are already playing a mission
        if (currentMenu != activeMenu.None)
        {
            //PlayerAny          
            Vector2 playerAnyDpad = GamePad.GetAxis(GamePad.Axis.Dpad, GamePad.Index.Any);

            //Get into the MainMenu
            if (currentMenu == activeMenu.TitleScreen)
                if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.Y) || Input.GetKey(KeyCode.A)) && !pressedButton)
                {
                    pressedButton = true;                               //Prevend accidental menu navigation
                    GotoMainMenu();
                }

            //Menuselection
            if (currentMenu == activeMenu.ModeSelection)
                if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton)
                {
                    pressedButton = true;
                    UISelectionAudio();
                    //StoryMode
                    if (currentMainMenuSelection == 0)
                    {
                        GotoMissionSelection();
                        gameMode = selectedGameMode.campaign;
                    }
                    //SurvivalMode
                    if (currentMainMenuSelection == 1)
                    {
                        GoToCharSelection();
                        gameMode = selectedGameMode.survive;
                        Background.GetComponent<MeshRenderer>().material = BackgroundMaterials[0];
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

                    GoToCharSelection();
                }

            //CharacterSelection
            if (currentMenu == activeMenu.CharSelection)
            {
                GameObject[] charPreviews = GameObject.FindGameObjectsWithTag("CharPreviewer");

                int pos = 256, posY = 32;

                for (int j = 0; j < charPreviews.Length; j++)
                {
                    charPreviews[j].transform.localPosition = new Vector3((0 - pos * (charPreviews.Length - 1)) + j * (pos * 2), posY, 0f);
                }

                for (int i = 0; i < MAXPLAYERS; i++)
                {
                    playerDpad[i] = GamePad.GetAxis(GamePad.Axis.Dpad, gamePadIndex[i]);

                    if (((GamePad.GetButton(GamePad.Button.A, gamePadIndex[i])) && !playerActive[i]) || ((Input.GetKey(KeyCode.A)) && !playerActive[0]))
                    {
                        playerCount++;
                        playerActive[i] = true;
                        PlayerChosenChar[i].SetActive(true);
                        PlayerChosenChar[i].GetComponentInChildren<Text>().text = "Spieler " + i + 1;
                        UINavigationAudio();
                    }
                    if ((GamePad.GetButton(GamePad.Button.B, gamePadIndex[i])) && playerActive[i])
                    {
                        playerCount--;
                        playerActive[i] = false;
                        PlayerChosenChar[i].SetActive(false);
                        UINavigationAudio();
                    }
                    if ((playerDpad[i].y < 0f) && !pressedPlayerDpad[i])
                    {
                        pressedPlayerDpad[i] = true;
                        chosenCharacterIndex[i]++;
                        chosenCharacterIndex[i] %= PlayableCharacters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(chosenCharacterIndex[i])];
                        UINavigationAudio();
                    }
                    if ((playerDpad[i].y > 0f) && !pressedPlayerDpad[i])
                    {
                        pressedPlayerDpad[i] = true;
                        chosenCharacterIndex[i]--;
                        chosenCharacterIndex[i] %= PlayableCharacters.Length;
                        PlayerChosenChar[i].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(chosenCharacterIndex[i])];
                        UINavigationAudio();
                    }
                    if (playerDpad[i].y == 0f)
                    {
                        pressedPlayerDpad[i] = false;
                    }

                    float attributeValue;
                    Debug.Log(Mathf.Abs(-1.5f));
                    
                    attributeValue = PlayableCharacters[Mathf.Abs(chosenCharacterIndex[i])].GetComponent<Player>().maxHP/10f;
                    PlayerChosenChar[i].transform.GetChild(3).transform.localScale = new Vector3(attributeValue, PlayerChosenChar[i].transform.GetChild(3).transform.localScale.y, PlayerChosenChar[i].transform.GetChild(3).transform.localScale.z);
                    
                    attributeValue = PlayableCharacters[Mathf.Abs(chosenCharacterIndex[i])].GetComponent<Player>().speed / 10f;
                    PlayerChosenChar[i].transform.GetChild(4).transform.localScale = new Vector3(attributeValue, PlayerChosenChar[i].transform.GetChild(3).transform.localScale.y, PlayerChosenChar[i].transform.GetChild(3).transform.localScale.z);

                    int amountOfCannons =0;
                    for (int l = 0; l < PlayableCharacters[Mathf.Abs(chosenCharacterIndex[i])].GetComponent<Player>().transform.childCount; l++)
                    {
                        if (PlayableCharacters[Mathf.Abs(chosenCharacterIndex[i])].GetComponent<Player>().transform.GetChild(l).tag == "Cannon")
                            amountOfCannons++;
                    }

                    attributeValue = amountOfCannons/1.5f;
                    PlayerChosenChar[i].transform.GetChild(5).transform.localScale = new Vector3(attributeValue, PlayerChosenChar[i].transform.GetChild(3).transform.localScale.y, PlayerChosenChar[i].transform.GetChild(3).transform.localScale.z);
                    
                }

                //Keyboard for Player 01
                /*
                if ((Input.GetKey(KeyCode.A)) && !playerActive[0])
                {
                    playerCount++;
                    playerActive[0] = true;
                    PlayerChosenChar[0].SetActive(true);
                    UINavigationAudio();
                }
                */
                if ((Input.GetKey(KeyCode.B)) && playerActive[0])
                {
                    playerCount--;
                    playerActive[0] = false;
                    PlayerChosenChar[0].SetActive(false);
                    UINavigationAudio();
                }
                if (Input.GetKey(KeyCode.DownArrow) && !pressedArrow)
                {
                    pressedArrow = true;
                    chosenCharacterIndex[0]++;
                    chosenCharacterIndex[0] %= PlayableCharacters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(chosenCharacterIndex[0])];
                }
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow)
                {
                    pressedArrow = true;
                    chosenCharacterIndex[0]--;
                    chosenCharacterIndex[0] %= PlayableCharacters.Length;
                    PlayerChosenChar[0].GetComponent<Image>().sprite = CharPreviews[Mathf.Abs(chosenCharacterIndex[0])];
                }
                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    pressedArrow = false;
                }
            }

            //Iterate up/down through the menu
            if (currentMenu == activeMenu.ModeSelection || currentMenu == activeMenu.MissionSelection)
            {
                if ((playerAnyDpad.y < 0f) && !pressedDpad)
                {
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection++;

                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            if (currentMissionSelected < Missions.Count - 1)
                                currentMissionSelected++;
                            else
                                currentMissionSelected = 0;
                        } while (Missions[currentMissionSelected].GetComponent<Mission>().status < 1);

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
                            if (currentMissionSelected > 0)
                                currentMissionSelected--;
                            else
                                currentMissionSelected = Missions.Count - 1;
                        } while (Missions[currentMissionSelected].GetComponent<Mission>().status < 1);

                    Dpad();
                }

                if ((playerAnyDpad.y == 0f) && pressedDpad)
                {
                    pressedDpad = false;
                }

                //################################Keyboardsupport################################
                if (Input.GetKey(KeyCode.DownArrow) && !pressedArrow)
                {
                    Debug.Log(currentMissionSelected);
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection++;

                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            Debug.Log(currentMissionSelected);
                            if (currentMissionSelected > 0)
                                currentMissionSelected--;
                            else
                                currentMissionSelected = Missions.Count - 1;
                            Debug.Log(currentMissionSelected);
                        } while (Missions[currentMissionSelected].GetComponent<Mission>().status < 1);

                    Debug.Log(currentMissionSelected);

                    Dpad();
                }

                //Navigate up the MainMenu
                if (Input.GetKey(KeyCode.UpArrow) && !pressedArrow)
                { //&& currentMenu==1 ?
                    if (currentMenu == activeMenu.ModeSelection)
                        currentMainMenuSelection--;
                    if (currentMenu == activeMenu.MissionSelection)
                        do
                        {
                            if (currentMissionSelected < Missions.Count - 1)
                                currentMissionSelected++;
                            else
                                currentMissionSelected = 0;
                            //Debug.Log("Searching: " + currentMissionSelection);
                        } while (Missions[currentMissionSelected].GetComponent<Mission>().status < 1);
                    Dpad();
                }
            }

            //Keyboardsupport
            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                pressedArrow = false;
            }

            //Reallow button controls
            if (GamePad.GetButtonUp(GamePad.Button.A, GamePad.Index.Any) || GamePad.GetButtonUp(GamePad.Button.Y, GamePad.Index.Any) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.Y))
            {
                //Timed invoke?
                pressedButton = false;
            }
        }

        //Continue Mission or try again
        if (currentMenu == activeMenu.Highscore)
            if ((GamePad.GetButton(GamePad.Button.A, GamePad.Index.Any) || Input.GetKey(KeyCode.A)) && !pressedButton && fadeFinished)
            {
                pressedButton = true;

                if (gameMode == selectedGameMode.campaign)
                    GotoMissionSelection();
                else
                    GoToCharSelection();
            }

        //Start the game
        if (currentMenu == activeMenu.CharSelection)
            if (((GamePad.GetButton(GamePad.Button.Start, GamePad.Index.Any) ||
                    Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)) &&
                    (playerActive[0] || playerActive[1] || playerActive[2] || playerActive[3])))
            {
                GameStart();
            }
    }

    //Setting the selected Mission in campaign
    void setMissionMarker()
    {
        iTween.MoveTo(missionMarker, iTween.Hash("position", Missions[currentMissionSelected].transform.position, "easeType", "linear", "time", .5f));

        chosenEpisode.text = "Episode\t" + (currentMissionSelected / 5 + 1) + "\nMission\t" + (currentMissionSelected % 5 + 1);
        missionText.text = "E" + (currentMissionSelected / 5 + 1) + "M" + ((currentMissionSelected % 5) + 1);
    }

    //Navigating to the main menu
    void GotoMainMenu()
    {
        currentMenu = activeMenu.ModeSelection;
        currentMainMenuSelection = 0;                       //Default is 1 -> Misson

        ShowAndHideCanvas();

        UISelectionAudio();

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
        UISelectionAudio();

        currentMenu = activeMenu.MissionSelection;
        ShowAndHideCanvas();

        GetComponent<AudioSource>().clip = UImusic[4];
        GetComponent<AudioSource>().Play();

        Dpad();
    }

    //Navigating to the selection menu
    void GoToCharSelection()
    {
        if (currentMenu == activeMenu.Highscore)
        {
            GetComponent<AudioSource>().clip = UImusic[2];
            GetComponent<AudioSource>().Play();
        }

        currentMenu = activeMenu.CharSelection;
        ShowAndHideCanvas();

        UISelectionAudio();
    }

}