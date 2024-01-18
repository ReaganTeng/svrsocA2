using UnityEngine;
using System;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class PhotonChatController : MonoBehaviour, IChatClientListener
{
    public string nickName;
    ChatClient chatClient;

    public static Action<string, string> OnRoomInvite = delegate { };
    public static Action<ChatClient> OnChatConnected = delegate { };
    //public static Action<PhotonStatus> OnStatusUpdated = delegate { };


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"Photon Chat OnStatusUpdate: {user} changed to {status}: {message}");
        //PhotonStatus newStatus = new PhotonStatus(user, status, (string)message);
        Debug.Log($"Status Update for {user} and its now {status}.");
        //OnStatusUpdated?.Invoke(newStatus);
    }

    // Start is called before the first frame update
    void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
        UiFriend.OnInviteFriend += HandleFriendInvite;
    }

    void OnDestroy()
    {
        UiFriend.OnInviteFriend -= HandleFriendInvite;
    }

    public void HandleFriendInvite(string recipient)
    {
        chatClient.SendPrivateMessage(recipient, PhotonNetwork.CurrentRoom.Name);
    }

    void Start()
    {
        chatClient = new ChatClient(this);
        ConnectToPhotonChat();
    }

    void ConnectToPhotonChat()
    {
        Debug.Log("Connecting to photon chat");
        chatClient.AuthValues = new Photon.Chat.AuthenticationValues(nickName);
        //ChatAppSettings chatSettings 
        //    = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        //chatClient.ConnectUsingSettings(chatSettings);
    }


    public void OnDisconnected()
    {
        Debug.Log("You are disconnected from chat");
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }

    public void OnConnected()
    {
        Debug.Log("You are connected to chat");
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        SendDirectMessage("BRI", "HI");
    }


    public void SendDirectMessage(string recipient, string message)
    {
        chatClient.SendPrivateMessage(recipient, message);
    }

    // Update is called once per frame
    void Update()
    {
        chatClient.Service();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if(!string.IsNullOrEmpty($"{message}"))
        {
            string[] splitName = channelName.Split(new char[] {':'});
            string senderName = splitName[0];
            if(!sender.Equals(senderName, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"{sender}: {message}");
                
                //SHOW DISPLAY INVITES UI
                OnRoomInvite?.Invoke(sender, $"{message}");
            }
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

   

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}
