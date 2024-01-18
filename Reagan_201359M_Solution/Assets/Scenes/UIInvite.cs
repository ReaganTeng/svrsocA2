using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInvite : MonoBehaviour
{
    public string _friendName;
    public string _roomName;
    public TextMeshProUGUI _friendNameText;

    public static Action<UIInvite> OnInviteAccept = delegate { };
    public static Action<string> OnRoomInviteAccept = delegate { };
    public static Action<UIInvite> OnInviteDecline = delegate { };

    public void Initialize(string friendName, string roomName)
    {
      _friendName = friendName;
        _roomName = roomName;

        _friendNameText.SetText(_friendName);
    }


    public void AcceptInvite()
    {
        OnInviteAccept?.Invoke(this);
        OnRoomInviteAccept?.Invoke(_roomName);
    }

    public void DeclineInvite()
    {
        OnInviteDecline?.Invoke(this);
    }


}
