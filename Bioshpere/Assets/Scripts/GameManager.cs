using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// declare state enumeration to drive the game
public enum EGameState { WaitingToBeginGameInitialization, SelectTeams, InitializeWorld, BeginPlay, Turn, StealOpportunity, PlaceCard}

// declare card difficulty enumeration
public enum EQuestionDifficulty { Easy, Medium, Hard }

// declare biome enumeration
public enum EBiomeType {Salt, Fresh, Estuary, Desert, Tundra, Savannah, Grassland, Temperate, Rain, Taiga }

// declare biome suit enumeration
public enum EBiomeSuit { Water, Dry, Grassy, Forest }


public class GameManager : MonoBehaviour
{
    public Card selectedCard;

    private EGameState eGameState;
    private Canvas mainCanvas;
    private GameObject canvasTitleScreen;
    private GameObject canvasMainMenu;
    private GameObject canvasGameBoard;
    private GameObject canvasTeamInfo;
    private GameObject canvasScoreboard;
    private List<GameObject> listBoardCardPosition;
    private StandingsBox standingsBox;
	AudioSource audioSource;

    private Dictionary<GameObject, string> screenDictionary = new Dictionary<GameObject, string>();

    // random used to randomize card selection to populate the gameboard




    [SerializeField]
    public GameObject gameBoard;
    public ScoreBoard scoreBoard;
    public QuestionCard questionCard;
    public GameData gameData;
	public AudioManager audioManager;

    [Header("Scoreboard")]
    public Sprite[] image;

