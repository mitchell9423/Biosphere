using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    private const int V = 0;
    public int pointValue;
    public int biome;
    int colorIndex = 0;
    public Color color;
    int textureIndex;
    public Texture texture;
    public GameData gamedata;

    public void AssignRandomComponents()
    {
        textureIndex = Random.Range(0, 4);
        colorIndex = Random.Range(0, 3);
        texture = gamedata.textures[textureIndex];
        color = gamedata.colors[colorIndex];
        SetPointValue();
    }

    public void SetPointValue()
    {
        pointValue = (colorIndex + 1) * 10;
        biome = textureIndex;
    }
}
