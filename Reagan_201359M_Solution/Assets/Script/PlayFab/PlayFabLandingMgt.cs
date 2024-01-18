using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

using System.Collections.Generic;

using TMPro;

using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Diagnostics;
using static UnityEditor.Progress;
using UnityEngine.TextCore.Text;

public class PlayFabLandingMgt : MonoBehaviour
{
    //NAME TO DISPLAY IN LEADERBOARD
    string nametodisplay = "NIL";

    public TextMeshProUGUI lbtext;
    public TextMeshProUGUI shopText;

    public TextMeshProUGUI moneyText;


    public ShopController shopcontroller;


    TextMeshProUGUI nullText = null;

    public Skillbox[] skillboxes;

    public List<string> LeaderboardStr;


    public List<string> FriendListStr;


    public string loginscene;

    List<ItemInstance> ItemsInPlayer = new List<ItemInstance>();
    //List<CatalogItem> items = new List<CatalogItem>();



    public TextMeshProUGUI txtFrdList, friendLeaderboard, friendName;
    public GameObject friendContent, friendLBContent;

    public TMP_InputField addFriendInputField, removeFriendInputField;
    List<FriendInfo> _friends = null;
    public GameObject panelContent;

    public GameObject playerdisplayPrefab;
    enum FriendIdType
    { 
        PlayFabId,
        Username,
        Email,
        DisplayName
    }

    void Awake()
    {
    }


