﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : GameBase
{
    static bool teamPageExtended = false;
    [SerializeField] RectTransform TeamInfoPage;
    bool showBtns = true;
    bool showTeamInfo;
    Vector3 closedPos;

    bool bShowStandings = false;
    Vector3 standingsClosedPos;
    Vector3 standingsOpenPos;

    // team sheet stuff
    float t = 0.0f;
    float tDefault = (1.0f / 256.0f);

    float u = 0.0f;
    float uDefault = (1.0f / 256.0f);


    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.CompareTag("TeamInfoTab"))
        {
            closedPos = TeamInfoPage.localPosition;
        }

        if (gameObject.transform.parent.name == "panelStandings")
        {
            standingsClosedPos = gameObject.transform.parent.position;
            standingsOpenPos = GameObject.Find("standingsDownPosition").transform.position;
        }

        
    }


    // Update is called once per frame
    void LateUpdate()
    {
        //if (gameObject.CompareTag("TeamInfoTab"))
        //	TranslateTeamInfoPage();
    }

    private void Update()
    {
        if (gameObject.CompareTag("TeamInfoTab"))
        {
            t += tDefault;
            TranslateTeamInfoPage();
        }
    }




    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }


    public void ViewTeamData()
    {
        t = tDefault;
        Debug.Log("ButtonManager.ViewTeamData: Begin Function...");

        if (!teamPageExtended)
        {
            Debug.Log("ButtonManager.ViewTeamData: Extending team info sheet...");
            teamPageExtended = true;
            showTeamInfo = true;
        }
        else if (showTeamInfo)
        {
            Debug.Log("ButtonManager.ViewTeamData: Retracting team info sheet...");
            teamPageExtended = false;
            showTeamInfo = false;
        }

        Debug.Log("ButtonManager.ViewTeamData: End Function...");
    }


    private void TranslateTeamInfoPage()
    {
        float x = TeamInfoPage.localPosition.x;
        if (showTeamInfo)
        {
            TeamInfoPage.localPosition = Vector3.Lerp(TeamInfoPage.localPosition, Vector3.zero, t);
            //TeamInfoPage.localPosition = Vector3.MoveTowards(TeamInfoPage.localPosition, Vector3.zero, 15.0f);
        }
        else
        {
            TeamInfoPage.localPosition = Vector3.Lerp(TeamInfoPage.localPosition, closedPos, t);
            //TeamInfoPage.localPosition = Vector3.MoveTowards(TeamInfoPage.localPosition, closedPos, 15.0f);
        }
    }


    /// <summary>
    /// Called from button on Main Menu screen responsible for incrementing the team count
    /// </summary>
    public void IncrementTeamcount()
    {
        gameManager.IncrementTeamCount();
    }


    /// <summary>
    /// Called from button on Main Menu screen responsible for decrementing the team count
    /// </summary>
    public void DecrementTeamCount()
    {
        gameManager.DecrementTeamCount();
    }


    public void SubmitTeamCountAndNames()
    {
        gameManager.SubmitTeamCountAndNames();
    }

    public void AcknowledgeTeamNamesErrorScreen()
    {
        gameManager.AcknowledgeTeamNamesErrorScreen();
    }


    /// <summary>
    /// Called when the CORRECT ANSWER button is pressed from a question screen
    /// </summary>
    public void QuestionAnsweredCorrectly()
    {
        Debug.Log("ButtonManager.QuestionAnsweredCorrectly: Begin Function...");

        Debug.Log("ButtonManager.QuestionAnsweredCorrectly: Calling GameManager.QuestionAnsweredCorrectly...");
        gameManager.QuestionAnsweredCorrectly();

        Debug.Log("ButtonManager.QuestionAnsweredCorrectly: End Function.");
    }


    /// <summary>
    /// Called when the INCORRECT ANSWER button is selected from a question screen
    /// </summary>
    public void QuestionAnsweredIncorrectly()
    {
        Debug.Log("ButtonManager.ToggleStandingsBox: Begin Function...");

        gameManager.FacilitateIncorrectAnswer();

        Debug.Log("ButtonManager.ToggleStandingsBox: End Function.");
    }


    public void SendCardToBiosphere()
    {
        GameManager.SendCardToBiosphere();
    }

    public void SendCardToSuit()
    {
        GameManager.SendCardToSuit();
    }

    public void BumpSuitCardToBiosphere()
    {
        GameManager.BumpSuitCardToBiosphere();
    }

    public void BumpSuitCard()
    {
        GameManager.BumpSuitCard();
    }

    public void OmitCard()
    {
        GameManager.OmitCard();
    }

    public void btnClickYesToSteal()
    {
        gameManager.ButtonClickStealYes();
    }

    public void btnClickNoToSteal()
    {
        gameManager.ButtonClickStealNo();
    }
  


}
