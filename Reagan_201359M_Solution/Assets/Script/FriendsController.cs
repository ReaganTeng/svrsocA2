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
        pf.GetAvialablePlayers("highscore");
        pf.GetFriends();
        pf.OnGetFriendLB();

        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //pf.seeRegisteredPlayersPanel();

        //pf.GetAvialablePlayers("highscore");
    }
    public void OpenPanel(System.Action callBack = null)
    {
        panel.SetActive(true);
        playerCallback = callBack;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        playerCallback();
    }
}
