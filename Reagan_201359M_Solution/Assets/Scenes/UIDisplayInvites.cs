using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayInvites : MonoBehaviour
{
    public Transform inviteContainer;
    public UIInvite uiInvitePrefab;
    public RectTransform contentRect;
    public Vector2 originalSize;
    public Vector2 increaseSize;

    List<UIInvite> invites;

    private void Awake()
    {
        invites = new List<UIInvite>();
        
        contentRect = inviteContainer.GetComponent<RectTransform>();
        originalSize = contentRect.sizeDelta;
        increaseSize = new Vector2(0, 
            uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);
        PhotonChatController.OnRoomInvite += HandleRoomInvite;

        UIInvite.OnInviteAccept += HandleInviteAccept;
        UIInvite.OnInviteAccept += HandleInviteDecline;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnRoomInvite -= HandleRoomInvite;
        UIInvite.OnInviteAccept -= HandleInviteAccept;
        UIInvite.OnInviteAccept -= HandleInviteDecline;
    }


    void HandleInviteAccept(UIInvite invite)
    {
        //throw new NotImplementedException();
        if(invites.Contains(invite))
        {
            invites.Remove(invite);
            Destroy(invite.gameObject);
        }
    }

    void HandleInviteDecline(UIInvite invite)
    {
        //throw new NotImplementedException();
        if (invites.Contains(invite))
        {
            invites.Remove(invite);
            Destroy(invite.gameObject);
        }
    }

    void HandleRoomInvite(string friend, string room)
    {
        Debug.Log($"Room invite for {friend} to room {room}");
        UIInvite uiInvite
            = Instantiate(uiInvitePrefab, inviteContainer);
    }
}
