using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsController : MonoBehaviour
{
    public GameObject panel;
    System.Action playerCallback;
    public PlayFabLandingMgt pf;


    // Start is called before the first frame update
    void Start()
    {
        pf.GetAvialablePlayers();
        //pf.GetFriends();
        pf.OnGetFriendLB();

        //panel.SetActive(false);
        setPanelToFalse(panel);
    }


  

    // Update is called once per frame
    void Update()
    {
        //pf.seeRegisteredPlayersPanel();

        //pf.GetAvialablePlayers("highscore");
    }
    public void OpenPanel(System.Action callBack = null)
    {
        //panel.SetActive(true);
        togglePanels(panel);
        playerCallback = callBack;
    }

    public void ClosePanel()
    {
        //panel.SetActive(false);
        togglePanels(panel);

        playerCallback();
    }



    public void togglePanels(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = !canvasGrp.interactable;
        canvasGrp.blocksRaycasts = !canvasGrp.blocksRaycasts;
        if (!canvasGrp.interactable)
        {
            canvasGrp.alpha = 0;
        }
        else
        {
            canvasGrp.alpha = 1;
        }
    }

    void setPanelToFalse(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;
        canvasGrp.alpha = 0;

    }
}
