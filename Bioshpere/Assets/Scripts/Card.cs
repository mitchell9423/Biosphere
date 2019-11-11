﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public EBiomeType eBiomeType;
    public EBiomeSuit eBiomeSuit;
    public EQuestionDifficulty eQuestionDifficulty;
    [TextArea(1, 20)]
    public string Question;
    public Color color;

    // unused stuff
    public int pointValue;
    public int biome;
    int colorIndex = 0;
    int textureIndex;
    public Texture texture;
    public GameData gamedata;

    public void AssignRandomComponents()
    {
        //textureIndex = Random.Range(0, 4);
        //colorIndex = Random.Range(0, 3);
        //texture = gamedata.textures[textureIndex];
        //color = gamedata.colors[colorIndex];
        SetPointValue();
    }

    //public void SetPointValue()
    //{
    //    pointValue = (colorIndex + 1) * 10;
    //    biome = textureIndex;
    //}

    public void AssignComponentData()
    {
        SetTexture();
        SetPointValue();
        SetOutlineColor();
    }


    private void SetTexture()
    {
        // series of if statements for putting the texture on the card
        if (eBiomeType == EBiomeType.Desert)
        {
            texture = gamedata.textures[0];
        }

        if (eBiomeType == EBiomeType.Estuary)
        {
            texture = gamedata.textures[1];
        }

        if (eBiomeType == EBiomeType.Fresh)
        {
            texture = gamedata.textures[1];
        }

        if (eBiomeType == EBiomeType.Grassland)
        {
            texture = gamedata.textures[2];
        }

        if (eBiomeType == EBiomeType.Rain)
        {
            texture = gamedata.textures[2];
        }

        if (eBiomeType == EBiomeType.Salt)
        {
            texture = gamedata.textures[1];
        }

        if (eBiomeType == EBiomeType.Savannah)
        {
            texture = gamedata.textures[5];
        }

        if (eBiomeType == EBiomeType.Taiga)
        {
            texture = gamedata.textures[4];
        }

        if (eBiomeType == EBiomeType.Temperate)
        {
            texture = gamedata.textures[2];
        }

        if (eBiomeType == EBiomeType.Tundra)
        {
            texture = gamedata.textures[3];
        }
    }

    private void SetPointValue()
    {
        // series of if statements
        if(eQuestionDifficulty == EQuestionDifficulty.Easy)
        {
            pointValue = 10;
        }
        
        if(eQuestionDifficulty == EQuestionDifficulty.Medium)
        {
            pointValue = 20;
        }

        if(eQuestionDifficulty == EQuestionDifficulty.Hard)
        {
            pointValue = 30;
        }
    }

    
    private void SetOutlineColor()
    {
        // series of if statements
        if(eQuestionDifficulty == EQuestionDifficulty.Easy)
        {
            color = gamedata.colors[0];
        }

        if(eQuestionDifficulty == EQuestionDifficulty.Medium)
        {
            color = gamedata.colors[1];
        }

        if(eQuestionDifficulty == EQuestionDifficulty.Hard)
        {
            color = gamedata.colors[2];
        }
    }


    // PROPERTIES

    public EBiomeType EBiomeType
    {
        get { return this.eBiomeType; }
        set { this.eBiomeType = value; }
    }

    public EBiomeSuit EBiomeSuit
    {
        get { return this.eBiomeSuit; }
        set { this.eBiomeSuit = value; }
    }

    public EQuestionDifficulty EQuestionDifficulty
    {
        get { return this.eQuestionDifficulty; }
        set { this.eQuestionDifficulty = value; }
    }

}
        



