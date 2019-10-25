using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Data Container", menuName = "Data Container")]
public class GameData : ScriptableObject
{
    public class Team
    {
        public string name;
        public int score;
    }

    [SerializeField]
    public Texture[] textures;
    public Color[] colors = new Color[] { Color.green, Color.yellow, Color.red };
    public int numberTeams = 2;
    public int currentTeam = 0;
    public Sprite[] questionBackground;
    public Sprite[] questionOverlay;
    public int questionBiome;
    public int questionPointValue;

    public Team[] teams = new Team[2];
}

