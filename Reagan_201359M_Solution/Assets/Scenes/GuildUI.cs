using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuildUI : MonoBehaviour
{
    public TextMeshProUGUI GuildName;
    public TextMeshProUGUI LeaderName;


    public Button ViewGuildButton;
    public Button RemoveGuildButton;
    public Button InviteButton;
    public Button AcceptInvitation;
    public Button RejectInvitation;


    [HideInInspector]
    public string guilid;
}
