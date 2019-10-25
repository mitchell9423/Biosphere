using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

	static bool teamPageExtended = false;
	[SerializeField] RectTransform TeamInfoPage;
    bool showBtns = true;
	bool showTeamInfo;
	Vector3 closedPos;

	// Start is called before the first frame update
	void Start()
    {
		if (gameObject.CompareTag("TeamInfoTab"))
		{
			closedPos = TeamInfoPage.localPosition;
		}
	}


    // Update is called once per frame
    void LateUpdate()
	{
		if (gameObject.CompareTag("TeamInfoTab"))
			TranslateTeamInfoPage();
	}


    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }


	public void ViewTeamData()
	{
		print("hi");
		if(!teamPageExtended)
		{
			teamPageExtended = true;
			showTeamInfo = true;
		}
		else if(showTeamInfo)
		{
			teamPageExtended = false;
			showTeamInfo = false;
		}
	}


	private void TranslateTeamInfoPage()
	{
		float x = TeamInfoPage.localPosition.x;
		if (showTeamInfo)
		{
			TeamInfoPage.localPosition = Vector3.MoveTowards(TeamInfoPage.localPosition, Vector3.zero, 15.0f);
		}
		else
		{
			TeamInfoPage.localPosition = Vector3.MoveTowards(TeamInfoPage.localPosition, closedPos, 15.0f);
		}
	}
}
