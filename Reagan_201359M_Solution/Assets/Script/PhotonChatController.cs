using UnityEngine;
using System;
using Photon.Chat;
using Photon.Pun;

using ExitGames.Client.Photon;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
//using UnityEditor.PackageManager;
using Photon.Chat.Demo;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PhotonChatController : MonoBehaviour, IChatClientListener
{

   // [SerializeField] private string nickName;
    private ChatClient chatClient;

    

    public static Action<string, string> OnRoomInvite = delegate { };
    public static Action<ChatClient> OnChatConnected = delegate { };
    public static Action<PhotonStatus> OnStatusUpdated = delegate { };

    [HideInInspector]
    public string username;
    string currentchat;
    bool isconnected;

    public TextMeshProUGUI chatdisplay;

    //private Dictionary<string, string> photonIdToUsernameMap = new Dictionary<string, string>();


    #region Unity Methods

    public void OnError(PlayFabError e)
    {
        //UpdateMsg(ErrMsg, "Error" + e.GenerateErrorReport());
        Debug.Log("Error" + e.GenerateErrorReport());
    }


    void CorrectNamePref()
    {
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
                if (r.Leaderboard[item].PlayFabId == playersownId)
                {
                    if (!PlayerPrefs.HasKey("NAME")
                      ||
                      (PlayerPrefs.HasKey("NAME") &&
                      PlayerPrefs.GetString("NAME") != r.Leaderboard[item].DisplayName))
                    {
                        Debug.Log($"USERNAME FOUND {r.Leaderboard[item].DisplayName}");
                        PlayerPrefs.SetString("NAME", r.Leaderboard[item].DisplayName);

                        //r.Leaderboard[item].Username
                    }

                    username = PlayerPrefs.GetString("NAME");


                    Debug.Log($"USERNAME IS {username}");


                    privateReceiver = "blue";
                    ChatConnectOnClick();

                    // Use the actual Photon ID of the user
                    //string photonId = chatClient.AuthValues.UserId;
                    //photonIdToUsernameMap.Add(photonId, username);
                }
            }
        }
       , OnError);
    }

    //recepientname
    string privateReceiver;
    void Awake()
    {
        isconnected = false;

        CorrectNamePref();

        

        PlayerDisplayUI.OnInviteFriend += HandleFriendInvite;
        //ConnectoToPhotonChat();

    }


   
    private void OnDestroy()
    {
        PlayerDisplayUI.OnInviteFriend -= HandleFriendInvite;
    }

    public void NameOnValueChange(string value)
    {
        //username = value;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("SENDING MESSAGE");
            //SEND MESSAGE FOR TESTING PURPOSED
            //SendPublicMessage();

            SendPrivateMessage();
        }
    }

    #endregion

    #region  Private Methods

    private void ConnectoToPhotonChat()
    {
        Debug.Log("Connecting to Photon Chat");
        chatClient.AuthValues = new AuthenticationValues("Schindler");
        ChatAppSettings chatSettings 
            = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.ConnectUsingSettings(chatSettings);

       

    }

    #endregion

    #region  Public Methods

    public void HandleFriendInvite(string recipient)
    {
        if (!PhotonNetwork.InRoom) return;
        chatClient.SendPrivateMessage(recipient, PhotonNetwork.CurrentRoom.Name);
    }

    #endregion

    #region Photon Chat Callbacks

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"Photon Chat DebugReturn: {message}");
    }

    public void OnDisconnected()
    {
        Debug.Log("You have disconnected from the Photon Chat");
       chatClient.SetOnlineStatus(ChatUserStatus.Offline);


    }
    

    public void TypeChatOnValueChange(string value)
    {
        currentchat = value;
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

    }

    public void SendPrivateMessage()
    {
        if (!string.IsNullOrEmpty(privateReceiver))
        {
            string privateChannel = privateReceiver; // Use the Photon ID as the channel name
            currentchat = $"HELLO {privateReceiver}";
            chatClient.SendPrivateMessage(privateChannel, currentchat);

            // CLEAR CHAT FIELDS HERE
            currentchat = "";
        }
        //else
        //{
        //    Debug.Log("Private receiver is not set. Please set a valid recipient before sending a private message.");
        //}
    }

   

    public void SendPublicMessage()
    {
       // Debug.Log("COMPUL OUT");
        //if (privateReceiver == "")
        //{
            //Debug.Log("COMPUL");
            currentchat = "Hello";
            chatClient.PublishMessage("RegionChannel", currentchat);

            //CLEAR CHAT FIELDS HERE
            currentchat = "";
        //}
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
            string msg = $"\n{senders[i]} messaged: {messages[i]}\n";
            //DISPLAY INSIDE MESSAGE BOX;
            chatdisplay.text += msg;
        }
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

        string msg = $"\n{sender} messaged: {message}\n";
        chatdisplay.text += msg;


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

        string party_ChannelName = "RegionChannel";

        for (int i = 0; i < channels.Length; i++)
        {
            if (party_ChannelName.Equals(channels[i]) && results[i])
            {
                ChatChannel partyChannel;
                if (this.chatClient.TryGetChannel(party_ChannelName, out partyChannel))
                {
                    if (!partyChannel.PublishSubscribers)
                    {
                        Debug.LogError("PublishSubscribers was not set during channel creation.");
                    }
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
    #endregion


  

}
