using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


// the purpose of this class is to have all of its decendants contain a variable type reference to GameManager type
public abstract class GameBase : MonoBehaviour
{
    protected GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public virtual GameManager GameManager
    {
        get { return this.gameManager; }
        set { this.gameManager = value; }
    }
}
