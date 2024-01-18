using System;
using TMPro;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;

public class UiFriend : MonoBehaviour
{
    
    [SerializeField] private TMP_Text friendNameText;
    public FriendInfo friend;
    [SerializeField] private string friendName;
    [SerializeField] private bool isOnline;
    [SerializeField] private Image onlineImage;
    [SerializeField] private GameObject inviteButton;
    [SerializeField] private Color onlineColor;
    [SerializeField] private Color offlineColor;

    public static Action<string> OnRemoveFriend = delegate { };
    public static Action<string> OnInviteFriend = delegate { };
    public static Action<string> OnGetCurrentStatus = delegate { };
    public static Action OnGetRoomStatus = delegate { };

    private void Awake()
    {
        //PhotonChatController.OnStatusUpdated += HandleStatusUpdated;
        //PhotonChatFriendController.OnStatusUpdated += HandleStatusUpdated;
        //PhotonRoomController.OnRoomStatusChange += HandleInRoom;
    }
    private void OnDestroy()
    {
        //PhotonChatController.OnStatusUpdated -= HandleStatusUpdated;
        //PhotonChatFriendController.OnStatusUpdated -= HandleStatusUpdated;
        //PhotonRoomController.OnRoomStatusChange -= HandleInRoom;
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(friendName)) return;
        OnGetCurrentStatus?.Invoke(friendName);
        OnGetRoomStatus?.Invoke();
    }

    public void Initialize(FriendInfo friend)
    {
        Debug.Log($"{friend.UserId} is online: {friend.IsOnline} ; in room: {friend.IsInRoom} ; room name: {friend.Room}");

        this.friend = friend;
        friendNameText.SetText(this.friend.UserId);

        SetupUI();
    }
    public void Initialize(string friendName)
    {
        Debug.Log($"{friendName} is added");
        this.friendName = friendName;

        SetupUI();
        OnGetCurrentStatus?.Invoke(friendName);
        OnGetRoomStatus?.Invoke();
    }

    //void HandleStatusUpdated(PhotonStatus status)
    //{
    //    if (string.Compare(friendName, status.PlayerName) == 0)
    //    {
    //        Debug.Log($"Updating status in UI for {status.PlayerName} to status {status.Status}");
    //        SetStatus(status.Status);
    //    }
    //}

    void HandleInRoom(bool inRoom)
    {
        Debug.Log($"Updating invite ui to {inRoom}");
        inviteButton.SetActive(inRoom && isOnline);
    }

    void SetupUI()
    {
        friendNameText.SetText(friendName);
        inviteButton.SetActive(false);
    }

    void SetStatus(int status)
    {
        if (status == ChatUserStatus.Online)
        {
            onlineImage.color = onlineColor;
            isOnline = true;
            OnGetRoomStatus?.Invoke();
        }
        else
        {
            onlineImage.color = offlineColor;
            isOnline = false;
            inviteButton.SetActive(false);
        }
    }

    public void RemoveFriend()
    {
        Debug.Log($"Clicked to remove friend {friendName}");
        OnRemoveFriend?.Invoke(friend.UserId);
    }

    public void InviteFriend()
    {
        Debug.Log($"Clicked to invite friend {friendName}");
        OnInviteFriend?.Invoke(friendName);
    }
    
}