    void DisplayFriends(List<FriendInfo> friends)
    {
        txtFrdList.text = "";
        friends.ForEach(f =>
        {
            Debug.Log($"{f.FriendPlayFabId},{f.TitleDisplayName}");
            //txtFrdList.text 
            //+= $"{f.FriendPlayFabId},{f.TitleDisplayName}\n";

            //TextMeshProUGUI newText = Instantiate(friendName, friendContent.transform);
            //newText.text = $"{f.FriendPlayFabId},{f.TitleDisplayName}";

            //if (f.Profile != null )
            //{
            //    Debug.Log($"{f.FriendPlayFabId}/{f.Profile.DisplayName}");
            //}
        });
    }


    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(
            new GetFriendsListRequest { }
            , result =>
            {
                _friends = result.Friends;
                DisplayFriends(_friends);
            }, OnError);
    }


    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch(idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        PlayFabClientAPI.AddFriend(
            request,
            result =>
            {
                Debug.Log("ADDED FRIEND");
            }, OnError
            );
    }

    public void OnAddFriend()
    {
        AddFriend(FriendIdType.DisplayName, addFriendInputField.text);
    }

    public void OnAddFriend(string dm)
    {
        AddFriend(FriendIdType.DisplayName, addFriendInputField.text);
    }



    void RemoveFriend(FriendInfo friendInfo)
    {
        PlayFabClientAPI.RemoveFriend(
            new RemoveFriendRequest
            {
                FriendPlayFabId = friendInfo.FriendPlayFabId,
            }, result =>
            {
                _friends.Remove(friendInfo);
            }, OnError
            );
    }
    void RemoveFriend(string pfid)
    {
        var req = new RemoveFriendRequest { 
            FriendPlayFabId = pfid 
        };
        PlayFabClientAPI.RemoveFriend(req
            , result =>
            {
                Debug.Log("UNFREIND");
            }, OnError
            );

    }
    public void OnRemoveFriend()
    {
        RemoveFriend(removeFriendInputField.text);
    }


    public void OnGetFriendLB()
    {
        PlayFabClientAPI.GetFriendLeaderboard(
            new GetFriendLeaderboardRequest
            {
                StatisticName = "highscore",
                MaxResultsCount = 10,
            },
            r =>
            {
                friendLeaderboard.text = "Friends LB\n";
                foreach (var item in r.Leaderboard)
                {
                    string onerow = $"{item.Position}" +
                    $"/{item.DisplayName}/{item.StatValue}";
                    
                    TextMeshProUGUI newText = Instantiate(friendName, friendLBContent.transform);
                    newText.text = onerow;
                }
            }, OnError
            );
    }

    string pFabId;

    public void GetAvialablePlayers(string statisticname)
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = statisticname,
            StartPosition = 0,
            MaxResultsCount = 10,
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnGetPlayersSucceeded, OnError);


        


        //USE LEADERBOARD


        //var request = new ListUsersCharactersRequest
        //{
        //    // The PlayFab ID of the player you want to get characters for
        //    PlayFabId = pFabId,
        //};
        ////PlayFabClientAPI.get
        ////PlayFabClientAPI.GetPlay
        //PlayFabClientAPI.GetAllUsersCharacters(request, 
        //    OnGetAllUsersCharacters, 
        //    OnError);
        ////Debug.Log("YYY");

    }


    void OnGetPlayersSucceeded(GetLeaderboardResult r)
    {
        // Use a counter to keep track of how many callbacks have completed
        int completedCallbacks = 0;

        RectTransform rectTransform;
        int count = 0;
        for (int item = 0; item < r.Leaderboard.Count; item++)
        {
            var req = new GetAccountInfoRequest
            {
                PlayFabId = r.Leaderboard[item].PlayFabId,
            };

            // Use a lambda function to capture the item variable
            //PlayFabClientAPI.GetAccountInfo(req, (result) =>
            //{
            //    completedCallbacks++;
            //}, OnError);

         
            GameObject newText = Instantiate(playerdisplayPrefab, panelContent.transform);
            PlayerDisplayUI pdUI = newText.GetComponent<PlayerDisplayUI>();
            pdUI.displayname.text = r.Leaderboard[item].DisplayName;

            //newText.text = text;
            rectTransform = newText.GetComponent<RectTransform>();

            count++;

            //if (!FriendListStr.Any() ||
            //    item > FriendListStr.Count - 1)
            //{
            //    FriendListStr.Add(onerow);


            //}
            //else
            //{
            //    //LeaderboardStr[item] = onerow;
            //    FriendListStr[item] = onerow;
            //}
            //Debug.Log(LeaderboardStr.Count);
        }

        RectTransform contentRectTransform = panelContent.GetComponent<RectTransform>();

        foreach (RectTransform c in panelContent.GetComponentsInChildren<RectTransform>())
        {
            if (c.GetComponent<PlayerDisplayUI>() != null)
            {
                //Debug.Log($"{count}");
                //Debug.Log($"{r.Leaderboard.Count}");

                //PlayerDisplayUI p = c.GetComponent<PlayerDisplayUI>();
                //c.GetComponent<PlayerDisplayUI>().AddButton.onClick.AddListener(()
                //    => {
                //        AddFriend(FriendIdType.DisplayName, $"{r.Leaderboard[item].DisplayName}");
                //    }
                //    );
                //c.GetComponent<PlayerDisplayUI>().RemoveButton.onClick.AddListener(()
                //   => {

                //       RemoveFriend(p.displayname.text);
                //   }
                //   );
            }

        }
        //contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x,
        //           rectTransform.sizeDelta.y * count);
    }


    public void seeRegisteredPlayersPanel()
    {
        for (int x = 0; x < 10; x++)
        {
            //ClearAllChildren(panelContent);

            GetAvialablePlayers("highscore");
           //OnButtonGetLeaderboard("highscore");

            for (int i = 0; i < FriendListStr.Count; i++)
            {
                string lbstring = FriendListStr[i];
                //AddLbContent(lbstring);
                Debug.Log($"FRIEND {lbstring}");
            }
        }
    }

    void GiveItemTo(string secondPlayerId, string myItemInstanceId)
    {
        PlayFabClientAPI.OpenTrade(
            new OpenTradeRequest
            {
                AllowedPlayerIds = new List<string> { secondPlayerId },
                OfferedInventoryInstanceIds = new List<string> { myItemInstanceId }
            }
            , OnGiveItemSucceed, OnError
            );
    }


    void OnGiveItemSucceed(OpenTradeResponse result)
    {
        Debug.Log($"ITEM GIVEN FROM {result}");
    }


    void ExamineTrade(string firstPlayFabId, string tradeId)
    {
        PlayFabClientAPI.GetTradeStatus(
            new GetTradeStatusRequest
            {
                OfferingPlayerId = firstPlayFabId,
                TradeId = tradeId
            }, OnExamineTradeSuccess, OnError);
    }


    void OnExamineTradeSuccess(GetTradeStatusResponse result)
    {
        // Process user data here
        

            Debug.Log($"TRADE {result} SUCCESS");
        
    }



    void OnGetUserDataSuccess(GetUserDataResult result)
    {
        // Process user data here
        foreach (var entry in result.Data)
        {
            string key = entry.Key;
            string value = entry.Value.Value;

            Debug.Log($"User data - Key: {key}, Value: {value}");
        }
    }

    void OnGetAllUsersCharacters(ListUsersCharactersResult result)
    {
        // Handle the result, which contains a list of characters for the specified player
        if (result.Characters != null 
            && result.Characters.Count > 0
        )
        {
            foreach (var character in result.Characters)
            {
                Debug.Log($"Character ID: {character.CharacterId}, Character Name: {character.CharacterName}\n");
                Debug.Log("CCC");
            }
        }
        else
        {
            Debug.Log("No characters found.");
        }

        //Debug.Log($"RESULT: {result.Characters[0].CharacterId}\n");

        // Handle the result, which contains a list of characters for the specified player
        //foreach (var character in result.Characters)
        //{
        //    Debug.Log($"Character ID: {character.CharacterId}, Character Name: {character.CharacterName}\n");
        //    Debug.Log("CCC");
        //}
    }



    private void Start()
    {

        // Ensure PlayFabUserMgt has been instantiated
        if (PlayFabUserMgt.Instance != null)
        {
            // Access pFabId from PlayFabUserMgt
            pFabId = PlayFabUserMgt.Instance.pFabId;
            Debug.Log($"PlayFab ID from another scene: {pFabId}");
        }
        //InitializePlayFabEvents();

        //PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);

        GetPlayerInventory();
        GetCatalog("Legacy");

        GetVirtualCurrencies("RD");


        LoadJSON();

        //TOOK FROM SHOPCONTROLLER
        shopcontroller.ModifySkillContent("RD");
        //playfablandingmgt.GetCatalog("Legacy");
        //playfablandingmgt.GetVirtualCurrencies("RD");
        shopcontroller.switchShopPanel();



        shopcontroller.panel.SetActive(false);
    }

    private void Update()
    {
        
    }
    public void OnError(PlayFabError e)
    {
        //UpdateMsg(ErrMsg, "Error" + e.GenerateErrorReport());
        Debug.Log("Error" + e.GenerateErrorReport());
    }
    public void UpdateMsg(ref TextMeshProUGUI text, string msg)
    { //to display in console and messagebox
        Debug.Log("MESSAGE " + msg);

        if (text != null)
        {
            text.text = msg;
        }
    }



    public void UpdateLBMsg(ref TextMeshProUGUI text, string msg)
    { //to display in console and messagebox
        Debug.Log("MESSAGE " + msg);

        if (text != null)
        {
            text.text = msg;
            //AddContent(text.text);
        }
    }

    bool hasNoletters(string input)
    {
        // Assume the string has no letters until we find one
        bool hasNoLetters = true;

        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c))
            {
                hasNoLetters = false;
                break; // Exit the loop early since we found a letter
            }
        }

        if (hasNoLetters)
        {
            Debug.Log("The string has no letters.");
        }
        else
        {
            Debug.Log("The string contains at least one letter.");
        }

        return hasNoLetters;
    }






    ///LEADERBOARD
    public void OnButtonGetLeaderboard(string statisticname)
    {
        //LeaderboardStr.Clear();

        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = statisticname,
            StartPosition = 0,
            MaxResultsCount = 10,
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);

        //UpdateMsg(ref lbtext, LeaderboardStr);
        //UpdateLBMsg(ref contentText, LeaderboardStr);
        //Debug.Log("LB STRING " + LeaderboardStr);
    }

    //ONLEADERBOARDGET NEW
    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        // Use a counter to keep track of how many callbacks have completed
        int completedCallbacks = 0;

        for (int item = 0; item < r.Leaderboard.Count; item++)
        {
            var req = new GetAccountInfoRequest
            {
                PlayFabId = r.Leaderboard[item].PlayFabId,
            };

            // Use a lambda function to capture the item variable
            PlayFabClientAPI.GetAccountInfo(req, (result) =>
            {
                completedCallbacks++;                
            }, OnError);

            string onerow = "ROW";
            onerow = "#" + r.Leaderboard[item].Position + " " + r.Leaderboard[item].DisplayName + " " + r.Leaderboard[item].StatValue;
            Debug.Log("LB ADDED TO ROW");
            //TURN LEADERBOARDSTR INTO A LIST ALSO

            if (!LeaderboardStr.Any() ||
                item > LeaderboardStr.Count - 1)
            {
                LeaderboardStr.Add(onerow);
            }
            else
            {
                LeaderboardStr[item] = onerow;
            }
            //Debug.Log(LeaderboardStr.Count);
        }
    }
    ///

    //bool namedecided = false;
    //void GetLBName(GetAccountInfoResult r)
    //{
    //    nametodisplay = "NIL";
    //    if (r != null
    //        && r.AccountInfo.Username != null)
    //    {
    //        nametodisplay = r.AccountInfo.Username;
    //        Debug.Log("Username is " + nametodisplay + " " + r.AccountInfo.PlayFabId);
    //    }
    //}


    ///ECONOMY
    public void GetVirtualCurrencies(string currencytype)
    {
        //DISPLAY AMOUNT OF MONEY
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            r => {
                //RM, RD
                int coins = r.VirtualCurrency[currencytype];
                UpdateMsg(ref shopText, "Coins: " + coins);
                moneyText.text = $"${coins}";
            }
            , OnError);
    }
    public void GetCatalog(string catalogname)
    {
        //items.Clear();
        var catreq = new GetCatalogItemsRequest
        {
            CatalogVersion = catalogname,
        };
        PlayFabClientAPI.GetCatalogItems(catreq, r =>
        {
            
            //UpdateMsg(ref nullText, "Catalog Items");
            //foreach (CatalogItem item in items)
            foreach (CatalogItem item in r.Catalog)
            {
                Debug.Log("FIND ITEM");
                bool itemfound = false;
                //UpdateMsg(ref nullText, 
                //    item.DisplayName + ", " + item.VirtualCurrencyPrices["RD"]);
               
                foreach (ItemInstance it in ItemsInPlayer)
                {
                    if (it.ItemId == item.ItemId)
                    {
                        Debug.Log("ALREADY EXISTED IN PLAYER'S INVENTORY");
                        itemfound = true;
                        break;
                    }
                }
                
                //IF ITEM NOT FOUND, THEN ADD IN INVENTORY
                if (!itemfound)
                {
                    Debug.Log("ITEM NOT FOUND IN PLAYER");
                    shopcontroller.AddCatalogContent($"{item.DisplayName}\n{item.Description}",
                       $"{item.VirtualCurrencyPrices["RD"]}"
                       , "Legacy", item.ItemId, "RD");
                }
            }


        }, OnError);


       

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


    public void BuyItem(string catalogname, string itemid,
        string currencytype, int price)
    {
        var buyreq = new PurchaseItemRequest { 
            CatalogVersion = catalogname,
            ItemId = itemid,
            VirtualCurrency = currencytype, 
            Price = price
        };
        PlayFabClientAPI.PurchaseItem(buyreq,
            r =>
            {
                UpdateMsg(ref nullText, "BOUGHT!");
                GetVirtualCurrencies("RD");
            }, OnError);
        //REFRESH THS SHOP ITEMS
        //ClearAllChildren(shopcontroller.itemContent);
        //GetCatalog("Legacy");

    }

    public void BuySkill(string currencytype, int price, string data2send, Skillbox sb)
    {
        if (int.Parse(sb.skillleveltext.text) < 10)
        {
            //Debug.Log("BOUGHT");

            var buyreq = new SubtractUserVirtualCurrencyRequest
            {
                VirtualCurrency = currencytype,
                Amount = price,
            };
            PlayFabClientAPI.SubtractUserVirtualCurrency(buyreq,
                r =>
                {
                    UpdateMsg(ref nullText, "BOUGHT!");
                    GetVirtualCurrencies("RD");
                }, OnError);

            sb.SetUI(new Skill(sb.skillname.text, int.Parse(sb.skillleveltext.text) + 1));
            SendJSON(data2send);
            //LoadJSON();
        }
    }
    ///


    ///INVENTORY
    public void GetPlayerInventory()
    {

        //CHECK USER INVENTORY IF ITEM IS ALREADY IN THE INVENTORY
        ItemsInPlayer.Clear(); // Clear the list before populating it again

        //var Inv = new UserInve();

        var UserInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(UserInv, 
            r =>
        {
            List<ItemInstance> ii = r.Inventory;
            foreach (ItemInstance item in r.Inventory)
            {
                Debug.Log("PLAYER HAS " + item.ItemId);
                ItemsInPlayer.Add(item);
            }
        }, OnError);



       
    }
    ///


    ///JSON
    public void SendJSON(string datatosend)
    {
        List<Skill> skilllist = new List<Skill>();
        foreach (var item in skillboxes)
        {
            skilllist.Add(item.ReturnClass());
        }

        string stringListAsJson = JsonUtility.ToJson(new JSListWrapper<Skill>(skilllist));
        Debug.Log("JSON data prepared: " + stringListAsJson);
        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {datatosend, stringListAsJson }
            }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("Data sent success!"), OnError);
    }
    public void LoadJSON()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataReceived, OnError);
    }
    void OnJSONDataReceived(GetUserDataResult r)
    {
        //for (int x = 0; x < skillboxes.Length; x++)
        //{
            //Debug.Log("received JSON data");
            if (r.Data != null
                && r.Data.ContainsKey("Skills")
                //&& r.Data.ContainsKey(skillboxes[x].name)
                )
            {
                //Debug.Log(r.Data[skillboxes[x].name].Value);
                //Debug.Log(r.Data["Skills"].Value);
                //GET FROM JSON
                JSListWrapper<Skill> jlw = JsonUtility.FromJson<JSListWrapper<Skill>>(r.Data["Skills"].Value);
                //Debug.Log($"JLW {jlw.list[1].name}");
                for (int i = 0; i < skillboxes.Length; i++)
                {
                    skillboxes[i].SetUI(jlw.list[i]);
                }
            }
            else
            {
                for (int i = 0; i < skillboxes.Length; i++)
                {
                    skillboxes[i].SetUI(new Skill(skillboxes[i].skillname.text, 1));
                    //Debug.Log(skillboxes[i].);
                }

                SendJSON("Skills");
            }
        //}

        
    }

    public void BacktoMainScene()
    {
        SceneManager.LoadScene(loginscene);


        //var loginRequest = new LoginWithPlayFabRequest
        //{
        //    Username = userName.text,
        //    Password = userPassword.text,

        //    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
        //    {
        //        GetPlayerProfile = true
        //    }
        //};
        //PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess, OnError);
    }


    void OnLoginSuccess(LoginResult r)
    {
        //UpdateMsg(ref nullText, "Login Success!");

        ////Debug.Log("LoginResult: " + r.PlayFabId);

        //var req = new GetAccountInfoRequest
        //{
        //    PlayFabId = r.PlayFabId
        //};

        //PlayFabClientAPI.GetAccountInfo(req, GetUserName, OnError);

        
    }


    //LOGIN WITH EMAIL
    //void OnButtonLoginEmail()
    //{
    //    var loginRequest = new LoginWithEmailAddressRequest
    //    {
    //        Email = userEmail.text,
    //        Password = userPassword.text,

    //        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
    //        {
    //            GetPlayerProfile = true
    //        }
    //    };

    //    PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
    //}




    //FOR TESTING PURPOSES
    void InstantLogin()
    {
        //userEmail.text = "yuzhe100903@gmail.com";
        //userName.text = "reagan";

        //userPassword.text = "1234567890";

        //OnButtonLogin();

        //SceneManager.LoadScene(ShopScene);
        //SceneManager.LoadScene(GameScene);
    }
    ///





}
