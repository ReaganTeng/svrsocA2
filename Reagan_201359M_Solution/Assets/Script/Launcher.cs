using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public bool isOffline = false;
    private Vector3 spawnPosition = new Vector3(1.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        if (!isOffline)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (!isOffline)
        {
            Debug.Log("Connected");
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (!isOffline)
        {
            Debug.Log("Joined");

            // Instantiate local player
            PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(0, -3.0f, 0), Quaternion.identity);

            // Set custom property for local player
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SpawnPosition", spawnPosition } });
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Check if the updated properties include SpawnPosition
        if (changedProps.ContainsKey("SpawnPosition"))
        {
            // Instantiate other players for the local player after properties are updated
            InstantiateOtherPlayers();
        }
    }

    void InstantiateOtherPlayers()
    {
        foreach (var otherPlayer in PhotonNetwork.PlayerList)
        {
            if (otherPlayer != PhotonNetwork.LocalPlayer)
            {
                Vector3 otherSpawnPosition = (Vector3)otherPlayer.CustomProperties["SpawnPosition"];
                PhotonNetwork.Instantiate(PlayerPrefab.name, otherSpawnPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
