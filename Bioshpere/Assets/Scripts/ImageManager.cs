using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageManager : GameBase
{
    [SerializeField]
    public Texture defaultTexture;
    public Material material;
    public Card card;
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
        Debug.Log("ImageManager.OnMouseDown: Begin Function...");

        Debug.Log("ImageManager.OnMouseDown: User clicked on card: " + this.card.name);
        
        // store the physical card prefab
        GameObject physicalCardClicked = transform.gameObject;
        Debug.Log(
                     "ImageManager.OnMouseDown: Stored card prefab: " + physicalCardClicked.name +
                     " located in position: " + physicalCardClicked.transform.parent.name
                 );

        // call GameManager.CardClickEvent
        Debug.Log("ImageManager.OnMouseDown: Calling GameManager.CardClickEvent...");
        gameManager.CardClickEvent(physicalCardClicked);

        Debug.Log("ImageManager.OnMouseDown: End Function.");


        //gameManager.ShowQuestion(card);

        //if (gameObject.tag == "Card")
        //{
        //    gameManager.ShowQuestion(card);
        //}

        //if (gameObject.tag == "ScoreCard")
        //{
        //    if (isEmpty)
        //        scoreBoard.FillSlot(gameObject.GetComponent<ImageManager>());
        //}
    }

    public void DisplayCard()
    {
        //ADDED FOLLOWING CODE IN THIS WRAPPER
        //transform.gameObject.AddComponent<Renderer>();
        Material matInst = new Material(transform.GetComponent<Image>().material);
        transform.gameObject.GetComponent<ImageManager>().material = matInst;
        transform.gameObject.GetComponent<Image>().material = matInst;
        //Material matInst = transform.GetComponent<Image>().material;
        matInst.SetTexture("Texture2D_59B7C346", card.texture);
        matInst.SetColor("Color_7B8D3D8F", card.color);
        // END WRAPPPER

        // this was originally running
        //material.SetTexture("Texture2D_59B7C346", card.texture);
        //material.SetColor("Color_7B8D3D8F", card.color);
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

    public Card Card
    {
        get { return this.card; }
        set { this.card = value; }
    }
}
