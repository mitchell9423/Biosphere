using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Card card;

    [SerializeField]
    public GameObject gameBoard;
    public ScoreBoard scoreBoard;
    public QuestionCard questionCard;

    [Header("Scoreboard")]
    public Sprite[] image;

    [Header("TeamProperties")]
    [SerializeField] public int numberOfTeams;

    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ShowQuestion(Card selectedCard)
    {
        card = selectedCard;
        card.gamedata.questionBiome = card.biome;
        card.gamedata.questionPointValue = card.pointValue;
        gameBoard.SetActive(false);
        questionCard.SetActive(true);
    }

    public void GoToScoreboard()
    {
        questionCard.ResetCard();
        scoreBoard.SetActive(true);
        scoreBoard.SetNewCard(card);
    }

    public void GoToGameBoard()
    {
        scoreBoard.slotSelected = false;
        scoreBoard.SetActive(false);
        questionCard.SetActive(false);
        gameBoard.SetActive(true);
    }
}
