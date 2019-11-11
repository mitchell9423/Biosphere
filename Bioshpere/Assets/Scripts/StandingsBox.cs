using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingsBox : GameBase
{
    RectTransform rectTransform;
    Vector2 closedPos = new Vector2();
    Vector2 openPos = new Vector2();
    bool isOpen = false;
    float t;
    float speed = 2.0f;

    private void Start()
    {
        t = speed * Time.deltaTime;
        rectTransform = gameObject.GetComponent<RectTransform>();
        closedPos = rectTransform.position;
        openPos = new Vector2(closedPos.x, closedPos.y + 100.0f);
    }

    private void Update()
    {
        ViewStandingsBox();
    }


    // Switches between open and close states.
    public void ToggleStandingsBox()
    {
        isOpen = !isOpen;
    }

    private void ViewStandingsBox()
    {
        if (!isOpen)
        {
            rectTransform.position = Vector2.Lerp(closedPos, openPos, t);
        }
        else
        {
            rectTransform.position = Vector2.Lerp(openPos, closedPos, t);
        }
    }
}
