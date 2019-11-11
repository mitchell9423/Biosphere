using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Team : MonoBehaviour
{
    // declaration section

    public string teamName = ""           ;    // team name
    public int teamPoints = 0             ;    // team raw points (no multipliers factored in)
    public Texture teamMascot = null         ;    // team Mascot which is an animal placed on their team sheet
    public GameObject teamInfoSheet = null  ;    // team information showing cards collected and point value

    // arrays to hold cards
    public Card[] arrayBiosphere = new Card[10];    // array of biosphere cards
    public Card[] arrayBiomeSuitWater = new Card[3];    // array of biome suit Water (estuary, salt, fresh)
    public Card[] arrayBiomeSuitDry = new Card[2];    // array of biome suit Dry (desert, tundra)
    public Card[] arrayBiomeSuitGrassy = new Card[2] ;    // array of biome suit Grassy (savannah, grassland)
    public Card[] arrayBiomeSuitForest = new Card[3];    // array of biome suit Forest (temperate, rain, taiga)

    // Start is called before the first frame update
    void Start()
    {
        // initialize the team data
        //teamPoints           = 0           ;
        //teamName             = ""          ;
        //teamMascot           = null        ;
        //teamInfoSheet        = null;

        //arrayBiosphere       = new Card[10];
        //arrayBiomeSuitWater  = new Card[3] ;
        //arrayBiomeSuitDry    = new Card[2] ;
        //arrayBiomeSuitGrassy = new Card[2] ;
        //arrayBiomeSuitForest = new Card[3] ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    // PROPERTIES

    public string TeamName
    {
        get { return this.teamName; }
        set { this.teamName = value; }
    }

    public int TeamPoints
    {
        get { return this.teamPoints; }
        set { this.teamPoints = value; }
    }

    public GameObject TeamInfoSheet
    {
        get { return this.teamInfoSheet; }
        set { this.teamInfoSheet = value; }
    }

    public Texture TeamMascot
    {
        get { return this.teamMascot; }
        set { this.teamMascot = value; }
    }

    public Card[] ArrayBiosphere
    {
        get { return this.arrayBiosphere; }
        set { this.arrayBiosphere = value; }
    }

    public Card[] ArrayBiomeSuitWater
    {
        get { return this.arrayBiomeSuitWater; }
        set { this.arrayBiomeSuitWater = value; }
    }

    public Card[] ArrayBiomeSuitDry
    {
        get { return this.arrayBiomeSuitDry; }
        set { this.arrayBiomeSuitDry = value; }
    }
    
    public Card[] ArrayBiomeSuitGrassy
    {
        get { return this.arrayBiomeSuitGrassy; }
        set { this.arrayBiomeSuitGrassy = value; }
    }

    public Card[] ArrayBiomeSuitForest
    {
        get { return this.arrayBiomeSuitForest; }
        set { this.arrayBiomeSuitForest = value; }
    }





}
