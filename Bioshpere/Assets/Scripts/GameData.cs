﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Data Container", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    //public class Team
    //{
    //    public string name;
    //    public int score  ;
    //}

    [SerializeField]
    public Sprite[] questionBackground                                                     ;
    public Sprite[] questionOverlay                                                        ;
    public Texture[] textures                                                              ;
    public int questionBiome                                                               ;
    public int questionPointValue                                                          ;
    public Color[] colors            = new Color[] { Color.green, Color.yellow, Color.red };
    //public Team[] arrayTeams         = new Team[2]                                       ;
    public GameObject[] arrayTeams                                                         ;
    public int defaultNumberOfTeams  = 2                                                   ;
    public int maxNumberOfTeams      = 4                                                   ;
    public int minNumberOfTeams      = 2                                                   ;
    public int selectedNumberOfTeams = 2                                                   ;
    public int currentTurnTeam       = 0                                                   ;
    public int currentAnsweringTeam  = 0                                                   ;
    public int correctAnsweringTeam  = -1                                                  ;
    public int currentStealOpportunityTeam = -1;
    public bool bAwardedPointTeamCardBiomeExistsInTeamBiosphere                            ;
    public bool bAwardedPointTeamCardBiomeExistsInTeamSuit                                 ;
    public bool bAwardedPointTeamCardBiomePointsLessThanTeamSuitBiome                      ;
    public int maxNumberCardsOnTable = 3                                                   ;
    public List<Card> listCardDeck = new List<Card>()                                      ;
    public List<Card> listCardsOnTable = new List<Card>()                                  ;
    public List<Card> listCardsRemovedFromGame = new List<Card>()                          ;
    public GameObject[] physicalCard;
    

    public Card selectedQuestionCard;
    public GameObject selectedPhysicalQuestionCard;
    public GameObject selectedPhysicalQuestionCardPlacementObject;
    public EBiomeType selectedQuestionEBiomeType;
    public string selectedQuestionCardQuestion;
    public EQuestionDifficulty selectedQuestionEQuestionDifficulty;
    public EBiomeSuit selectedQuestionEBiomeSuit;
    public int selectedQuestionPointValue;

	[Header("Audio Files")]
	[SerializeField] List<AudioClip> soundClips = new List<AudioClip>();
    
    // Properties

    
    public int MaxNumberOfTeams
    {
        get { return this.maxNumberOfTeams; }
    }

    public int MinNumberOfTeams
    {
        get { return this.minNumberOfTeams; }
    }

    public int DefaultNumberOfTeams
    {
        get { return this.defaultNumberOfTeams; }
    }

    public int SelectedNumberOfTeams
    {
        set { selectedNumberOfTeams = value; }
        get { return this.selectedNumberOfTeams; }
    }

    public int CurrentTurnTeam
    {
        set { currentTurnTeam = value; }
        get { return this.currentTurnTeam; }
    }

    public int CurrentAnsweringTeam
    {
        get { return this.currentAnsweringTeam; }
        set { this.currentAnsweringTeam = value; }
    }

    public int CorrectAnsweringTeam
    {
        get { return this.correctAnsweringTeam; }
        set { this.correctAnsweringTeam = value; }
    }

    public GameObject[] ArrayTeams
    {
        get { return this.arrayTeams; }
        set { this.arrayTeams = value; }
    }

    public List<Card> ListCardDeck
    {
        get { return this.listCardDeck; }
        set { this.listCardDeck = value; }
    }


    public List<Card> ListCardsOnTable
    {
        get { return this.listCardsOnTable; }
        set { this.listCardsOnTable = value; }
    }

    public List<Card> ListCardsRemovedFromGame
    {
        get { return this.listCardsRemovedFromGame; }
        set { this.listCardsRemovedFromGame = value; }
    }

    public int MaxNumberCardsOnTable
    {
        get { return this.maxNumberCardsOnTable; }
    }

    public Card SelectedQuestionCard
    {
        get { return this.selectedQuestionCard; }
        set { this.selectedQuestionCard = value; }
    }

    public GameObject SelectedPhysicalQuestionCard
    {
        get { return this.selectedPhysicalQuestionCard; }
        set { this.selectedPhysicalQuestionCard = value; }
    }

    public GameObject SelectedPhysicalQuestionCardPlacementObject
    {
        get { return this.selectedPhysicalQuestionCardPlacementObject; }
        set { this.selectedPhysicalQuestionCardPlacementObject = value; }
    }

    public EBiomeType SelectedQuestionEBiomeType
    {
        get { return this.selectedQuestionEBiomeType; }
        set { this.selectedQuestionEBiomeType = value; }
    }

    public string SelectedQuestionCardQuestion
    {
        get { return this.selectedQuestionCardQuestion; }
        set { this.selectedQuestionCardQuestion = value; }
    }

    public int SelectedQuestionPointValue
    {
        get { return this.selectedQuestionPointValue; }
        set { this.selectedQuestionPointValue = value; }
    }

    public EQuestionDifficulty SelectedEQuestionDifficulty
    {
        get { return this.selectedQuestionEQuestionDifficulty; }
        set { this.selectedQuestionEQuestionDifficulty = value; }
    }

    public bool BAwardedPointTeamCardBiomeExistsInTeamBiosphere
    {
        get { return this.bAwardedPointTeamCardBiomeExistsInTeamBiosphere; }
        set { this.bAwardedPointTeamCardBiomeExistsInTeamBiosphere = value; }
    }

    public bool BAwardedPointTeamCardBiomeExistsInTeamSuit
    {
        get { return this.bAwardedPointTeamCardBiomeExistsInTeamSuit; }
        set { this.bAwardedPointTeamCardBiomeExistsInTeamSuit = value; }
    }

    public bool BAwardedPointTeamCardBiomePointsLessThanTeamSuitBiome
    {
        get { return this.bAwardedPointTeamCardBiomePointsLessThanTeamSuitBiome; }
        set { this.bAwardedPointTeamCardBiomePointsLessThanTeamSuitBiome = value; }
    }

    public int CurrentStealOpportunityTeam
    {
        get { return this.currentStealOpportunityTeam; }
        set { this.currentStealOpportunityTeam = value; }
    }

	public AudioClip GetSoundClip(string clipName)
	{
		foreach (AudioClip clip in soundClips)
		{
			if (clip.name == clipName)
				return clip;
		}
		return null;
	}


















}

