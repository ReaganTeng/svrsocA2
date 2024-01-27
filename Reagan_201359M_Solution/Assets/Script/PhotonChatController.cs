using UnityEngine;
using System;
using Photon.Chat;
using Photon.Pun;
using Photon.Chat.Demo;

using ExitGames.Client.Photon;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
//using UnityEditor.PackageManager;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;


public class CustomDropdownOptionData
{
    public string Text { get; set; }
    public Color Color { get; set; }

    public CustomDropdownOptionData(string text, Color color)
    {
        Text = text;
        Color = color;
    }
}


public class PhotonChatController : MonoBehaviour, IChatClientListener
{
  

    string everyone = "EVERYONE";
    string friends = "FRIENDS";

    public TMP_Dropdown namelist;

   // [SerializeField] private string nickName;
    private ChatClient chatClient;

    string currentchat;

    public TMP_InputField chatInputfield;

    int listoffset;

    public static Action<string, string> OnRoomInvite = delegate { };
    public static Action<ChatClient> OnChatConnected = delegate { };
    public static Action<PhotonStatus> OnStatusUpdated = delegate { };

    [HideInInspector]
    public string username;
    bool isconnected;

    public GameObject chatdisplay;

    //private Dictionary<string, string> photonIdToUsernameMap = new Dictionary<string, string>();

    List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
    
    List<string> PlayerDisplaynameList = new List<string>();
  
    public void OnError(PlayFabError e)
    {
        //UpdateMsg(ErrMsg, "Error" + e.GenerateErrorReport());
        Debug.Log("Error" + e.GenerateErrorReport());
    }


    

    public void TypeChatOnValueChange()
    {
        currentchat = chatInputfield.text;
        Debug.Log("CHAT");
    }

    void ChatList()
    {
        //REFRESH EVERYTHING
        PlayerDisplaynameList.Clear();
        options.Clear();
        namelist.AddOptions(options);
        //

        options.Add(new TMP_Dropdown.OptionData(everyone));
        options.Add(new TMP_Dropdown.OptionData(friends));
        listoffset = options.Count;
        string playersownId = PlayerPrefs.GetString("PLAYFABID");

        PlayFabClientAPI.GetLeaderboard(
           new GetLeaderboardRequest
           {
               StatisticName = "highscore",
               MaxResultsCount = 10,
           },
            r =>
            {
                for (int item = 0; item < r.Leaderboard.Count; item++)
                {
                    string NAME = r.Leaderboard[item].DisplayName;

                    //IF IT IS NOT PLAYER ITSELF
                    if (r.Leaderboard[item].PlayFabId == playersownId)
                    {
                        //CONNECT PLAYER ITSELF TO PHOTON CHAT
                        if (!PlayerPrefs.HasKey("NAME")
                          ||
                          (PlayerPrefs.HasKey("NAME") &&
                          PlayerPrefs.GetString("NAME") != r.Leaderboard[item].DisplayName))
                        {
                            //Debug.Log($"USERNAME FOUND {r.Leaderboard[item].DisplayName}");
                            PlayerPrefs.SetString("NAME", r.Leaderboard[item].DisplayName);
                        }
                        username = PlayerPrefs.GetString("NAME");
                        ChatConnectOnClick();
                        //
                    }
                    else
                    //ADD AS PART OF THE OPTIONS
                    {
                        PlayerDisplaynameList.Add(NAME);



                        options.Add(new TMP_Dropdown.OptionData(NAME));
                        

                       // Debug.Log($"LOOPED {NAME}");
                    }
                    //
                }

                namelist.AddOptions(options);
                //Debug.Log("LOOP OPTIONS ADDED");

            }
       , OnError);
        //Debug.Log("CHATLIST");

    }



    void SendToFriends()
    {
        string playersownId = PlayerPrefs.GetString("PLAYFABID");
        PlayFabClientAPI.GetFriendLeaderboard(
            new GetFriendLeaderboardRequest
            {
                StatisticName = "highscore",
                MaxResultsCount = 10,
            },
            r =>
            {
                for (int item = 0; item < r.Leaderboard.Count; item++)
                {
                    if (r.Leaderboard[item].PlayFabId != playersownId)
                    {
                        SendPrivateMessage(r.Leaderboard[item].DisplayName);
                    }
                }
            }, OnError
            );
        
    }


