﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCard : GameBase
{
    public bool awake;
    public Image[] image;
    public Text[] txt;
    public Sprite background;
    public GameData gameData;

    public ButtonManager correctButton;
    public ButtonManager incorrectButton;
    public GameObject buttons;

    public GameObject gameBoard;
    bool grow = true;

    float scale = 0.0f;
    bool showBtns = true;

    // Start is called before the first frame update
    void Start()
    {
        //buttons = GameObject.FindGameObjectWithTag("Buttons");
        //buttons.SetActive(false);
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if(Input.anyKeyDown)
            {
                if (!buttons.activeSelf)
                {
                    buttons.SetActive(true);
                }
                else if (!Input.GetKeyDown(KeyCode.Mouse0))
                {
                    buttons.SetActive(false);
                }
            }

            if (!awake && grow)
            {
                buttons.SetActive(false);
                showBtns = false;
                //ConstructQuestionCard();
                awake = true;
            }
            else if (awake && !grow && scale <= 0.0f)
            {
                awake = false;
                grow = true;
                gameObject.SetActive(false);
            }

            transform.localScale = new Vector3(scale, scale, 1.0f);
        }

        if (grow && awake)
        {
            scale = Mathf.Clamp(scale + Time.deltaTime, 0.0f, 1.0f);
        }
        else if (!grow)
        {
            scale = Mathf.Clamp(scale - Time.deltaTime, 0.0f, 1.0f);
        }
    }

    void ConstructQuestionCard()
    {
        BackgroundImage();
        QuestionOverlay();
    }

    void BackgroundImage()
    {
        image[0] = gameObject.GetComponent<Image>();
        transform.localScale = Vector3.zero;
        image[0].color = Color.white;
        image[0].sprite = gameData.questionBackground[gameData.questionBiome];
    }

    void QuestionOverlay()
    {
        string points = gameData.questionPointValue.ToString() + "pt";
        //string category = "";
        string category = gameData.SelectedQuestionEBiomeType.ToString();


        switch (gameData.questionBiome)
        {
            case 0:
                category = "Desert";
                break;
            case 1:
                category = "Rainforest";
                break;
            case 2:
                category = "Temparate";
                break;
            case 3:
                category = "Tundra";
                break;
        }

        if (gameData.questionBiome % 2 == 0)
        {
            txt[2].text = category;
            txt[3].text = points;
            txt[5].text = "Questions Go Here";
            switch (gameData.questionPointValue)
            {
                case 10:
                    image[1].sprite = gameData.questionOverlay[2];
                    break;
                case 20:
                    image[1].sprite = gameData.questionOverlay[1];
                    break;
                case 30:
                    image[1].sprite = gameData.questionOverlay[0];
                    break;
            }
        }
        else
        {
            txt[0].text = category;
            txt[1].text = points;
            txt[4].text = "Questions Go Here";
            switch (gameData.questionPointValue)
            {
                case 10:
                    image[1].sprite = gameData.questionOverlay[5];
                    break;
                case 20:
                    image[1].sprite = gameData.questionOverlay[4];
                    break;
                case 30:
                    image[1].sprite = gameData.questionOverlay[3];
                    break;
            }
        }
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void ResetCard()
    {
        buttons.SetActive(false);
        for (int i = 0; i < txt.Length; i++)
        {
            txt[i].text = "";
        }
        grow = true;
        scale = 0.0f;
        awake = false;
    }
}
