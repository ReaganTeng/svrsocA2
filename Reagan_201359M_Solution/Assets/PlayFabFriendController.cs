using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEditor.PackageManager;
using System.Linq;

public class PlayFabFriendController : MonoBehaviour
{
    public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { };

    List<FriendInfo> friends;

    private void Awake()
    {
        friends = new List<FriendInfo>();
        PhotonConnector.GetPhotonFriends += HandleGetFriends;
        UiAddFriend.OnAddFriend += HandleAddPlayfabFriend;
        UiFriend.OnRemoveFriend += HandleRemoveFriend;
    }

    private void HandleGetFriends()
    {
        GetPlayFabFriends();
    }

    private void OnDestroy()
    {
        PhotonConnector.GetPhotonFriends -= HandleGetFriends;
        UiAddFriend.OnAddFriend -= HandleAddPlayfabFriend;
        UiFriend.OnRemoveFriend -= HandleRemoveFriend;

    }

    private void HandleRemoveFriend(string name)
    {
        string id = friends.FirstOrDefault(f 
            => f.TitleDisplayName == name).FriendPlayFabId.ToString();
        var request = new RemoveFriendRequest
        {
            FriendPlayFabId = name,
        };
        PlayFabClientAPI.RemoveFriend(request, OnFriendRemoveSuccess, OnFailure);
    }

    private void OnFriendRemoveSuccess(RemoveFriendResult result)
    {
        GetPlayFabFriends();
    }

    private void HandleAddPlayfabFriend(string name)
    {
        var request = new AddFriendRequest {FriendTitleDisplayName = name};
        PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
        //throw new NotImplementedException();
    }

    private void OnFailure(PlayFabError error) {
        Debug.Log($"ERROR {error.GenerateErrorReport()}");
    }

    private void OnFriendAddedSuccess(AddFriendResult result)
    {
        GetPlayFabFriends();
    }

    private void GetPlayFabFriends()
    {
        var request = new GetFriendsListRequest
        { XboxToken = null
        };

        PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);


    }

    private void OnFriendsListSuccess(GetFriendsListResult result)
    {
        friends = result.Friends;
        OnFriendListUpdated?.Invoke(result.Friends);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