	private void Awake()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
		if (audioSource != null)
			Debug.Log("GameManager.Awake: AudioSource Loaded...");
	}


	void Start()
    {
        InitialiazeDictionary();
        Debug.Log("GameManager.Start: Begin Function...");

        standingsBox = GameObject.FindGameObjectWithTag("StandingsBox").GetComponent<StandingsBox>();

        // when the game first starts, set initial state to BeginGame
        eGameState = EGameState.WaitingToBeginGameInitialization;

        // declare an array to hold scripts of type GameBase
        GameBase[] arrayGameBase;

        // initialize card placement gameobject list
        listBoardCardPosition = new List<GameObject>();

        //BEGIN FIND AND STORE OBJECTS OF SIGNIFICANCE



        // find and store all GameBase types starting from the object tagged MainCanvas
        arrayGameBase = GameObject.FindWithTag("MainCanvas").GetComponentsInChildren<GameBase>(true);

        // loop through to ensure we got stuff
        for (int i = 0; i < arrayGameBase.Length; ++i)
        {
            Debug.Log("GameManager.Start: Found GameBase Type: " + arrayGameBase[i].GetType() + " As Component Of: " + arrayGameBase[i].name);
            arrayGameBase[i].GameManager = this;
        }

        // END FIND OBJECTS OF SIGNIFICANCE

        // initialize selectedCard to null because we haven't selected one yet
        selectedCard = null;

        // hide all screens but title screen
        ShowOneScreen("TitleScreen");

        // initialize gamedata because some data will retain because they are serialized fields
        if (gameData == null)
        {
            Debug.Log(
                         "GameManager.Start: FAIL: attempted to initialize GameData serialized asset but " +
                         "received a null reference.  This could be because the GameData object in Resources " +
                         " folder is missing or the name was changed."
                     );
            return;
        }

        else
        {
            // set the selected number of teams to the default number of teams
            gameData.SelectedNumberOfTeams = gameData.MinNumberOfTeams;

            // echo the work
            Debug.Log(
                         "GameManager.Start: Initialized GameData.SelectedNumberOfTeams to " +
                         gameData.SelectedNumberOfTeams + ", the value of gameData.MinNumberOfTeams."
                     );

            // reset the Team array
            gameData.ArrayTeams = new GameObject[0];

            // echo the work
            Debug.Log("GameManager.Start: Initialized GameData.Team array to a new array of zero elements.");

            // reset the current team turn to zero
            gameData.CurrentTurnTeam = 0;

            // echo the work
            Debug.Log("GameManager.Start: Initialized GameData.CurrentTeamTurn to 0.");

            // reset the current answering team turn to zero
            gameData.CurrentAnsweringTeam = 0;

            // echo the work
            Debug.Log("GameManager.Start: Initialized GameData.CurrentAnsweringTeam to 0.");

            // reset the correct answering team to -1
            gameData.CorrectAnsweringTeam = -1;

            // echo the work
            Debug.Log("GameManager.Start: Initialized GameData.CorrectAnsweringTeam to -1.");


            // populate gamedata card list
            // first clear the list of any cruft from previous game run
            gameData.ListCardDeck.Clear();
            gameData.ListCardsOnTable.Clear();
            gameData.ListCardsRemovedFromGame.Clear();

            // echo the work
            Debug.Log(
                         "GameManager.Start: Cleared lists in GameData: ListCardDeck, " +
                         "ListCardsOnTable, ListCardsRemovedFromTableInitialized " +
                         "GameData.CurrentTeamTurn to 0."
                     );

            // load cards from Resources\BiomeCards folder into temp array
            Card[] tmpArray = Resources.LoadAll<Card>("BiomeCards");

            // echo the work
            Debug.Log("GameManager.Start: Loaded all cards from Resources/BiomeCards folder");


            // populate gamedata card list and set its data
            for (int i = 0; i < tmpArray.Length; ++i)
            {
                gameData.ListCardDeck.Add(tmpArray[i]);
                gameData.ListCardDeck[i].gamedata = gameData;
                gameData.ListCardDeck[i].AssignComponentData();
            }

            // echo the work
            Debug.Log("GameManager.Start: Populated and InitializedGameData ListCardDeck");

            Debug.Log("GameManager.Start: End Function...");

        }
    }

    private void Update()
    {
        if (eGameState == EGameState.BeginPlay)
        {
            Debug.Log("GameManager.Update: gameState = BeginPlay.  Calling InitializeGameWorld()..");
            InitializeGameWorld();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Initialize the dictionary with all objects that have tag "screen"
    private void InitialiazeDictionary()
    {
        // find objects with tag value Screen
        GameObject[] arrayScreenObjects = GameObject.FindGameObjectsWithTag("Screen");

        // loop through objects with tag value Screen 
        for (int i = 0; i < arrayScreenObjects.Length; ++i)
        {
            // store in Dictionary
            screenDictionary.Add(arrayScreenObjects[i], arrayScreenObjects[i].name);
            Debug.Log("GameManager.Start: Screen Name: " + arrayScreenObjects[i].name + " Was Stored.");
        }
    }

    public void ShowQuestion(Card in_SelectedCard)
    {
        selectedCard = in_SelectedCard;
        selectedCard.gamedata.questionBiome = selectedCard.biome;
        selectedCard.gamedata.questionPointValue = selectedCard.pointValue;
        gameBoard.SetActive(false);
        questionCard.SetActive(true);
    }

    public void GoToScoreboard()
    {
        questionCard.ResetCard();
        scoreBoard.SetActive(true);
        scoreBoard.SetNewCard(selectedCard);
    }

    public void GoToGameBoard()
    {
        scoreBoard.slotSelected = false;
        scoreBoard.SetActive(false);
        questionCard.SetActive(false);
        gameBoard.SetActive(true);
	}

    public void ShowTeamCountAndNamesMenuScreen()
	{
		eGameState = EGameState.SelectTeams;
        Debug.Log("GameManager.ShowTeamCountAndNamesMenu: eGameState set to: " + eGameState);
        ShowOneScreen("TeamCountAndNamesMenu");
        InitializeTeamCountAndNamesMenuScreen();
    }

    private void ShowOneScreen(string in_NameOfScreenToShow)
    {
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if (kvp.Value != in_NameOfScreenToShow)
            {
                kvp.Key.SetActive(false);
                Debug.Log("GameManager.ShowOneScreen: Screen Disabled: " + kvp.Value);
            }

            if (kvp.Value == in_NameOfScreenToShow)
            {
                kvp.Key.SetActive(true);
                Debug.Log("GameManager.ShowOneScreen: Screen Enabled: " + kvp.Value);
                Debug.Log("GameManager.ShowOneScreen: Actual Property Value: " + kvp.Key.activeInHierarchy);
            }
        }

		audioManager.PlayScreenAudio(in_NameOfScreenToShow);
		audioManager.PlayOneShot(gameData.GetSoundClip("Swoosh"));
    }

    /// <summary>
    /// This function initializes the UI panel named TeamCountAndNamesMenu tagged as Screen.  
    /// We want to initialize by hiding team 3 and team 4 text controls as well as ensure
    /// the team count is initialized to 2.
    /// </summary>
    private void InitializeTeamCountAndNamesMenuScreen()
    {
        // we initialize the main menu screen to two teams
        // hide what isn't necessary and set the team count to two

        // to make an object hidden, just add to the list of objects to be hidden
        List<string> uiObjectsToToggleActive = new List<string>();
        uiObjectsToToggleActive.Add("lblTeam3Name");
        uiObjectsToToggleActive.Add("inputfieldTeam3Name");
        uiObjectsToToggleActive.Add("lblTeam4Name");
        uiObjectsToToggleActive.Add("inputfieldTeam4Name");
        uiObjectsToToggleActive.Add("panelTeamNamesError");

        // call the func to set objects in list to inactive
        ScreenObjectsToggleActive("TeamCountAndNamesMenu", uiObjectsToToggleActive, false);

        // initialize the team count label to two
        GameObject lblTeamCount = ReturnChildObject("TeamCountAndNamesMenu", "lblTeamCount");

        if (lblTeamCount == null)
        {
            Debug.Log("GameManager.InitializeTeamCountAndNamesMenuScreen: lblTeamCount contains a null reference, the name of the control was changed");
        }

        else
        {
            lblTeamCount.GetComponent<Text>().text = gameData.defaultNumberOfTeams.ToString();
            Debug.Log(
                      "GameManager.InitializeTeamCountAndNamesMenuScreen: Set main menu team count to " +
                       gameData.defaultNumberOfTeams.ToString()
                     );
        }


    }


    /// <summary>
    /// Increment team count on Main Menu and set GameData to selected value
    /// </summary>
    public void IncrementTeamCount()
    {
        // get a handle to the text object on the Main Menu for displaying a team count
        GameObject teamCountLabel = ReturnChildObject("TeamCountAndNamesMenu", "lblTeamCount");

        if (teamCountLabel == null)
        {
            Debug.Log(
                      "FAIL: GameManager.IncrementTeamCount: Attempted to get a handle to the team count text " +
                      " object on the Main Menu.  Either the name of the object changed, or, the object's " +
                      " parent name changed, or the object's parent tag has been changed."
                     );
            return;
        }

        else
        {
            // first check if the value of gameData.SelectedNumberOfTeams to see if we have to do anything
            // if it is already the max team count then we don't have to do anything
            if (gameData.SelectedNumberOfTeams == gameData.MaxNumberOfTeams)
            {
                return;
            }

            // else we have to increment and make sure we unhide the appropriate text objects
            else
            {
                // evaluate which text objects we need to unhide
                int i = gameData.SelectedNumberOfTeams + 1;

                // create the string
                string objName = "lblTeam" + i.ToString() + "Name";

                // get a handle to the gameobject
                GameObject tmpGameObj = ReturnChildObject("TeamCountAndNamesMenu", objName);

                // set it to false
                tmpGameObj.SetActive(true);

                // do the other object
                // set the string
                objName = "inputfieldTeam" + i.ToString() + "Name";

                // get a handle to the gameobject
                tmpGameObj = ReturnChildObject("TeamCountAndNamesMenu", objName);

                // set it to false
                tmpGameObj.SetActive(true);

                // prefix increment and clamp gameData variable between its defined min and max
                gameData.SelectedNumberOfTeams =
                    Mathf.Clamp(++gameData.SelectedNumberOfTeams, gameData.MinNumberOfTeams, gameData.MaxNumberOfTeams);

                // set the label on the TeamCountAndNamesMenu appropriately
                teamCountLabel.GetComponent<Text>().text = gameData.SelectedNumberOfTeams.ToString();

                Debug.Log("GameManager.IncrementTeamCount: Incremented Team Count To: " + gameData.SelectedNumberOfTeams.ToString());
            }
        }
    }


    /// <summary>
    /// Decrement team count on Main Menu and set GameData to selected value
    /// </summary>
    public void DecrementTeamCount()
    {
        // get a handle to the text object on the Main Menu for displaying a team count
        GameObject teamCountLabel = ReturnChildObject("TeamCountAndNamesMenu", "lblTeamCount");

        if (teamCountLabel == null)
        {
            Debug.Log(
                      "FAIL: GameManager.IncrementTeamCount: Attempted to get a handle to the team count text " +
                      " object on the Main Menu.  Either the name of the object changed, or, the object's " +
                      " parent name changed, or the object's parent tag has been changed."
                     );
            return;
        }

        else
        {
            // first check if the value of gameData.SelectedNumberOfTeams to see if we have to do anything
            // if it is already the min team count then we don't have to do anything
            if (gameData.SelectedNumberOfTeams == gameData.MinNumberOfTeams)
            {
                return;
            }

            // else we have to decrement and make sure we unhide the appropriate text objects
            else
            {
                // evaluate which text objects we need to unhide
                int i = gameData.SelectedNumberOfTeams;

                // create the string
                string objName = "lblTeam" + i.ToString() + "Name";

                // get a handle to the gameobject
                GameObject tmpGameObj = ReturnChildObject("TeamCountAndNamesMenu", objName);

                // set it to false
                tmpGameObj.SetActive(false);

                // do the other object
                // set the string
                objName = "inputfieldTeam" + i.ToString() + "Name";

                // get a handle to the gameobject
                tmpGameObj = ReturnChildObject("TeamCountAndNamesMenu", objName);

                // set it to false
                tmpGameObj.SetActive(false);

                // prefix decrement and clamp gameData variable between its defined min and max
                gameData.SelectedNumberOfTeams =
                    Mathf.Clamp(--gameData.SelectedNumberOfTeams, gameData.MinNumberOfTeams, gameData.MaxNumberOfTeams);

                // set the label on the TeamCountAndNamesMenu appropriately
                teamCountLabel.GetComponent<Text>().text = gameData.SelectedNumberOfTeams.ToString();

                Debug.Log("GameManager.DecrementTeamCount: Decremented Team Count To: " + gameData.SelectedNumberOfTeams.ToString());
            }
        }
    }


    /// <summary>
    /// This function iterates through the dictionary populated in start which contains references to anything tagged <br></br>
    /// Screen. The argument in_ParentName is used as the root of the search and the first child that matches <br></br>
    /// the string in_ChildName is returned.  Null is returned when nothing is found or the in_ParentName is not in <br></br>
    /// the dictionary.
    /// </summary>
    /// <param name="in_ParentName"></param>
    /// <param name="in_ChildName"></param>
    /// <returns></returns>

    private GameObject ReturnChildObject(string in_ParentName, string in_ChildName)
    {
        Debug.Log("GameManager.ReturnChildObject: Begin Function...");

        Debug.Log(
                     "GameManager.ReturnChildObject: Arguments: in_ParentName: " +
                     in_ParentName + ", in_ChildName: " + in_ChildName + "."
                 );

        // we only want the first object we find that matches
        bool bBreakEarly = false;

        // set a readable variable
        GameObject childObject = null;
        if (screenDictionary == null)
        {
            print("Dictionary doesn't exist.");
        }
        else
        {
            foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
            {
                //Debug.Log("GameManager.ReturnChildObject: Loop: ScreenDictionary.Value: " + kvp.Value + ".");

                // if we find a hit for the parent in our dictionary
                if (kvp.Value == in_ParentName)
                {
                    Debug.Log("GameManager.ReturnChildObject: Loop: HIT!  ScreenDictionary.Value: " + kvp.Value + ".");

                    // create array of all child nodes below this parent node
                    Transform[] childTransforms = kvp.Key.GetComponentsInChildren<Transform>(true);

                    // loop through array to find a hit childname
                    foreach (Transform t in childTransforms)
                    {
                        //Debug.Log("GameManager.ReturnChildObject: InnerLoop: t.gameObject name: " + t.gameObject.name + ".");

                        // if we find a hit
                        if (t.gameObject.name == in_ChildName)
                        {
                            Debug.Log("GameManager.ReturnChildObject: InnerLoop: HIT!" + t.gameObject.name + ".");
                            // set our variable
                            childObject = t.gameObject;

                            // set the bool
                            bBreakEarly = true;

                            break;
                        }
                    }


                    // ALL THE CODE BELOW WAS ORIGINAL
                    //// then get the number of direct children this parent has
                    //int numChildren = kvp.Key.transform.childCount;
                    //for (int i = 0; i < numChildren; ++i)
                    //{
                    //    // create a temporary variable to hold the child
                    //    GameObject teamCountAndNamesMenuGameObject = kvp.Key.transform.GetChild(i).gameObject;

                    //    // if the child is what we are looking for
                    //    if (teamCountAndNamesMenuGameObject.name == in_ChildName)
                    //    {
                    //        // set the function var 
                    //        childObject = teamCountAndNamesMenuGameObject;

                    //        // set the boolean to break because we found what we came for
                    //        bBreakEarly = true;

                    //        // break out of the loop
                    //        break;
                    //    }
                    //}
                }

                // on the outerloop if we set to break because we found what we came for
                if (bBreakEarly)
                {
                    break;
                }
            }
        }

        //Debug.Log("GameManager.ReturnChildObject: Returning childObject name: " + childObject.name);
        Debug.Log("GameManager.ReturnChildObject: End Function.");
        return childObject;
    }


    /// <summary>
    /// This function sets a supplied list of gameobjects that are assumed to be under a supplied <br></br>
    /// parent name to active or inactive depending on the supplied boolean argument
    /// </summary>
    /// <param name="in_ParentName"></param>
    /// <param name="in_listChildNames"></param>
    /// <param name="in_active"></param>


    private void ScreenObjectsToggleActive(string in_ParentName, List<string> in_listChildNames, bool in_active)
    {
        // loop though all objects tagged Screen in dictionary
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            // if we find the parent name
            if (kvp.Value == in_ParentName)
            {
                // get all children
                Transform[] childTransforms = kvp.Key.GetComponentsInChildren<Transform>(true);

                // loop through children
                foreach (Transform t in childTransforms)
                {
                    // assign child to temporary variable for readability
                    GameObject gObjChild = t.gameObject;

                    // loop through the input agrument of child names to find
                    foreach (string s in in_listChildNames)
                    {
                        // if we find a match
                        if (s == gObjChild.name)
                        {
                            // do what we said we want to do
                            gObjChild.SetActive(in_active);
                        }
                    }
                }


                // ALL THIS CODE WAS COMMENTED OUT BECAUSE ITS A FAILED PIECE OF CODE THAT ASSUMES CHILDREN ARE DIRECTLY BENEATH PARENT
                //// record the number of children the parent has
                //int numChildren = kvp.Key.transform.childCount;

                //// loop through the children
                //for (int i = 0; i < numChildren; ++i)
                //{
                //    // assign child to temporary variable for readability
                //    GameObject teamCountAndNamesMenuGameObject = kvp.Key.transform.GetChild(i).gameObject;

                //    // loop through array of child names to find a match
                //    for (int j = 0; j < in_listChildNames.Count; ++j)
                //    {
                //        // if we find a match
                //        if (in_listChildNames[j] == teamCountAndNamesMenuGameObject.name)
                //        {
                //            // do what the in_active boolean argument says
                //            teamCountAndNamesMenuGameObject.SetActive(in_active);
                //        }
                //    }
                //}
            }
        }
    }


    /// <summary>
    /// Take TeamCountAndNamesMenu data and perform team initialization
    /// </summary>
    public void SubmitTeamCountAndNames()
    {
        // list to store team names temporarily
        List<string> listTeamNames = new List<string>();

        // get the count of teams and loop through to initialize GameData
        for (int i = 0; i < gameData.SelectedNumberOfTeams; ++i)
        {
            // Get the team name inputfield object per team

            // make a string for the label name because it contains a team number in the name
            string gameObjectName = "inputfieldTeam" + (i + 1).ToString() + "Name";

            // call the func to get the object we want
            GameObject gObjTeamName = ReturnChildObject("TeamCountAndNamesMenu", gameObjectName);

            // report if we received a null return
            if (gObjTeamName == null)
            {
                Debug.Log(
                          "GameManager.SubmitTeamCountAndNamesMenuThe return from ReturnChildObject was null.  " +
                          "The string we used was: " + gameObjectName + ", could be due to the text object " +
                          "being removed or renamed."
                         );
                // return because we should not continue under this condition.
                return;
            }
            else
            {
                // we need to iterate over the returned object's children because inputfields contain multiple
                // children and more are added at runtime.

                // set a temporary var for the child count
                int tmpinputFieldChildCount = gObjTeamName.transform.childCount;

                // loop through the children to find the one we want called UserInputText
                for (int j = 0; j < tmpinputFieldChildCount; ++j)
                {
                    // assign the child to temporary var
                    GameObject inputField = gObjTeamName.transform.GetChild(j).gameObject;

                    // assign a bool to ensure no two teams have the same name in lowercase
                    
                    // if we find a hit
                    if (inputField.name == "UserInputText")
                    {
                        int teamNameLength = inputField.GetComponent<Text>().text.Length;
                        string teamName = inputField.GetComponent<Text>().text;
                        Debug.Log(
                                     "GameManager.SubmitTeamCountAndNames: Team " +
                                     (i + 1).ToString() + " entered: " +
                                     inputField.GetComponent<Text>().text + ".  " +
                                     "Length was: " + inputField.GetComponent<Text>().text.Length
                                 );

                        // if the team name length is zero
                        if (teamNameLength == 0)
                        {
                            // call the function to show the error
                            Debug.Log(
                                      "GameManager.SubmitTeamCountAndNames: Error: Team: " +
                                       (i + 1).ToString() + " had a blank name.  Calling " +
                                       "ShowOneScreen func to show TeamNamesErrorScreen " +
                                       "then retrun control immediately aborting func. "
                                     );
                            ShowOneScreen("TeamNamesErrorScreen");

                            // return control because we don't want to do anything else                            
                            return;
                        }

                        // assign the team name to the list
                        listTeamNames.Add(teamName);

                        // we got what we came for now continue the iterator
                        continue;
                    } // end if (inputField.name == "UserInputText")    

                } // end loop through input field child objects

            }  // end else           

        } // end loop for inputfield objects

        // IF WE MADE IT THIS FAR THEN ALL TEAMS HAD SUFFICIENT NAME DATA

        // now ensure there aren't duplicate teams, convert all to lowercase
        for (int i = 0; i < listTeamNames.Count; ++i)
        {
            string tmpTeamName = listTeamNames[i].ToLower();
            for(int j = i+1; j < listTeamNames.Count; ++j)
            {
                if(tmpTeamName == listTeamNames[j].ToLower())
                {
                    // if this is true then we have a duplicate team name.  Raise the rror and return
                    ShowOneScreen("TeamNamesErrorScreen");
                    return;
                }
            }
        }


        // print the output of the list
        Debug.Log(
                     "GameManager.SubmitTeamNamesAndCount: Submitted Team Count: " +
                     gameData.selectedNumberOfTeams.ToString() + ".  Their Names Are..."
                 );
        for (int i = 0; i < gameData.SelectedNumberOfTeams; ++i)
        {
            Debug.Log(
                         "GameManager.SubmitTeamNamesAndCount: Team " + (i + 1).ToString() +
                         " : " + listTeamNames[i].ToString() + "."
                     );
        }

        // POPULATE TEAM ARRAY IN GAMEDATA
        // initialize the team array in gamedata to be the selected number of teams in gamedata
        gameData.ArrayTeams = new GameObject[gameData.SelectedNumberOfTeams];

        // use gamedata.selectednumberofteams as iteration limit to create teams
        for (int i = 0; i < gameData.SelectedNumberOfTeams; ++i)
        {
            // instantiate a new team and initialize
            GameObject gObj = new GameObject();
            gObj.AddComponent<Team>();
            Team team = gObj.GetComponent<Team>();
            team.TeamName = listTeamNames[i].ToString();

            // assign the correct team sheet to the team
            // first build the string for the correct object name based on the team number
            string teamInfoSheetName = "Team" + (i + 1).ToString() + "InfoSheet";
            GameObject teamInfoSheet = ReturnChildObject("TeamInfo", teamInfoSheetName);

            // assign the infosheet to the Team instance
            team.TeamInfoSheet = teamInfoSheet;

            // store the new team in GameData.arrayTeams
            gameData.arrayTeams[i] = gObj;
            Debug.Log(
                         "GameManager.SubmitTeamCountAndNames: added team: " +
                         gameData.arrayTeams[i] + " to GameData.arrayTeams " +
                         "at element: " + i.ToString() + "..."
                     );
        }

        // test the array of teams in gamedata
        for (int i = 0; i < gameData.arrayTeams.Length; ++i)
        {
            Debug.Log(
                         "GameData.arrayTeams[" + i.ToString() + "] Name: " +
                         gameData.arrayTeams[i].GetComponent<Team>().TeamName + "."
                     );
        }

        // show only the relevant stuff in standings box

        // get a handle to the standingsPanel
        GameObject panelStandings = null;
        foreach(KeyValuePair<GameObject,string> kvp in screenDictionary)
        {
            if(kvp.Value == "panelStandings")
            {
                panelStandings = kvp.Key;
            }
        }
        
        Transform[] standingsBoxTextFields = panelStandings.GetComponentsInChildren<Transform>(true);

        // loop through the array and deactivate the text fields we don't need for this game
        // be smart, start i at numteams
        int a = gameData.ArrayTeams.Length;
        for( int i = a; i < gameData.MaxNumberOfTeams; ++i)
        {
            //loop through the sandingsBoxTextFields array
            {
                for(int j = 0; j < standingsBoxTextFields.Length; ++j)
                {
                    // meaning, the sequence of the item, because the team sequence number exists in the name
                    if (standingsBoxTextFields[j].name.Contains((i+1).ToString()))
                    {
                        // deactivate the control
                        standingsBoxTextFields[j].gameObject.SetActive(false);
                    }
                }
            }
            
        }


        // set the gamestate to InitializeWorld
        eGameState = EGameState.InitializeWorld;

        Debug.Log(
                     "GameManager.SubmitTeamCountAndNames: Updated eGameState to InitializeWorld, " +
                     "calling InitializeGameWorld..."
                 );

        InitializeGameWorld();

	}



    /// <summary>
    /// Initialize the game world by disabling and enabling appropriate screens, initialize gameboard, and team sheets
    /// </summary>
    private void InitializeGameWorld()
    {
        Debug.Log("GameManager.InitializeGameWorld: Begin Function...");

        if (eGameState == EGameState.InitializeWorld)
        {
            // deactivate all screens 
            HideAllScreens();
            Debug.Log("GameManager.InitializeGameWorld: Disabled All Screens...");

            // create list of screens to enable 
            List<string> screensToEnable = new List<string>();
            screensToEnable.Add("GameBoard");
            screensToEnable.Add("TeamInfo");
            screensToEnable.Add("panelStandings");

            // toggle screens on
            Debug.Log("GameManager.InitializeGameWorld: calling GameManager.ToggleScreens...");
            ToggleScreens(screensToEnable, true);


			//audioManager.PlayScreenAudio("GameBoard");
			//audioManager.PlayOneShot(gameData.GetSoundClip("Swoosh"));

			//TODO ENSURE WE DEACTIVATE THE TEAMS SHEETS WHO ARE NOT PARTICIPATING

			// set each team's sheet active
			foreach (GameObject gObjTeam in gameData.ArrayTeams)
            {
                gObjTeam.GetComponent<Team>().TeamInfoSheet.SetActive(true);
            }

            // initialize our gameboard
            Debug.Log("GameManager.InitializeGameWorld: Calling InitializeGameBoard...");
            InitializeGameBoard();

            eGameState = EGameState.Turn;
            Debug.Log("GameManager.InitializeGameWorld: Set EGameState = Turn...");

            Debug.Log("GameManager.InitializeGameWorld: Need to set team turn text. Calling SetGameBoardTurnTeamText...");
            SetGameBoardTurnTeamText();

            // initialize the standings box
            Debug.Log("GameManager.InitializeGameBoard: Need to initialize standings box.  Calling UpdateStandings...");
            UpdateStandings();
            

            Debug.Log("GameManager.InitializeGameWorld: End Function...");
        }
    }


    private void HideAllScreens()
    {
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            kvp.Key.SetActive(false);
        }
    }

    private void ToggleScreens(List<string> in_ScreensToToggle, bool in_Active)
    {
		audioManager.PlayScreenAudio(in_ScreensToToggle[0]);
		audioManager.PlayOneShot(gameData.GetSoundClip("Swoosh"));
		Debug.Log("GameManager.ToggleScreens: Begin Function...");
        foreach (string s in in_ScreensToToggle)
		{
			foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
            {
                if (kvp.Value == s)
				{
					kvp.Key.SetActive(in_Active);
                    Debug.Log(
                                 "GameManager.ToggleScreens: Toggled Screen: " +
                                 kvp.Value + ", SetActive: " + in_Active);
                }
            }
        }
        Debug.Log("GameManager.ToggleScreens: End Function...");
    }




    /// <summary>
    /// When one or more team names do not contain text on the Team Names Entry Screen <br></br>
    /// the team names entry screen is hidden and an error screen pops up.  This func <br></br>
    /// is responsible for disabling the error screen upon acknowledging it and bringing<br></br>
    /// back up the team names entry screen.
    /// </summary>
    public void AcknowledgeTeamNamesErrorScreen()
    {
        ShowOneScreen("TeamCountAndNamesMenu");
    }


    /// <summary>
    /// Initialize the gameboard by finding card placement objects and filling the gameboard with cards
    /// </summary>
    private void InitializeGameBoard()
    {
        Debug.Log("GameManager.InitializeGameBoard: Begin Function...");


        // get a handle to each empty gameobject called CardPositionx where x in the number from left
        // to right, 1 to 3.  Each empty GameObject contains a child prefab called PhysicalCard used for 
        // loading a material to display the card with respect to a biome, and an ImageManager type (script)
        // component which is responsible for setting its card variable to the randomly pulled card.


        // first ensure our list of cards is populated, if not there is a problem
        if (gameData.ListCardDeck.Count == 0)
        {
            Debug.Log("GameManager.InitializeGameTable: FAIL; GameData.listCards does not contain cards!");
            return;
        }

        // get a handle to the gameboard and iterate through all card places named CardPositionx (children)
        string gameBoard = "GameBoard";
        GameObject gameBoardObject = null;
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            // if the value is the same as our gameboard
            if (kvp.Value == gameBoard)
            {
                // assign it to our temp var and break
                gameBoardObject = kvp.Key;
                break;
            }
        }

        if (gameBoardObject == null)
        {
            Debug.Log(
                      "GameManager.InitializeGameBoard: FAIL; could not find gameboard in the gameworld.  " +
                      "The object could've been deleted from the hierarchy, renamed, or its tag is changed " +
                      " from Screen."
                     );
            return;
        }


        // first get the child count of the parent object
        int cardPositionCount = gameBoardObject.transform.childCount;

        // make sure there are children
        if (cardPositionCount == 0)
        {
            Debug.Log("GameManager.InitializeGameBoard: FAIL The GameBoard world object was expected to have children.");
            return;
        }

        // loop number of max cards defined on a table in gamedata
        for (int i = 0; i < gameData.MaxNumberCardsOnTable; ++i)
        {
            //create the string we are looking for in children
            string nameToLookFor = "CardPosition" + (i + 1).ToString();

            // loop through all children on the parent object to find and store in class list
            for (int j = 0; j < cardPositionCount; ++j)
            {
                // set a simple var for the child
                Transform t = gameBoardObject.transform.GetChild(j);

                // if we found what we are looking for then store it
                if (t.name == nameToLookFor)
                {
                    listBoardCardPosition.Add(t.gameObject);
                    break;
                }
            }
        }

        // ensure we have a matching count of card position objects to what gamedata suggests
        if (listBoardCardPosition.Count != gameData.MaxNumberCardsOnTable)
        {
            Debug.Log(
                         "GameManager.InitializeGameBoard: FAIL: count of gameobjects to hold a card " +
                         "does not match gameData.  We found: " + listBoardCardPosition.Count + ", and " +
                         "GameData says we should have: " + gameData.MaxNumberCardsOnTable + "."
                     );

            // if we don't have a match then clear our list of cards
            listBoardCardPosition.Clear();
            return;
        }

        // populate the gameboard with cards
        for (int i = 0; i < listBoardCardPosition.Count; ++i)
        {
            InstantiateNewPhysicalCard(listBoardCardPosition[i]);

            //// randomly generate a card
            //int randCardNum = Random.Range(0, gameData.ListCardDeck.Count); // random.range is exclusive at the top

            //// create an instance of the physical card prefab using the iterator to pass into the physical card array
            //GameObject physicalCard = Instantiate(gameData.physicalCard[0], listBoardCardPosition[i].transform);

            //// set the object active
            //physicalCard.SetActive(true);

            //// set the imagemanger reference to GameManager (this)
            //physicalCard.GetComponent<ImageManager>().GameManager = this;


            //// set the parent of this physical card to the card position
            ////physicalCard.transform.SetParent(listBoardCardPosition[i].transform);

            //// initialize data on card positions (the components)

            //// set imageManager data which includes using a random card from the carddeck list
            //ImageManager imageManager = physicalCard.GetComponent<ImageManager>();
            //imageManager.SetCard(gameData.ListCardDeck[randCardNum]);

            //// remove this card from the deck and place into list of cards on table
            //gameData.ListCardsOnTable.Add(gameData.ListCardDeck[randCardNum]);
            //gameData.ListCardDeck.RemoveAt(randCardNum);
        }

        // set the text of the team turn on upper left of screen


        Debug.Log("GameManager.InitializeGameBoard: End Function...");
    }


    private void SetGameBoardTurnTeamText()
    {
        Debug.Log("GameManager.SetGameBoardTurnTeamText: Begin Function...");

        // get a handle to the child object of the GameBoard called lblTeamTurn
        GameObject lblTeamTurn = ReturnChildObject("GameBoard", "lblTeamTurn");

        // if we got a handle 
        if (!lblTeamTurn)
        {
            Debug.Log(
                         "GameManager.SetGameBoardTurnTeamText: FAIL: Did not find " +
                         "Text GameObject to set the team turn text."
                     );
            return;
        }


        // set the text to Team \n Team Name \n Turn
        string s = gameData.ArrayTeams[gameData.CurrentTurnTeam].GetComponent<Team>().TeamName;
        lblTeamTurn.GetComponent<Text>().text = "Team\n" + s +"'s" + "\nTurn";

        // echo the work
        Debug.Log("GameManager.SetGameBoardTurnTeamText: Updated GameBoard turn team text...");

        Debug.Log("GameManager.SetGameBoardTurnTeamText: End Function...");
    }



    public void CardClickEvent(GameObject in_PhysicalCardPrefab)
    {
        Debug.Log("GameManager.CardClickEvent: Begin Function...");

        // first set gamedata to update on selected card
        Debug.Log("GameManager.CardClickEvent: Calling SetSelectedCardGameData Function...");
        SetSelectedCardGameData(in_PhysicalCardPrefab);



        // initialize the universal question card object for the question
        Debug.Log("GameManager.CardClickEvent: Calling SetQuestionCardData Function...");
        SetQuestionCardData(in_PhysicalCardPrefab.GetComponent<ImageManager>().Card);

        Debug.Log("GameManager.CardClickEvent: End Function...");

    }

    private void SetSelectedCardGameData(GameObject in_PhysicalCardPrefab)
    {
        Debug.Log("GameManager.SetSelectedCardGameData: Begin Function...");

        // first extract Card instance from the prefab
        Card in_Card = in_PhysicalCardPrefab.GetComponent<ImageManager>().Card;

        // set gamedata selectedcard
        gameData.SelectedQuestionCard = in_Card;

        // set gamedata selected card question
        gameData.SelectedQuestionCardQuestion = in_Card.Question;

        // set gamedata selected PHYSICAL card (parent of the Card script component)
        gameData.SelectedPhysicalQuestionCard = in_PhysicalCardPrefab;

        // set gamedata selected PHYSICAL card parent object (the gameboard spot for this card)
        gameData.SelectedPhysicalQuestionCardPlacementObject =
            gameData.SelectedPhysicalQuestionCard.transform.parent.gameObject;

        // set gamedata selected card question point value
        gameData.SelectedQuestionPointValue = in_Card.pointValue;

        // set gamedata selected card question biometype
        gameData.SelectedQuestionEBiomeType = in_Card.eBiomeType;

        // set gamedata selected card question biome suit
        gameData.selectedQuestionEBiomeSuit = in_Card.EBiomeSuit;

        // set gamedata selected card question difficulty
        gameData.selectedQuestionEQuestionDifficulty = in_Card.eQuestionDifficulty;

        Debug.Log("GameManager.SetSelectedCardGameData: End Function...");

    }


    private void SetQuestionCardData(Card in_Card)
    {
        Debug.Log("GameManager.SetQuestionCardData: Begin Function...");

        //HideAllScreens();

        // get a handle to the universal question card
        //TODO CHANGE THIS SO WE DON'T HAVE TO FIND THE CARD EACH TIME
        GameObject questionCardGameObject = null;
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if (kvp.Value == "QuestionCard")
            {
                questionCardGameObject = kvp.Key;
                Debug.Log("we found the card");
            }
        }

        // set question card data
        Sprite questionSprite = FindSpriteByName(gameData.SelectedQuestionEBiomeType.ToString());
        questionCardGameObject.transform.localScale = Vector3.zero;
        questionCardGameObject.GetComponent<Image>().color = Color.white;
        questionCardGameObject.GetComponent<Image>().sprite = questionSprite;

        // check sprite name for left or right this depends on data positioning
        QuestionCard questionCard = questionCardGameObject.GetComponent<QuestionCard>();

        // fill in point value, biome type, and question
        if (questionSprite.name.Contains("Left"))
        {
            questionCard.txt[0].text = in_Card.eBiomeType.ToString();
            questionCard.txt[1].text = in_Card.pointValue.ToString();
            questionCard.txt[4].text = in_Card.Question;

            if (in_Card.eQuestionDifficulty == EQuestionDifficulty.Easy)
            {
                questionCard.image[1].sprite = gameData.questionOverlay[5];
            }

            if (in_Card.eQuestionDifficulty == EQuestionDifficulty.Medium)
            {
                questionCard.image[1].sprite = gameData.questionOverlay[4];
            }

            if (in_Card.eQuestionDifficulty == EQuestionDifficulty.Hard)
            {
                questionCard.image[1].sprite = gameData.questionOverlay[3];
            }
        }

        // else it's a right sided card
        else
        {
            questionCard.txt[2].text = in_Card.eBiomeType.ToString();
            questionCard.txt[3].text = in_Card.pointValue.ToString();
            questionCard.txt[5].text = in_Card.Question;

            if (in_Card.eQuestionDifficulty == EQuestionDifficulty.Easy)
            {
                questionCard.image[1].sprite = gameData.questionOverlay[2];
            }

            if (in_Card.eQuestionDifficulty == EQuestionDifficulty.Medium)
            {
                questionCard.image[1].sprite = gameData.questionOverlay[1];
            }

            if (in_Card.eQuestionDifficulty == EQuestionDifficulty.Hard)
            {
                questionCard.image[1].sprite = gameData.questionOverlay[0];
            }

        }

        Debug.Log("GameManager.SetQuestionCardData: Calling ShowOneScreen Function...");
        ShowOneScreen("QuestionCard");
        

        Debug.Log("GameManager.SetQuestionCardData: End Function...");

    }




    private Sprite FindSpriteByName(string in_Name)
    {
        Debug.Log("GameManager.FindSpriteByName: Begin Function...");

        Debug.Log(
                     "GameManager.FindSpriteByName: Finding Question Card sprite for Biome: " +
                     gameData.SelectedQuestionEBiomeType.ToString()
                 );

        Debug.Log(
                     "GameManager.FindSpriteByName: Question Card Background sprite array length: " +
                     gameData.questionBackground.Length + "."
                 );

        Sprite sprite = null;
        for (int i = 0; i < gameData.questionBackground.Length; ++i)
        {
            Debug.Log(
                         "GameManager.FindSpriteByName: Currently Browsed Sprite Name: " +
                         gameData.questionBackground[i].name + "..."
                     );
            if (gameData.questionBackground[i].name.Contains(gameData.SelectedQuestionEBiomeType.ToString()))
            {
                sprite = gameData.questionBackground[i];
                Debug.Log(
                             "GameManager.FindSpriteByName: Found Sprite Match: " +
                             gameData.questionBackground[i].name + "!");
                Debug.Log("GameManager.FindSpriteByName: End Function...");
                return sprite;
            }
        }

        // if we get here we did not find one so we will return a random one
        Debug.Log(
                     "GameManager.FindSpriteByName: WARNING!  Did not find sprite, " +
                     "supplying random sprite..."
                 );
        int random = Random.Range(0, gameData.questionBackground.Length);
        sprite = gameData.questionBackground[random];
        Debug.Log(
                     "GameManager.FindSpriteByName: Assigned random sprite: " +
                     gameData.questionBackground[random].name + "."
                 );

        Debug.Log("GameManager.FindSpriteByName: End Function...");
        return gameData.questionBackground[random];
    }



    public void QuestionAnsweredCorrectly()
    {
        Debug.Log("GameManager.QuestionAnsweredCorrectly: Begin Function...");

        Debug.Log("GameManager.QuestionAnsweredCorrectly: Calling FacilitateCorrectAnswer...");
        FacilitateCorrectAnswer();


        Debug.Log("GameManager.QuestionAnsweredCorrectly: End Function...");
    }

    public void QuestionAnsweredIncorrectly()
    {
        Debug.Log("GameManager.QuestionAnsweredInCorrectly: Begin Function...");

        Debug.Log("GameManager.QuestionAnsweredInCorrectly: Calling FacilitateInCorrectAnswer...");
        FacilitateIncorrectAnswer();
        

        Debug.Log("GameManager.QuestionAnsweredInCorrectly: End Function...");
    }


    private void FacilitateCorrectAnswer()
    {
        Debug.Log("GameManager.FacilitateCorrectAnswer: Begin Function...");

        // get a handle to the current answering team
        Team correctAnsweringTeam =
            gameData.ArrayTeams[gameData.currentAnsweringTeam].GetComponent<Team>();
        Debug.Log(
                   "GameManager.FacilitateCorrectAnswer: Correct Answering Team: " +
                   correctAnsweringTeam.TeamName + "."
                 );

        // assign the card points to the teams raw points
        int selectedQuestionPointValue = gameData.SelectedQuestionPointValue;
        correctAnsweringTeam.TeamPoints += selectedQuestionPointValue;
        Debug.Log("GameManager.FacilitateCorrectAnswer: Points Awarded: " + selectedQuestionPointValue + ".");

        // Check if this team has this biome in the biosphere

        // echo what we are doing
        Debug.Log(
                     "GameManager.FacilitateCorrectAnswer: Checking if selected question biome " +
                     "exists in " + correctAnsweringTeam.TeamName + "'s biosphere..."
                 );

        // assign temp vars
        bool bSelectedQuestionBiomeExistsInTeamBiosphere = false;
        EBiomeType selectedQuestionCardBiomeType = gameData.SelectedQuestionEBiomeType;

        // loop through biosphere array to find a match
        for (int i = 0; i < correctAnsweringTeam.ArrayBiosphere.Length; ++i)
        {
            // if we found a a non null entry
            if (correctAnsweringTeam.ArrayBiosphere[i])
            {
                // if we get a biome match
                if (selectedQuestionCardBiomeType == correctAnsweringTeam.ArrayBiosphere[i].EBiomeType)
                {
                    // set bool to true and break, this biome exists in team biosphere
                    bSelectedQuestionBiomeExistsInTeamBiosphere = true;
                    break;
                }
            }
        }
        // echo the result
        Debug.Log(
                     "GameManager.FacilitateCorrectAnswer: bSelectedQuestionBiomeExistsInTeamBiosphere " +
                     bSelectedQuestionBiomeExistsInTeamBiosphere
                 );


        // Check if this team has this biome in their suit and store the card if it exists for point comparison later
        bool bSelectedQuestionBiomeExistsInTeamSuit = false;

        EBiomeSuit eSelectedQuestionCardBiomeSuit = gameData.selectedQuestionEBiomeSuit;
        
        // echo what we are doing
        Debug.Log(
                     "GameManager.FacilitateCorrectAnswer: Checking if selected question biome " +
                     "exists in " + correctAnsweringTeam.TeamName + "'s " +
                     eSelectedQuestionCardBiomeSuit + " suit..."
                 );

        //TODO WE NEED TO IMPLEMENT THE BUMP MECHANIC BOOLEAN LOGIC INSIDE THESE SUIT LOOPS BY TESTING POINT VALUES


        // if the selected question card biome type is Dry
        if (eSelectedQuestionCardBiomeSuit == EBiomeSuit.Dry)
        {
            for (int i = 0; i < correctAnsweringTeam.ArrayBiomeSuitDry.Length; ++i)
            {
                if (correctAnsweringTeam.ArrayBiomeSuitDry[i])
                {
                    if (selectedQuestionCardBiomeType == correctAnsweringTeam.ArrayBiomeSuitDry[i].EBiomeType)
                    {
                        bSelectedQuestionBiomeExistsInTeamSuit = true;
                        break;
                    }
                }
            }
        }

        // if the selected question card biome type is Forest
        if (eSelectedQuestionCardBiomeSuit == EBiomeSuit.Forest)
        {
            for (int i = 0; i < correctAnsweringTeam.ArrayBiomeSuitForest.Length; ++i)
            {
                if (correctAnsweringTeam.ArrayBiomeSuitForest[i])
                {
                    if (selectedQuestionCardBiomeType == correctAnsweringTeam.ArrayBiomeSuitForest[i].EBiomeType)
                    {
                        bSelectedQuestionBiomeExistsInTeamSuit = true;
                        break;
                    }
                }
            }
        }

        // if the selected question card biome type is Grassy
        if (eSelectedQuestionCardBiomeSuit == EBiomeSuit.Grassy)
        {
            for (int i = 0; i < correctAnsweringTeam.ArrayBiomeSuitGrassy.Length; ++i)
            {
                if (correctAnsweringTeam.ArrayBiomeSuitGrassy[i])
                {
                    if (selectedQuestionCardBiomeType == correctAnsweringTeam.ArrayBiomeSuitGrassy[i].EBiomeType)
                    {
                        bSelectedQuestionBiomeExistsInTeamSuit = true;
                        break;
                    }
                }
            }
        }

        // if the selected question card biome type is Water
        if (eSelectedQuestionCardBiomeSuit == EBiomeSuit.Water)
        {
            for (int i = 0; i < correctAnsweringTeam.ArrayBiomeSuitWater.Length; ++i)
            {
                if (correctAnsweringTeam.ArrayBiomeSuitWater[i])
                { if (selectedQuestionCardBiomeType == correctAnsweringTeam.ArrayBiomeSuitWater[i].EBiomeType)
                    {
                        bSelectedQuestionBiomeExistsInTeamSuit = true;
                        break;
                    }
                }
            }
        }

        // echo the result
        Debug.Log(
                     "GameManager.FacilitateCorrectAnswer: bSelectedQuestionBiomeExistsInTeamSuit: " +
                     bSelectedQuestionBiomeExistsInTeamSuit
                 );

        // update gamedata
        Debug.Log("GameManager.FacilitateCorrectAnswer: Updating GameData...");

        // set gamedata boolean for question card biome existing in correct answering team biosphere
        gameData.BAwardedPointTeamCardBiomeExistsInTeamBiosphere = bSelectedQuestionBiomeExistsInTeamBiosphere;

        // set gamedata boolean for question card biome existing in correct answering team suit
        gameData.BAwardedPointTeamCardBiomeExistsInTeamSuit = bSelectedQuestionBiomeExistsInTeamSuit;

        // set gamedata correct answering team
        gameData.CorrectAnsweringTeam = gameData.currentAnsweringTeam;
        Debug.Log("GameManager.FacilitateCorrectAnswer: Updated GameData!");

        // STEPS TO PRODUCE NOW:
        // 0. already in gamedata: STORE THE GAMEOBJECT THE CHOSEN CARD IS A CHILD OF SO WE CAN SPAWN A NEW ONE IN ITS SPOT
        // 01.already in gamedata: STORE THE CARD (ITS A PREFAB) IN A TEMPORARY VARIABLE TO MOVE TO LATER
        // INSTANTIATE A NEW PREFAB AND SET ITS PARENT BUT ENSURE WE DISABLE IT UNTIL TEAM CHOOSES ACTION ON CARD THEY WON FIRST (AESTHETIC)

        // 1. GO BACK TO GAMEBOARD SCREEN
        // 2. PULL OUT THE CORRECT ANSWERING TEAM'S SHEET AUTOMAGICALLY
        // 3. ACTIVATE THE OVERLAY AT THE UPPER PORTION OF THE SCREEN WITH BUTTONS TO TAKE ACTION
        //    ON THE CARD THE CORRECT ANSWERING TEAM RECEIVED


        // deactivate the selected card prefab
        gameData.SelectedPhysicalQuestionCard.SetActive(false);
        Debug.Log("GameManager.FacilitateCorrectAnswer: Deactivated the selected physical card.");

        // remove the card from list of cards on the table;
        gameData.ListCardsOnTable.Remove(gameData.selectedQuestionCard);
        Debug.Log(
                     "GameManager.FacilitateCorrectAnswer: Removed card: " + gameData.selectedQuestionCard.name +
                     " from GameData.ListCardsOnTable."
                 );

        // update the standings
        Debug.Log("GameManager.FacilitateCorrectAnswer: Calling UpdateStandings...");
        UpdateStandings();


        // call InitializeCardPlaceState to initialize the state of prompting the player what to do with their card
        Debug.Log("GameManager.FacilitateCorrectAnswer: Calling InitializeCardPlaceState...");
        InitializePlaceCardState();

        Debug.Log("GameManager.FacilitateCorrectAnswer: End Function...");
    }


    private bool InstantiateNewPhysicalCard(GameObject in_GObjParentOfPhysicalCard)
    {
        Debug.Log("GameManager.InstantiateNewPhysicalcard: Begin Function...");

        // first set the state
        eGameState = EGameState.PlaceCard;

        // create temp var to return
        bool bInstantiatedNewPhysicalcard = false;

        // check if we can instantiate a new card by checking length of GameData.ListCardDeck
        if (gameData.ListCardDeck.Count == 0)
        {
            // just to be doubly sure, set our boolean to false
            bInstantiatedNewPhysicalcard = false;

            // echo the result and return
            Debug.Log("GameManager.InstantiateNewPhysicalCard: WARNING: THERE ARE NO MORE CARDS TO PLACE ON THE TABLE!");
            Debug.Log("GameManager.InstantiateNewPhysicalcard: End Function.");
            return bInstantiatedNewPhysicalcard;
        }

        // randomly generate a card
        int randCardNum = Random.Range(0, gameData.ListCardDeck.Count); // random.range is exclusive at the top

        // create an instance of the physical card prefab using the iterator to pass into the physical card array
        GameObject physicalCard = Instantiate(gameData.physicalCard[0], in_GObjParentOfPhysicalCard.transform);

        // set the location to be centered on the parent
        physicalCard.transform.localPosition = Vector3.zero;

        // set the object active
        physicalCard.SetActive(true);

        // set the imagemanger reference to GameManager (this)
        physicalCard.GetComponent<ImageManager>().GameManager = this;

        // set ImageManager script component data which includes using a random card from the carddeck list
        ImageManager imageManager = physicalCard.GetComponent<ImageManager>();
        imageManager.SetCard(gameData.ListCardDeck[randCardNum]);

        // remove this card from the deck and place into list of cards on table
        gameData.ListCardsOnTable.Add(gameData.ListCardDeck[randCardNum]);
        gameData.ListCardDeck.RemoveAt(randCardNum);

        return bInstantiatedNewPhysicalcard;

    }



    /// <summary>
    /// Upon getting the answer correct the game should transition to new state called PlaceCard <br></br> 
    /// where the game goes back to the gameboard with an overlay up top asking where they would like <br></br>
    /// to do with their card.  Only the actions available will allow corresponding buttons to be activated. <br></br>
    /// 
    /// </summary>
    private void InitializePlaceCardState()
    {
        Debug.Log("GameManager.InitializePlaceCardState: Begin Function...");

        // clean up the question card
		if (questionCard != null)
        questionCard.ResetCard();

        // begin by hiding all screens
        Debug.Log("GameManager.InitializePlaceCardState: Calling HideAllScreens...");
        HideAllScreens();
        

        // build a list of screens to show
        List<string> listScreensToShow = new List<string>();

        // add the gameboard
        listScreensToShow.Add("GameBoard");
        Debug.Log("GameManager.InitializePlaceCardState: Added GameBoard to list of screens to show.");

        // add the teamsheet for the correct answering team
        int i = gameData.CorrectAnsweringTeam;
        GameObject g = gameData.ArrayTeams[i];
        Team t = g.GetComponent<Team>();
        //string teamInfoSheet = t.TeamInfoSheet.name;
        listScreensToShow.Add("TeamInfo");
        Debug.Log("GameManager.InitializePlaceCardState: Added TeamInfo to list of screens to show.");

        // add the card info box
        listScreensToShow.Add("panelCardInfo");
        Debug.Log("GameManager.InitializePlaceCardState: Added panelCardInfo to list of screens to show.");

        // add the action box
        listScreensToShow.Add("panelCardAction");
        Debug.Log("GameManager.InitializePlaceCardState: Added panelCardAction to list of screens to show.");

        // add the standings box
        listScreensToShow.Add("panelStandings");
        Debug.Log("GameManager.InitializePlaceCardState: Added panelStandings to list of screens to show.");

        Debug.Log("GameManager.InitializePlaceCardState: Calling ToggleScreens...");
        ToggleScreens(listScreensToShow, true);

        // set the card data tab
        ReturnChildObject("panelCardInfo", "txtCardDetailBiome").GetComponent<Text>().text =
            gameData.SelectedQuestionEBiomeType.ToString();

        ReturnChildObject("panelCardInfo", "txtCardDetailSuit").GetComponent<Text>().text =
            gameData.selectedQuestionEBiomeSuit.ToString();

        ReturnChildObject("panelCardInfo", "txtCardDetailPoints").GetComponent<Text>().text =
            gameData.SelectedQuestionPointValue.ToString();


        // set collider of cards on table to false
        Debug.Log("GameManager.InitializePlaceCardState: Calling ToggleTableCard...");
        ToggleTableCard(false);

        // slide out the teaminfosheet
        Debug.Log("GameManager.InitializePlaceCardState: Sliding out the TeamInfoSheet for: " + t.TeamName + ".");
        t.teamInfoSheet.GetComponentInChildren<Button>().GetComponent<ButtonManager>().ViewTeamData();


        // slide out the points tab
        Debug.Log("GameManager.InitializePlaceCardState: Begin sliding out the PointsTab.");
        GameObject gObjPanelStandings = null;
        foreach(KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if (kvp.Value == "panelStandings")
            {
                Debug.Log("GameManager.InitializePlaceCardState: panelStandings found.");
                gObjPanelStandings = kvp.Key;                
            }
        }

        Debug.Log("GameManager.InitializePlaceCardState: Calling ToggleStandingsBox on panelStandings child button...");
        standingsBox.ToggleStandingsBox();

        // toggle buttons to what we can do and can't do with the card we got
        Debug.Log("GameManager.InitializePlaceCardState: Finding panelCardAction...");
        Button[] cardActionButtons = null;
        GameObject gObjPanelCardAction = GameObject.Find("panelCardAction");
        if(gObjPanelCardAction == null)
        {
            Debug.Log("GameManager.InitializePlaceCardState: FAIL: Could not find panelCardAction");
            return;
        }

        //get button components from children in panelCardAction
        cardActionButtons = gObjPanelCardAction.GetComponentsInChildren<Button>();
        Debug.Log("GameManager.InitializePlaceCardState: Obtained Access To " + cardActionButtons +" Buttons on panelCardAction.");

        foreach (Button b in cardActionButtons)
        {
            if (b.transform.name == "btnPlaceCardInBiosphere")
            {
                Debug.Log("GameManager.InitializePlaceCardState: Found btnPlaceCardInBiosphere...");
                b.interactable = !gameData.BAwardedPointTeamCardBiomeExistsInTeamBiosphere;
            }

            if (b.transform.name == "btnPlaceCardInSuit")
            {
                Debug.Log("GameManager.InitializePlaceCardState: Found btnPlaceCardInSuit...");
                b.interactable = !gameData.BAwardedPointTeamCardBiomeExistsInTeamSuit;
            }


            if (b.transform.name == "btnBumpSuitCardToBiosphere")
            {
                Debug.Log("GameManager.InitializePlaceCardState: Found btnBumpSuitCardToBiosphere...");
                b.interactable = !gameData.bAwardedPointTeamCardBiomeExistsInTeamBiosphere &&
                                  gameData.bAwardedPointTeamCardBiomeExistsInTeamSuit;
            }

            if (b.transform.name == "btnBumpSuitCard")
            {
                Debug.Log("GameManager.InitializePlaceCardState: Found btnBumpSuitCard...");
                b.interactable = gameData.bAwardedPointTeamCardBiomeExistsInTeamSuit &&
                                   !gameData.BAwardedPointTeamCardBiomePointsLessThanTeamSuitBiome;
            }
        }
        Debug.Log("GameManager.InitializePlaceCardState: End Function.");

    }


    public void SendCardToBiosphere()
    {
        Debug.Log("GameManager.SendCardToBiosphere: Begin Function...");
        Team t = gameData.ArrayTeams[gameData.CorrectAnsweringTeam].GetComponent<Team>();
        for(int i = 0; i < t.ArrayBiosphere.Length; ++i)
        {
            if(t.ArrayBiosphere[i] == null)
            {
                t.ArrayBiosphere[i] = gameData.SelectedQuestionCard;
                Debug.Log("GameManager.SendCardToBiosphere: Biome Card: " + gameData.SelectedQuestionCard.EBiomeType +
                          " went to Biosphere slot: " + i + ".");
                break;
            }
        }
        Debug.Log("GameManager.SendCardToBiosphere: Calling InitializePostCardAction...");
        InitializePostCardAction();
        Debug.Log("GameManager.SendCardToBiosphere: End Function...");

    }

    public void SendCardToSuit()
    {
        Debug.Log("GameManager.SendCardToSuit: Begin Function...");

        Team t = gameData.ArrayTeams[gameData.CorrectAnsweringTeam].GetComponent<Team>();

        // test for suit array to use
        if (gameData.SelectedQuestionCard.EBiomeSuit == EBiomeSuit.Dry)
        {
            for (int i = 0; i < t.ArrayBiomeSuitDry.Length; ++i)
            {
                if (t.ArrayBiomeSuitDry[i] == null)
                {
                    t.ArrayBiomeSuitDry[i] = gameData.SelectedQuestionCard;
                    Debug.Log("GameManager.SendCardToSuit: Biome Card: " + gameData.SelectedQuestionCard.EBiomeType +
                                " went to Dry slot: " + i + ".");
                    break;
                }
            }
        }

        if (gameData.SelectedQuestionCard.EBiomeSuit == EBiomeSuit.Forest)
        {
            for (int i = 0; i < t.ArrayBiomeSuitForest.Length; ++i)
            {
                if (t.ArrayBiomeSuitForest[i] == null)
                {
                    t.ArrayBiomeSuitForest[i] = gameData.SelectedQuestionCard;
                    Debug.Log("GameManager.SendCardToSuit: Biome Card: " + gameData.SelectedQuestionCard.EBiomeType +
                                " went to Forest slot: " + i + ".");
                    break;
                }
            }
        }

        if (gameData.SelectedQuestionCard.EBiomeSuit == EBiomeSuit.Grassy)
        {
            for (int i = 0; i < t.ArrayBiomeSuitGrassy.Length; ++i)
            {
                if (t.ArrayBiomeSuitGrassy[i] == null)
                {
                    t.ArrayBiomeSuitGrassy[i] = gameData.SelectedQuestionCard;
                    Debug.Log("GameManager.SendCardToSuit: Biome Card: " + gameData.SelectedQuestionCard.EBiomeType +
                                " went to Grassy slot: " + i + ".");
                    break;
                }
            }
        }

        if (gameData.SelectedQuestionCard.EBiomeSuit == EBiomeSuit.Water)
        {
            for (int i = 0; i < t.ArrayBiomeSuitWater.Length; ++i)
            {
                if (t.ArrayBiomeSuitWater[i] == null)
                {
                    t.ArrayBiomeSuitWater[i] = gameData.SelectedQuestionCard;
                    Debug.Log("GameManager.SendCardToSuit: Biome Card: " + gameData.SelectedQuestionCard.EBiomeType +
                                " went to Water slot: " + i + ".");
                    break;
                }
            }
        }

        Debug.Log("GameManager.SendCardToSuit: Calling InitializePostCardAction");
        InitializePostCardAction();
        Debug.Log("GameManager.SendCardToSuit: End Function...");
    }

    public void BumpSuitCardToBiosphere()
    {
        Debug.Log("GameManager.BumpSuitCardToBiosphere: Begin Function...");

        Debug.Log("GameManager.BumpSuitCardToBiosphere: Calling InitializePostCardAction");
        InitializePostCardAction();

        Debug.Log("GameManager.BumpSuitCardToBiosphere: End Function...");
    }

    public void BumpSuitCard()
    {
        Debug.Log("GameManager.BumpSuitCard: Begin Function...");

        Debug.Log("GameManager.BumpSuitCard: Calling InitializePostCardAction");
        InitializePostCardAction();

        Debug.Log("GameManager.BumpSuitCard: End Function...");
    }

    public void OmitCard()
    {
        Debug.Log("GameManager.OmitCard: Begin Function");

        // card needs to go to the unused pile
        Debug.Log("GameManager.OmitCard: Card: " + gameData.SelectedQuestionCard +
                  " is going to the list of cards removed from game.");
        gameData.ListCardsRemovedFromGame.Add(gameData.SelectedQuestionCard);

        Debug.Log("GameManager.OmitCard: Calling InitializePostCardAction");
        InitializePostCardAction();

        Debug.Log("GameManager.OmitCard: End Function");

    }





    private void ToggleTableCard(bool in_SetActive)
    {
        Debug.Log("GameManager.ToggleTableCard: Begin Function...");
        
        GameObject g = null;
        foreach(KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if(kvp.Value == "GameBoard")
            {
                Debug.Log("GameManager.ToggleTableCard: Found Gameboard...");
                g = kvp.Key;
            }
        }

        Transform[] childT = g.GetComponentsInChildren<Transform>(true);
        foreach(Transform t in childT)
        {
            if(t.name.Contains("Physical"))
            {
                t.gameObject.SetActive(in_SetActive);
                Debug.Log("GameManager.ToggleTableCard: Found physical card.  SetActive = " + in_SetActive);
            }
        }

        Debug.Log("GameManager.ToggleTableCard: End Function.");
    }


    private void InitializePostCardAction()
    {
        Debug.Log("GameManager.InitializePostCardAction: Begin Function...");

        // destroy the physical card that was just answered correctly
        GameObject.Destroy(gameData.SelectedPhysicalQuestionCard);

        // instantiate a new card in the place of the clicked card
        bool bWasCardDealt = InstantiateNewPhysicalCard(gameData.SelectedPhysicalQuestionCardPlacementObject);

        // set collider of cards on table to false
        Debug.Log("GameManager.InitializePlaceCardState: Calling ToggleTableCard...");
        ToggleTableCard(true);

        // push away the unnecessary screens
        // build a list of screens to hide
        List<string> listScreensToShow = new List<string>();
                
        //// add the teamsheet we pulled out for the correct answering team
        //int i = gameData.CorrectAnsweringTeam;
        //GameObject g = gameData.ArrayTeams[i];
        //Team t = g.GetComponent<Team>();
        //listScreensToShow.Add("TeamInfo");
        //Debug.Log("GameManager.InitializePlaceCardState: Added TeamInfo to list of screens to show.");

        // add the card info box
        listScreensToShow.Add("panelCardInfo");
        Debug.Log("GameManager.InitializePlaceCardState: Added panelCardInfo to list of screens to show.");

        // add the action box
        listScreensToShow.Add("panelCardAction");
        Debug.Log("GameManager.InitializePlaceCardState: Added panelCardAction to list of screens to show.");

        //// add the standings box
        //listScreensToShow.Add("panelStandings");
        //Debug.Log("GameManager.InitializePlaceCardState: Added panelStandings to list of screens to show.");

        Debug.Log("GameManager.InitializePlaceCardState: Calling ToggleScreens...");
        ToggleScreens(listScreensToShow, false);

        // slide in the teaminfosheet
        int i = gameData.CorrectAnsweringTeam;
        GameObject g = gameData.ArrayTeams[i];
        Team t = g.GetComponent<Team>();
        Debug.Log("GameManager.InitializePlaceCardState: Sliding out the TeamInfoSheet for: " + t.TeamName + ".");
        t.teamInfoSheet.GetComponentInChildren<Button>().GetComponent<ButtonManager>().ViewTeamData();
        
        // slide in the points tab
        Debug.Log("GameManager.InitializePlaceCardState: Begin sliding out the PointsTab.");
        GameObject gObjPanelStandings = null;
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if (kvp.Value == "panelStandings")
            {
                Debug.Log("GameManager.InitializePlaceCardState: panelStandings found.");
                gObjPanelStandings = kvp.Key;
            }
        }

        Debug.Log("GameManager.InitializePlaceCardState: Calling ToggleStandingsBox on panelStandings child button...");
        standingsBox.ToggleStandingsBox();

        // SET THE NEXT TEAM
        SetNextTurnTeam();

        // set the text on the gameboard to reflect the current turn team
        //SetGameBoardTurnTeamText();

        // set GameData Correct Answering Team = -1
        gameData.CorrectAnsweringTeam = -1;

        // set gamedata steal team to -1
        gameData.CurrentStealOpportunityTeam = -1;

        // set GameData Current Answering Team to current team
        gameData.CurrentAnsweringTeam = gameData.CurrentTurnTeam;       

        Debug.Log("GameManager.InitializePostCardAction: End Function.");
        
    }


    private void UpdateStandings()
    {
        Debug.Log("GameManager.UpdateStandings: Begin Fucntion...");
        List<Team> standingsList = new List<Team>();
        List<Team> tmpStandingsList = new List<Team>();
        // populate each list with the team array
        for(int i = 0; i < gameData.ArrayTeams.Length; ++i)
        {
            standingsList.Add(gameData.ArrayTeams[i].GetComponent<Team>());
        }

        for(int i = 0; i < standingsList.Count; ++i)
        {
            for(int j = 1; j < standingsList.Count; ++j)
            {
                if(standingsList[j].TeamPoints > standingsList[j -1].TeamPoints)
                {
                    // swap them
                    Team tmpTeam = standingsList[j];
                    standingsList[j] = standingsList[j - 1];
                    standingsList[j - 1] = tmpTeam;

                }
            }
        }



        //for(int i = 0; i < gameData.ArrayTeams.Length; ++i)
        //{
        //    Team team = gameData.ArrayTeams[i].GetComponent<Team>();
        //    int highestScore = team.TeamPoints;
            
        //    for (int j = 1; j < gameData.ArrayTeams.Length-1; ++j)
        //    {
        //        if (gameData.ArrayTeams[j].GetComponent<Team>().TeamPoints > highestScore)
        //        {
        //            team = gameData.ArrayTeams[j].GetComponent<Team>();
        //            highestScore = team.TeamPoints;

                    
        //        }
        //    }
        //    standingsList.Add(team);
        //}

        // get a handle to the standingsPanel
        GameObject panelStandings = null;
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if (kvp.Value == "panelStandings")
            {
                panelStandings = kvp.Key;
            }
        }

        Transform[] standingsBoxTextFields = panelStandings.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < standingsList.Count; ++i)
        {
            //loop through the sandingsBoxTextFields array
            {
                for (int j = 0; j < standingsBoxTextFields.Length; ++j)
                {
                    // meaning, the sequence of the item, because the team sequence number exists in the name
                    if (standingsBoxTextFields[j].name.Contains((i + 1).ToString())) 
                    {
                        //test what control it is:
                        if(standingsBoxTextFields[j].name.Contains("txtPlace"))
                        {
                            // set the text field for the team name
                            standingsBoxTextFields[j].GetComponent<Text>().text = standingsList[i].TeamName;
                        }

                        if (standingsBoxTextFields[j].name.Contains("lblPoints"))
                        {
                            standingsBoxTextFields[j].GetComponent<Text>().text = standingsList[i].TeamPoints.ToString();
                        }
                        
                    }
                }
            }

        }
        Debug.Log("GameManager.UpdateStandings: End Fucntion...");

    }

    public void FacilitateIncorrectAnswer()
    {
        Debug.Log("GameManager.FacilitateIncorrectAnswer: Begin Function...");

        // first check if this is a stealopportunity or a normal turn
        if (eGameState == EGameState.StealOpportunity)
        {
            // if so deduct the points from the incorrect answering steal team
            gameData.ArrayTeams[gameData.CurrentAnsweringTeam].GetComponent<Team>().TeamPoints -= gameData.SelectedQuestionPointValue;
            Debug.Log(
                         "GameManager.FacilitateIncorrectAnswer: This was a steal opportunity answered incorrectly.  " +
                         " Team " + gameData.ArrayTeams[gameData.CurrentAnsweringTeam].GetComponent<Team>().TeamName +
                         " MINUS " + gameData.SelectedQuestionPointValue
                     );

            // set the standings board
            UpdateStandings();

            Debug.Log("GameManager.FacilitateIncorrectAnswer: Updated Standings.");
        }

        // else the TURN team answered incorrectly and we should update the gamestate to be a steal opportunity 
        // and set the current steal opportunity team to be the team after the turn team
        else
        {
            //// update game state
            //eGameState = EGameState.StealOpportunity;
            //Debug.Log(
            //             "GameManager.FacilitateIncorrectAnswer: Found to be turn team answered incorrectly.  " +
            //             "Updated GameState to stealopportunity."
            //         );

            //// set current steal opportunity team to next team after turn team
            //if (gameData.CurrentTurnTeam == gameData.ArrayTeams.Length - 1)
            //{
            //    gameData.CurrentStealOpportunityTeam = 0;
            //}
            //else
            //{
            //    gameData.CurrentStealOpportunityTeam = gameData.CurrentTurnTeam + 1;
            //}

            Debug.Log(
                       "GameManager.FacilitateIncorrectAnswer: Found to be turn team answered incorrectly.  " +
                       "Nothing to do at this point..."
                   );
        }

        // in either case we still need to call HandleStealOpportunity
        Debug.Log("GameManager.FacilitateIncorrectAnswer: Calling HandleStealOpportunity...");
        HandleStealOpportunity();

        Debug.Log("GameManager.FacilitateIncorrectAnswer: End Function.");

    }


    public void HandleStealOpportunity()
    {
        Debug.Log("GameManager.HandleStealOpportunity: Begin Function...");

        // if gamestate is turn then we need to update it to steal opporutnity and initialize the steal opportunity team
        if (eGameState == EGameState.Turn)
        {
            // update game state
            eGameState = EGameState.StealOpportunity;
            Debug.Log(
                         "GameManager.FacilitateIncorrectAnswer: Found to be turn team answered incorrectly.  " +
                         "Updated GameState to stealopportunity."
                     );

            // set current steal opportunity team to next team after turn team
            if (gameData.CurrentTurnTeam == gameData.ArrayTeams.Length - 1)
            {
                gameData.CurrentStealOpportunityTeam = 0;
            }
            else
            {
                gameData.CurrentStealOpportunityTeam = gameData.CurrentTurnTeam + 1;
            }

            Debug.Log(
                         "GameManager.FacilitateIncorrectAnswer: Found to be turn team answered incorrectly.  " +
                         "Updating GameState.CurrentStealOpportunityTeam to: " +
                         gameData.ArrayTeams[gameData.CurrentStealOpportunityTeam].GetComponent<Team>().TeamName + "."
                     );

        }

        // else the state is already a steal opportunity
        else
        {
            // set the next team up for a steal opportunity
            if (gameData.CurrentStealOpportunityTeam == gameData.ArrayTeams.Length - 1)
            {
                gameData.CurrentStealOpportunityTeam = 0;
            }
            else
            {
                gameData.CurrentStealOpportunityTeam += 1;
            }

            Debug.Log(
                        "GameManager.FacilitateIncorrectAnswer: Updated GameData.CurrentStealOpportunityTeam to: " +
                        gameData.ArrayTeams[gameData.CurrentStealOpportunityTeam].GetComponent<Team>().TeamName + "."
                     );

        }

        // if currentstealopportunityteam is the same as turn team, then we went all the way around
        // so we must update data and display the answer:
        if (gameData.CurrentStealOpportunityTeam == gameData.CurrentTurnTeam)
        {
            Debug.Log("GameManager.HandleStealOpportunity: Steal opportunities wrapped around to turn team.");

            // reset current answering team to be -1
            gameData.CurrentAnsweringTeam = -1;
            Debug.Log("GameManager.HandleStealOpportunity: Set GameData.CurrentAnsweringTeam to -1.");

            // reset correct answering team to be -1
            gameData.CorrectAnsweringTeam = -1;
            Debug.Log("GameManager.HandleStealOpportunity: Set GameData.CorrectAnsweringTeam to -1.");
            
            // reset current steal opportunity team to be -1
            gameData.CurrentStealOpportunityTeam = -1;
            Debug.Log("GameManager.HandleStealOpportunity: Set GameData.CurrentStealOpportunityTeam to -1.");

            // place biome question card in list gamedata.listcardsremovedfromgame
            gameData.ListCardsRemovedFromGame.Add(gameData.SelectedQuestionCard);
            Debug.Log(
                          "GameManager.HandleStealOpportunity: Added current question card: " +
                          gameData.SelectedQuestionCard.name + " to GameData.ListCardsRemovedFromGame." 
                     );

            // remove the card from the card deck
            gameData.ListCardDeck.Remove(gameData.SelectedQuestionCard);
            Debug.Log("GameManager.HandleStealOpportunity: Removed card from deck.");

            // destroy the physical card
            GameObject.Destroy(gameData.SelectedPhysicalQuestionCard);

            // set next turn team
            Debug.Log("GameManager.HandleStealOpportunity: Calling SetNextTurnTeam...");
            SetNextTurnTeam();

            // display the answer
            Debug.Log("GameManager.HandleStealOpportunity: DISPLAY ANSWER HERE!!!!");            
        }

        // else we continue with a steal opportunity
        else
        {
            Debug.Log("GameManager.HandleStealOpportunity: Continuing Steal Opportunity To Next Team...");
            // ask the current steal opportunity team if they want to steal
            // show the query screen to ask if the team would like to steal
            // get a handle to the steal opportunity screen
            GameObject stealOpportunityScreen = null;
            foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
            {
                if (kvp.Value == "StealOpportunityScreen")
                {
                    stealOpportunityScreen = kvp.Key;
                    break;
                }
            }

            stealOpportunityScreen.SetActive(true);
            stealOpportunityScreen.GetComponentInChildren<Text>().text = "Team " +
                 gameData.ArrayTeams[gameData.CurrentStealOpportunityTeam].GetComponent<Team>().TeamName +
                 ", would you like to try and steal the question?  If you click YES " +
                 ", you must take the question.";
        }

        Debug.Log("GameManager.HandleStealOpportunity: End Function.");


        //// first see if the state is Turn state, and if so, set it to stealopportunity state
        //if (eGameState == EGameState.Turn)
        //{
        //    eGameState = EGameState.StealOpportunity;

        //    // go to the next team, because there will always be at least one team to query
        //    // first test if we are at the last team
        //    if (gameData.CurrentTurnTeam == gameData.ArrayTeams.Length - 1)
        //    {
        //        // set currentstealopportunityteam to 0
        //        gameData.CurrentStealOpportunityTeam = 0;
        //    }
        //    else
        //    {
        //        gameData.CurrentStealOpportunityTeam = gameData.CurrentTurnTeam + 1;
        //    }
        //}

        //else
        //{
        //    // get next team in line
        //    if (gameData.CurrentStealOpportunityTeam == gameData.ArrayTeams.Length - 1)
        //    {
        //        gameData.CurrentStealOpportunityTeam = 0;
        //    }
        //    else
        //    {
        //        gameData.CurrentStealOpportunityTeam = gameData.CurrentStealOpportunityTeam + 1;
        //    }
        //}

        //// if we circled back, then no one got the question right and we just need to come back to the gameboard
        //if (gameData.CurrentStealOpportunityTeam == gameData.CurrentTurnTeam)
        //{
        //    // list of screens to show
        //    List<string> listScreensToshow = new List<string>();
        //    listScreensToshow.Add("GameBoard");
        //    listScreensToshow.Add("panelStandings");

        //    // set the next turn team
        //    SetNextTurnTeam();

        //    // set the text for the turn team on the gameboard
        //    SetGameBoardTurnTeamText();
        //}

        //// else this is still a steal opportunity
        //else
        //{
        //    // show the query screen to ask if the team would like to steal
        //    // get a handle to the steal opportunity screen
        //    GameObject stealOpportunityScreen = null;
        //    foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        //    {
        //        if (kvp.Value == "StealOpportunityScreen")
        //        {
        //            stealOpportunityScreen = kvp.Key;
        //            break;
        //        }
        //    }

        //    stealOpportunityScreen.SetActive(true);
        //    stealOpportunityScreen.GetComponentInChildren<Text>().text = "Team " +
        //         gameData.ArrayTeams[gameData.CurrentStealOpportunityTeam].GetComponent<Team>().TeamName +
        //         ", would you like to try and steal the question?  If you click YES " +
        //         ", you must take the question.";
        //}
    }


    public void ButtonClickStealYes()
    {
        Debug.Log("GameManager.ButtonClickStealYes: Begin Function...");

        // hide the screen prompt
        GameObject stealOpportunityScreen = null;
        foreach (KeyValuePair<GameObject, string> kvp in screenDictionary)
        {
            if (kvp.Value == "StealOpportunityScreen")
            {
                stealOpportunityScreen = kvp.Key;
                break;
            }
        }
        stealOpportunityScreen.SetActive(false);
        Debug.Log("GameManager.ButtonClickStealYes: Deactivated steal prompt window.");

        // set gamedata.currentansweringteam to current steal opportunity team
        gameData.CurrentAnsweringTeam = gameData.CurrentStealOpportunityTeam;
    }

    public void ButtonClickStealNo()
    {
        // Just call HandleStealOpportunity
        HandleStealOpportunity();
    }
    


    private void SetNextTurnTeam()
    {
        // if the last team is going set it to the first
        if (gameData.CurrentTurnTeam == gameData.ArrayTeams.Length - 1)
        {
            gameData.CurrentTurnTeam = 0;
        }

        // else just add one
        else
        {
            ++gameData.CurrentTurnTeam;
        }

        // set the text on the gameboard
        SetGameBoardTurnTeamText();
    }


    // Properties

    public EGameState EGameState
    {
        get { return this.eGameState; }
    }
}
