using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;

public class LeaderboardController : MonoBehaviour
{
    public GameObject panel;
    System.Action playerCallback;

    //REAGAN
    public GameObject playfabmgt;

    // Start is called before the first frame update
    void Start()
    {
        playfabmgt.GetComponent<PlayFabLandingMgt>().OnButtonGetLeaderboard("highscore");
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OpenPanel(System.Action callBack = null)
    {
        panel.SetActive(true);
        playerCallback = callBack;

        //REAGAN
         seeHighScorePanel();
        
        //for (int i = 0; i < playfabmgt.GetComponent<PlayFabLandingMgt>().LeaderboardStr.Count; i++)
        //{
        //    string lbstring = playfabmgt.GetComponent<PlayFabLandingMgt>().LeaderboardStr[i];

        //    for (int x = 0; x < 100; x++)
        //    {
        //        AddLbContent(lbstring);
        //    }
        //}
    }



    public void seeHighScorePanel()
    {
        for (int x = 0; x < 10; x++)
        {
            ClearAllChildren(panelContent);

            playfabmgt.GetComponent<PlayFabLandingMgt>().OnButtonGetLeaderboard("highscore");

            for (int i = 0; i < playfabmgt.GetComponent<PlayFabLandingMgt>().LeaderboardStr.Count; i++)
            {
                string lbstring = playfabmgt.GetComponent<PlayFabLandingMgt>().LeaderboardStr[i];
                AddLbContent(lbstring);
                Debug.Log($"FINISHED {lbstring}");
            }
        }
    }


    public void seeTimePanel()
    {
        for (int x = 0; x < 10; x++)
        {
            ClearAllChildren(panelContent);

            playfabmgt.GetComponent<PlayFabLandingMgt>().OnButtonGetLeaderboard("Time");

            for (int i = 0; i < playfabmgt.GetComponent<PlayFabLandingMgt>().LeaderboardStr.Count; i++)
            {
                string lbstring = playfabmgt.GetComponent<PlayFabLandingMgt>().LeaderboardStr[i];
                AddLbContent(lbstring);
                Debug.Log($"FINISHED {lbstring}");
            }
        }
    }

    //public void OpenHighScorePanel(System.Action callBack = null)
    //{
         
    //    panel.SetActive(true);
    //    //playerCallback = callBack;

    //    //REAGAN
    //    //playfabmgt.GetComponent<PlayFabLandingMgt>().OnButtonGetLeaderboard("Time");
    //    //playfabmgt.GetComponent<PlayFabLandingMgt>().OnButtonGetLeaderboard("HighScore");

       

    //}

    public void ClosePanel()
    {
        panel.SetActive(false);
        playerCallback();
    }

    public Image contentImage;

    //void Start()
    //{
    //    // Add images dynamically (you can do this based on your needs)
    //    AddImage("Image1");
    //    AddImage("Image2");
    //    AddImage("Image3");
    //    AddImage("Image4");
    //    AddImage("Image5");
    //}

    void AddImage(string imageName)
    {
        // Instantiate a new image element and set its sprite
        Image newImage = Instantiate(contentImage, transform);
        newImage.sprite = Resources.Load<Sprite>(imageName); // Load your sprite dynamically

        // Adjust the RectTransform of the new image element
        RectTransform rectTransform = newImage.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -rectTransform.sizeDelta.y * transform.childCount);

        // Ensure the Content object is big enough to contain all the elements
        RectTransform contentRectTransform = GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, rectTransform.sizeDelta.y * transform.childCount);
    }



    void ClearAllChildren(GameObject parent)
    {
        // Remove all children from the parent's transform
        parent.transform.DetachChildren();


        // Iterate through each child and destroy it
        foreach (RectTransform child in parent.GetComponent<RectTransform>())
        {
            Destroy(child.gameObject);
        }


    }


    public GameObject panelContent;
    public TextMeshProUGUI contentText;

    void AddLbContent(string text)
    {

        TextMeshProUGUI newText = Instantiate(contentText, panelContent.transform);
        newText.text = text;
        RectTransform rectTransform = newText.GetComponent<RectTransform>();
        RectTransform contentRectTransform = panelContent.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x,
            rectTransform.sizeDelta.y * panelContent.transform.childCount);
        //rectTransform.position = new Vector2(0, rectTransform.position.y);

        Debug.Log("SD " + newText.text);

    }

}
