using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    //REAGAN
    public GameObject playfabmgt;

    public GameObject panel;
    System.Action playerCallback;

    //PREFABS TO DISPLAY INDIVIDUAL SKILLS AND CATALOGS
    public GameObject skillsPanelPrefab;
    public GameObject catalogPanelPrefab;


    //DISPLAY LIST OF ITEMS AND SKILLS
    public GameObject itemContent;
    public GameObject skillsContent;

    public GameObject itemScrollView;
    public GameObject skillsScrollView;

    public PlayFabLandingMgt playfablandingmgt;


    public Button switchButton;

    // Start is called before the first frame update
    void Start()
    {
        //switchButton.onClick.AddListener(switchShopPanel);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPanel(System.Action callBack = null)
    {
        panel.SetActive(true);
        //ModifySkillContent("RD");

        //if (itemContent.transform.childCount <= 0)
        //{
        //    playfablandingmgt.GetPlayerInventory();
        //    playfablandingmgt.GetCatalog("Legacy");
        //}

        playerCallback = callBack;
        
    }

    public void ClosePanel()
    {

        panel.SetActive(false);
        playerCallback();
    }

    public void ModifySkillContent(string currencytype)
    {


       

        Skillbox[] children = skillsContent.GetComponentsInChildren<Skillbox>();


        RectTransform contentRectTransform = skillsContent.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x,
            contentRectTransform.sizeDelta.y * skillsContent.transform.childCount * 1.0f);
        for (int i = 0; i < children.Length; i++)
        {
            Debug.Log("ADDED " + children.Length);

            Button childButton = children[i].GetComponentInChildren<Button>();

            Debug.Log("CHILD BUTTON " + childButton);

            Skillbox child = children[i];
            int price = children[i].price;


            childButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy for {price}";
            // Add a listener to the button for a click event
            childButton.onClick.AddListener(
                () => {
                    playfablandingmgt.BuySkill(currencytype, price, "Skills", child);
                }
                );
        }
    }

    public void AddCatalogContent(string text, string price, string catalogName, string itemID, string currencyType)
    {
        // Instantiate a new text element and set its content
        RectTransform newText = Instantiate(catalogPanelPrefab.GetComponent<RectTransform>(), itemContent.transform);
        newText.GetComponentInChildren<TextMeshProUGUI>().text = text;
        RectTransform rectTransform = newText.GetComponent<RectTransform>();

        // Ensure the Content object is big enough to contain all the elements
        RectTransform contentRectTransform = itemContent.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x,
            rectTransform.sizeDelta.y * itemContent.transform.childCount * 1.0f);
        RectTransform[] children = newText.GetComponentsInChildren<RectTransform>();

        Debug.Log($"CRT {itemContent.transform.childCount}");

        for (int x = 0; x < children.Length; x++)
        {
            Button childButton = children[x].GetComponentInChildren<Button>();

            if (childButton != null)
            {
                childButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy for {price}";
                // Add a listener to the button for a click event
                childButton.onClick.AddListener(
                () =>
                {
                    playfablandingmgt.BuyItem(catalogName, itemID, currencyType, int.Parse(price));
                    Destroy(newText.gameObject);
                }
                );
            }
        }

    }

    // Function to clear all children of a GameObject
    void ClearAllChildren(GameObject parent)
    {
        // Iterate through each child and destroy it
        foreach (RectTransform child in parent.GetComponent<RectTransform>())
        {
            Destroy(child.gameObject);
        }
    }


    public void switchShopPanel()
    {
        if (!itemScrollView.activeSelf)
        {
            itemScrollView.SetActive(true);
            skillsScrollView.SetActive(false);

            switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "See Skills";
        }
        else
        {
            itemScrollView.SetActive(false);
            skillsScrollView.SetActive(true);

            switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "See Items";

        }
    }
}
