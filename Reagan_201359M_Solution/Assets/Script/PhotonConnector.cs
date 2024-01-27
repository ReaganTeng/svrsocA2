using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
//using UnityEditor.Experimental.GraphView;
//using Photon.Chat;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private string nickName;
    public static Action GetPhotonFriends = delegate { };
    public static Action OnLobbyJoined = delegate { };

    bool finallyConnected;

    #region Unity Method
    private void Awake()
    {
        finallyConnected = false;
        //nickName = PlayerPrefs.GetString("USERNAME");

        //StartConnector();
    }
    public void StartConnector()
    {
        //if (PhotonNetwork.IsConnectedAndReady 
        //    || PhotonNetwork.IsConnected)
        if(!PhotonNetwork.IsConnectedAndReady
            || !PhotonNetwork.IsConnected)
        {
            //Debug.Log("PHOTON IS NOT READY");

            return;
        }

        if (!finallyConnected)
        {
            Debug.Log("PHOTON IS READY");
            ConnectToPhoton();
            finallyConnected = true;
        }
    }


    private void Update()
    {
        StartConnector();
    }

    #endregion
    #region Private Methods
    private void ConnectToPhoton()
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion
    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("You have connected to the Photon Master Server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("You have connected to a Photon Lobby");
        Debug.Log("Invoking get Playfab friends");
        GetPhotonFriends?.Invoke();
        OnLobbyJoined?.Invoke();
    }
    #endregion
}
