using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageManager : MonoBehaviour
{
    [SerializeField]
    public Texture defaultTexture;
    public Material material;
    public Card card;
    public GameManager gameManager;
    public ScoreBoard scoreBoard;
    public bool isEmpty = true;
    public bool isSelected = false;

    private void Start()
    {
        if (gameObject.tag == "ScoreCard")
        {
            ResetImage();
        }

        if (gameObject.tag == "Card")
        {
            card.AssignRandomComponents();
            DisplayCard();
        }
    }

    private void OnMouseDown()
    {
        if (gameObject.tag == "Card")
        {
            gameManager.ShowQuestion(card);
        }

        if (gameObject.tag == "ScoreCard")
        {
            if (isEmpty)
                scoreBoard.FillSlot(gameObject.GetComponent<ImageManager>());
        }
    }

    public void DisplayCard()
    {
        material.SetTexture("Texture2D_59B7C346", card.texture);
        material.SetColor("Color_7B8D3D8F", card.color);
    }

    public void ResetImage()
    {
        material.SetTexture("Texture2D_59B7C346", defaultTexture);
    }

    public void Shuffle()
    {
        card.AssignRandomComponents();
        DisplayCard();
    }

    public void SetCard(Card card)
    {
        this.card = card;
        DisplayCard();
    }

    public void FillSlot()
    {

    }
}
