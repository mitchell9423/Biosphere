using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : GameBase
{
    public ImageManager newCard;
    public Card currentCard;
    public bool slotSelected = false;
    public int score;
    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void SetNewCard(Card card)
    {
        newCard.SetCard(card);
        currentCard = card;
        score += card.pointValue;
        scoreText.text = "Score: " + score.ToString();
    }

    public void FillSlot(ImageManager slot)
    {
        if (!slotSelected)
        {
            slot.card = currentCard;
            slot.DisplayCard();
            slot.isEmpty = false;
            slotSelected = true;
        }
    }

}