    IEnumerator ListFriendsCoroutine()
    {
        string playersownId = PlayerPrefs.GetString("PLAYFABID");

        PlayFabClientAPI.GetFriendLeaderboard(
            new GetFriendLeaderboardRequest
            {
                StatisticName = "highscore",
                MaxResultsCount = 10,
            },
            friendResult =>
            {
                foreach (var friendEntry in friendResult.Leaderboard)
                {
                    if (friendEntry.PlayFabId != playersownId)
                    {
                        StartCoroutine(GetLeaderboardForFriend(friendEntry.PlayFabId, friendEntry.DisplayName));
                    }
                }
            },
            OnError);

        yield return new WaitForSeconds(1f); // Adjust delay as needed
    }

    IEnumerator GetLeaderboardForFriend(string friendPlayFabId, string friendresultDM)
    {
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = "highscore",
                MaxResultsCount = 10,
            },
            lbResult =>
            {
                for (int x = 0; x < lbResult.Leaderboard.Count; x++)
                {
                    if (lbResult.Leaderboard[x].PlayFabId == friendPlayFabId)
                    {
                        Debug.Log($"FOUND FRIEND {lbResult.Leaderboard[x].DisplayName} {friendresultDM} {friendPlayFabId}");
                        int idx = x +listoffset;

                        namelist.options[idx].text = $"{namelist.options[idx].text} (FRIEND)";
                        // Update UI or perform other actions with the found friend
                    }
                }
            },
            OnError);

        yield return null; // Ensure the coroutine has time to complete before moving on
    }




    //recepientname
    string privateReceiver;
    void Awake()
    {

        chatInputfield.text = "";
        isconnected = false;

        ChatList();
        //ListFriends();
        StartCoroutine(ListFriendsCoroutine());


        PlayerDisplayUI.OnInviteFriend += HandleFriendInvite;
        //ConnectoToPhotonChat();

    }


   
    private void OnDestroy()
    {
        PlayerDisplayUI.OnInviteFriend -= HandleFriendInvite;
    }

  

    public void onPrivateReceiverchanged()
    {
        int selectedIndex = namelist.value;

        if (namelist.value >= listoffset)
        {
            privateReceiver = PlayerDisplaynameList[selectedIndex - listoffset ];
        }
        else
        {
            privateReceiver = namelist.options[selectedIndex].text;
        }

        Debug.Log($"SELECTED OPTION IS {privateReceiver}");
    }


    //FOR JOINING CHAT
    public void ChatConnectOnClick()
    {
        isconnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new AuthenticationValues(username));
        Debug.Log($"Connecting {username} to Photon Chat");
    }


    private void Update()
    {
        if (isconnected)
        {
            chatClient.Service();
        }

        
    }


    public void SendMessage()
    {
        if (hasNoletters(currentchat))
        {
            return;
        }


        if (privateReceiver == everyone)
        {
            SendPublicMessage();
        }
        else if (privateReceiver == friends)
        {
            SendToFriends();
        }
        else
        {
            SendPrivateMessage();

        }


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
        //Debug.Log("DEBUGGGG");
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



    private void ConnectoToPhotonChat()
    {
        Debug.Log("Connecting to Photon Chat");
        chatClient.AuthValues = new AuthenticationValues("Schindler");
        ChatAppSettings chatSettings 
            = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);

       

    }

   

    public void HandleFriendInvite(string recipient)
    {
        if (!PhotonNetwork.InRoom) return;
        chatClient.SendPrivateMessage(recipient, PhotonNetwork.CurrentRoom.Name);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Photon Chat DebugReturn: {message}");
    }

    public void OnDisconnected()
    {
        Debug.Log("You have disconnected from the Photon Chat");
       chatClient.SetOnlineStatus(ChatUserStatus.Offline);


    }
    

   

    public void OnConnected()
    {
        //Debug.Log($"{chatClient.AppId} have connected to the Photon Chat");
        isconnected = true;
        //SECOND VERSION
        chatClient.Subscribe(new string[] { "RegionChannel" });
        //FIRST VERSION
        OnChatConnected?.Invoke(chatClient);
        chatClient.SetOnlineStatus(ChatUserStatus.Online);

        OnUserSubscribed("RegionChannel", username);

        //for (int i = 0; i < 100; i++)
        //{
        //    chatClient.PublishMessage("RegionChannel", "HELLO");
        //}
    }

    void SendPrivateMessage()
    {
        string privateChannel = privateReceiver; // Use the Photon ID as the channel name
        //currentchat = $"HELLO {privateReceiver}";
        chatClient.SendPrivateMessage(privateChannel, currentchat);
        currentchat = "";       
    }


    void SendPrivateMessage(string receiver)
    {
        string privateChannel = receiver; // Use the Photon ID as the channel name
        //currentchat = $"HELLO {privateReceiver}";
        chatClient.SendPrivateMessage(privateChannel, currentchat);
        currentchat = "";
    }


    void SendPublicMessage()
    {
        chatClient.PublishMessage("RegionChannel", currentchat);   
        //CLEAR CHAT FIELDS HERE
        currentchat = "";
    }


    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Photon Chat OnChatStateChange: {state}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log($"Photon Chat OnGetMessages {channelName}");
                
        for (int i = 0; i < senders.Length; i++)
        {
            //Debug.Log($"{senders[i]} messaged: {messages[i]}");
            string msg = $"{senders[i]}: {messages[i]}\n\n";
            //DISPLAY INSIDE MESSAGE BOX;
            //chatdisplay.text += msg;
            CreateMessage(msg);
        }
    }

    public void ClearMessageBox()
    {
        foreach(TextMeshProUGUI t in chatdisplay.GetComponentsInChildren<TextMeshProUGUI>())
        {
            Destroy(t);
        }
    }


    void CreateMessage(string msg)
    {
        // Create a new GameObject
        GameObject textObject = new GameObject("DynamicText");
        // Attach TextMeshProUGUI component to the GameObject
        TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
        // Set the text
        textComponent.text = msg;
        
        textComponent.fontSize = Screen.height * .05f;
        // Set the parent to your chatdisplay or another desired parent
        textObject.transform.SetParent(chatdisplay.transform, false);
    }

    // Helper method to get the username from a Photon ID
    //private string GetUsernameFromPhotonId(string photonId)
    //{
    //    if (photonIdToUsernameMap.TryGetValue(photonId, out string username))
    //    {
    //        Debug.Log($"US NAME IS {username}");
    //        return username;
    //    }

    //    // If the username is not found, return the Photon ID itself
    //    return photonId;
    //}

    public void OnPrivateMessage(string sender, object message, string channelName)
    {


        //string senderUsername = GetUsernameFromPhotonId(sender);

        string msg = $"{sender}: {message}\n\n";
        //chatdisplay.text += msg;

        CreateMessage(msg);


        //if (!string.IsNullOrEmpty(message.ToString()))
        //{
        //    // Channel Name format [Sender : Recipient]
        //    string[] splitNames = channelName.Split(new char[] { ':' });
        //    string senderName = splitNames[0];

        //    if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
        //    {
        //        Debug.Log($"{sender} messaged: {message}");
        //        OnRoomInvite?.Invoke(sender, message.ToString());
        //    }
        //}
    }




    public void OnSubscribed(string[] channels, bool[] results)
    {

        //for (int i = 0; i < channels.Length; i++)
        //{
        //    Debug.Log($"{channels[i]}");
        //}

        //string party_ChannelName = "RegionChannel";



        // Accessing chatClient within the class
        for (int i = 0; i < channels.Length; i++)
        { 
            ChatChannel partyChannel;
            if (chatClient.TryGetChannel(channels[i], out partyChannel))
            {
                if (partyChannel.PublishSubscribers)
                {
                    Debug.Log($"{partyChannel.Name} published");
                }
                else
                {
                    Debug.LogError($"PublishSubscribers was not set during channel {partyChannel.Name} creation.");
                }
            }
        }

    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log($"Photon Chat OnUnsubscribed");
        /*for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log($"{channels[i]}");
        }*/


    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"Photon Chat OnStatusUpdate: {user} changed to {status}: {message}");
        PhotonStatus newStatus = new PhotonStatus(user, status, (string)message);
        Debug.Log($"Status Update for {user} and its now {status}.");
        OnStatusUpdated?.Invoke(newStatus);
    }



    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"Photon Chat OnUserSubscribed: {channel} {user}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"Photon Chat OnUserUnsubscribed: {channel} {user}");
    }


  

}
