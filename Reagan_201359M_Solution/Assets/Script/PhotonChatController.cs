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

public class PhotonChatController : MonoBehaviour, IChatClientListener
{

   // [SerializeField] private string nickName;
    private ChatClient chatClient;

    //recepientname
    string privateReceiver;

    public static Action<string, string> OnRoomInvite = delegate { };
    public static Action<ChatClient> OnChatConnected = delegate { };
    public static Action<PhotonStatus> OnStatusUpdated = delegate { };

    [HideInInspector]
    public string username;
    string currentchat;
    bool isconnected;

    public TextMeshProUGUI chatdisplay;

    #region Unity Methods

    private void Awake()
    {
        //nickName = PlayerPrefs.GetString("USERNAME");
        //nickName = "RAY";
        isconnected = false;
        username = PlayerPrefs.GetString("NAME");
        privateReceiver = "";
        PlayerDisplayUI.OnInviteFriend += HandleFriendInvite;

        ChatConnectOnClick();
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
        Debug.Log("Connecting to Photon Chat");
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
        Debug.Log("You have connected to the Photon Chat");
        isconnected = true;
        //SECOND VERSION
        chatClient.Subscribe(new string[] { "RegionChannel" });





        //FIRST VERSION
        OnChatConnected?.Invoke(chatClient);
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        chatClient.SendPrivateMessage("bri", "HELLO");

    }

    public void SendPrivateMessage()
    {
        currentchat = "HELLO BLUE";
        chatClient.SendPrivateMessage("blue", "HELLO BLUE");

        //CLEAR CHAT FIELDS HERE
        currentchat = "";
    }

   

    public void SendPublicMessage()
    {
       // Debug.Log("COMPUL OUT");
        if (privateReceiver == "")
        {
            //Debug.Log("COMPUL");
            currentchat = "Hello";
            chatClient.PublishMessage("RegionChannel", currentchat);

            //CLEAR CHAT FIELDS HERE
            currentchat = "";
        }
    }


    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"Photon Chat OnChatStateChange: {state.ToString()}");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log($"Photon Chat OnGetMessages {channelName}");
                
        for (int i = 0; i < senders.Length; i++)
        {
            Debug.Log($"{senders[i]} messaged: {messages[i]}");
            string msg = $"\n{senders[i]} messaged: {messages[i]}\n";

            //DISPLAY INSIDE MESSAGE BOX;
            chatdisplay.text += msg;
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"{sender} messaged: {message}");
        string msg = $"\n{sender} messaged: {message}\n";
        //DISPLAY INSIDE MESSAGE BOX;
        chatdisplay.text += msg;

        /*if (!string.IsNullOrEmpty(message.ToString()))
        {
            // Channel Name format [Sender : Recipient]
            string[] splitNames = channelName.Split(new char[] { ':' });
            string senderName = splitNames[0];

            if (!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"{sender}: {message}");
                OnRoomInvite?.Invoke(sender, message.ToString());
            }
        }*/
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"Photon Chat OnSubscribed");
        //for (int i = 0; i < channels.Length; i++)
        //{
        //    Debug.Log($"{channels[i]}");
        //}
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
