using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiDisplayFriends : MonoBehaviour
{

    public Transform friendcontainer;
    public PlayerDisplayUI uifriendprefab;

    private void Awake()
    {
        PhotonFriendController.OnDisplayFriends += HandleDisplayFriends;
    }

    private void OnDestroy()
    {
        PhotonFriendController.OnDisplayFriends -= HandleDisplayFriends;
    }

    private void HandleDisplayFriends(List<FriendInfo> friends)
    {
       foreach(Transform child in friendcontainer)
        {
            Destroy(child.gameObject);
        }

       foreach(FriendInfo friend in friends)
        {
            Debug.Log("FRIEND");
            PlayerDisplayUI uifriend = Instantiate(uifriendprefab, friendcontainer);
            uifriend.Initialize(friend);
        }
    }
}
