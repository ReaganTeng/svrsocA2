using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    public string nickName;
    public static Action GetPhotonFriends = delegate { };


    private void Awake()
    {
        nickName = PlayerPrefs.GetString("USERNAME");
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
    }


    void HandleRoomInviteAccept(string roomName)
    {
        PlayerPrefs.SetString("PHOTONROOM", roomName);
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if(PhotonNetwork.InLobby) {
                JoinPlayerRoom();
            }
            
        }
    }

    public void OnCreateRoomClicked(string roomName)
    {
        CreatePhotonRoom(roomName);
    }

    void JoinPlayerRoom()
    {
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        PlayerPrefs.SetString("PHOTONROOM", "");
        PhotonNetwork.JoinRoom(roomName); 
    }

    // Start is called before the first frame update
    void Start()
    {
        
        string randomName = $"Tester{Guid.NewGuid()}";
        ConnectToPhoton(randomName);
    }

    // Update is called once per frame
    void ConnectToPhoton(string nickName)
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }
    void CreatePhotonRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, 
            TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
       
        Debug.Log("Connected to photon master server");
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connected to photon lobby");
        GetPhotonFriends?.Invoke();
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        if(!string.IsNullOrEmpty(roomName))
        {
            JoinPlayerRoom();
        }
        else
        {
            CreatePhotonRoom($"{PhotonNetwork.LocalPlayer.UserId}'s Room");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Photon room {PhotonNetwork.CurrentRoom.Name} created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined photon room {PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnLeftRoom()
    {
        Debug.Log($"Left photon room {PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnJoinRoomFailed(short returnCode, 
        string message)
    {
        Debug.Log($"Failed to join photon room {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnPlayerEnteredRoom(
        Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.UserId} has joined room " +
            $"{PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.UserId} has left room " +
           $"{PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnMasterClientSwitched(
        Photon.Realtime.Player newMasterClient)
    {
        Debug.Log($"New master client {newMasterClient.UserId}");
    }
}
