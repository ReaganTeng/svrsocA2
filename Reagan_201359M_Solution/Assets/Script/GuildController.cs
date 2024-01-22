using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.GroupsModels;
using System;

using UnityEngine.UI;
using TMPro;


public class GuildController : MonoBehaviour
{
    public GameObject panel;
    Action playerCallback;


    public TMP_InputField newguildIF;

    // A local cache of some bits of PlayFab data
    // This cache pretty much only serves this example , and assumes that entities are uniquely identifiable by EntityId alone, which isn't technically true. Your data cache will have to be better.
    public readonly HashSet<KeyValuePair<string, string>> EntityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public readonly Dictionary<string, string> GroupNameById = new Dictionary<string, string>();


    //string playersownId;
    string playersownTitleID;
    //bool grpcreated;
    public GameObject guildUI;
    public GameObject guildMemberUI;


    // Start is called before the first frame update
    public GameObject guildContent;
    public GameObject guildsJoinedContent;
    public GameObject membersContent;



    public GameObject yourGuildPanel;
    public GameObject guildsJoinedPanel;
    public GameObject viewMembersPanel;


    void togglePanels(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = !canvasGrp.interactable;
        if (!canvasGrp.interactable)
        {
            canvasGrp.alpha = 0;
        }
        else
        {
            canvasGrp.alpha = 1;
        }
        //Debug.Log("DEBUGGGG");
    }

    void setPanelToFalse(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = false;
        
        canvasGrp.alpha = 0;
        
    }

    void Awake()
    {
        //playersownId = PlayerPrefs.GetString("PLAYFABID");
        playersownTitleID = PlayerPrefs.GetString("PLAYFABTITLEID");
        ListGroups(EntityKeyMaker(playersownTitleID));
        setPanelToFalse(guildsJoinedPanel);
        setPanelToFalse(viewMembersPanel);
        //ViewMembers(EntityKeyMaker(playersownTitleID));
        ViewMembers(EntityKeyMaker(playersownTitleID));

        panel.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {

        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    CreateGroup("TESTGROUP");

        //}
        //if (!grpcreated)
        //{
        //    grpcreated =true;
        //}
    }
    public void OpenPanel(System.Action callBack = null)
    {
        panel.SetActive(true);
        playerCallback = callBack;

    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        playerCallback();
    }








    

    private void OnSharedError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void ListGroups(EntityKey entityKey)
    {
        var request = new ListMembershipRequest { Entity = entityKey };
        PlayFabGroupsAPI.ListMembership(request, OnListGroups, OnSharedError);
    }
    private void OnListGroups(ListMembershipResponse response)
    {
        foreach (GuildUI guiui in guildContent.GetComponentsInChildren<GuildUI>())
        {
            Destroy(guiui.gameObject);
        }

        var prevRequest = (ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
            

            //DISPLAY GUILD IN PANELCONTENT
            GameObject guildui_GO = Instantiate(guildUI, guildContent.transform);
            GuildUI guildui = guildui_GO.GetComponent<GuildUI>();
            guildui.GuildName.text = pair.GroupName;
            //guildui.ViewGuildButton;
            string guildid = pair.Group.Id;
            guildui.RemoveGuildButton.
            onClick.AddListener(() =>
            {
                DeleteGroup(guildid);
            });
            guildui.ViewGuildButton.
            onClick.AddListener(() =>
            {
                togglePanels(viewMembersPanel);
            });
        }
    }



    public void CreateGroup(string groupName, EntityKey entityKey)
    {
        //entityKey = PlayFabGroupsAPI.enti
        // A player-controlled entity creates a new group
        var request = new CreateGroupRequest { 
            GroupName = groupName, 
            Entity = entityKey };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
    }



    public static EntityKey EntityKeyMaker(string titleId)
    {
        //return new EntityKey { Id = entityId };

        //Debug.Log($"STRING IS {titleId}");
        //Id = "8DCBB61C669A63FF"
        return new EntityKey
        {
            Id = titleId,
            Type = "title_player_account",
            //Type = PlayerPrefs.GetString("A"),
        };
    }

    //REAGAN'S CREATEGROUP
    public void CreateGroup()
    {
        string input = newguildIF.text;
        if (!hasNoletters(input))
        {
            // Create an EntityKey using the player's own ID
            EntityKey entityKey = EntityKeyMaker(playersownTitleID);
            // A player-controlled entity creates a new group
            var request = new CreateGroupRequest
            {
                GroupName = input,
                Entity = entityKey,
            };
            // Make the CreateGroup API call
            PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
        }
        else
        {
            Debug.Log("STRING HAS NO LETTER");
        }
    }


    bool hasNoletters(string input)
    {
        // Assume the string has no letters until we find one
        bool hasNoLetters = true;

        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c))
            {
                hasNoLetters = false;
                break; // Exit the loop early since we found a letter
            }
        }

       
        return hasNoLetters;
    }




    private void OnCreateGroup(CreateGroupResponse response)
    {
        Debug.Log("Group Created: " + response.GroupName + " - " + response.Group.Id);

        var prevRequest = (CreateGroupRequest)response.Request;
        EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, response.Group.Id));
        GroupNameById[response.Group.Id] = response.GroupName;


        ListGroups(EntityKeyMaker(playersownTitleID));
    }



    public void DeleteGroup(string groupId)
    {
        // A title, or player-controlled entity with authority to do so, decides to destroy an existing group
        var request = new DeleteGroupRequest { Group = EntityKeyMaker(groupId) };
        PlayFabGroupsAPI.DeleteGroup(request, OnDeleteGroup, OnSharedError);
    }
    private void OnDeleteGroup(EmptyResponse response)
    {
        var prevRequest = (DeleteGroupRequest)response.Request;
        Debug.Log("Group Deleted: " + prevRequest.Group.Id);

        var temp = new HashSet<KeyValuePair<string, string>>();
        foreach (var each in EntityGroupPairs)
        {
            if (each.Value != prevRequest.Group.Id)
            {
                temp.Add(each);
            }

        }
        EntityGroupPairs.IntersectWith(temp);
        GroupNameById.Remove(prevRequest.Group.Id);

        ListGroups(EntityKeyMaker(playersownTitleID));

    }




    //REAGAN
    public void ViewMembers(EntityKey entityKey)
    {
        //entityKey = PlayFabGroupsAPI.enti
        // A player-controlled entity creates a new group
        var request = new ListGroupMembersRequest
        {
            Group = entityKey,
        };

        Debug.Log($"GROUP NAME {request.Group}");
        PlayFabGroupsAPI.ListGroupMembers(request,
            OnViewMembers,
            OnSharedError);
    }

    public void OnViewMembers(ListGroupMembersResponse r)
    {
        foreach (GuildMemberUI gmUI in membersContent.GetComponentsInChildren<GuildMemberUI>())
        {
            Destroy(gmUI.gameObject);
        }


        foreach (var member in r.Members)
        {
            GameObject guildmemUI = Instantiate(guildMemberUI, membersContent.transform);
            GuildMemberUI gmui = guildmemUI.GetComponent<GuildMemberUI>();
            gmui.MemberName.text = $"{member.RoleId} {member.RoleName}";
            
            gmui.KickButton.onClick.AddListener(
                () =>
                {
                    //KickMember(member.RoleId);
                }
            );
        }
    }
    //






    public void InviteToGroup(string groupId, EntityKey entityKey)
    {
        // A player-controlled entity invites another player-controlled entity to an existing group
        var request = new InviteToGroupRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.InviteToGroup(request, OnInvite, OnSharedError);
    }
    public void OnInvite(InviteToGroupResponse response)
    {
        var prevRequest = (InviteToGroupRequest)response.Request;

        // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
        var request = new AcceptGroupInvitationRequest { Group = EntityKeyMaker(prevRequest.Group.Id), Entity = prevRequest.Entity };
        PlayFabGroupsAPI.AcceptGroupInvitation(request, OnAcceptInvite, OnSharedError);
    }
    public void OnAcceptInvite(EmptyResponse response)
    {
        var prevRequest = (AcceptGroupInvitationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
        EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, prevRequest.Group.Id));
    }
    



    public void ApplyToGroup(string groupId, EntityKey entityKey)
    {
        // A player-controlled entity applies to join an existing group (of which they are not already a member)
        var request = new ApplyToGroupRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.ApplyToGroup(request, OnApply, OnSharedError);
    }
    public void OnApply(ApplyToGroupResponse response)
    {
        var prevRequest = (ApplyToGroupRequest)response.Request;

        // Presumably, this would be part of a separate process where the recipient reviews and accepts the request
        var request = new AcceptGroupApplicationRequest { Group = prevRequest.Group, Entity = prevRequest.Entity };
        PlayFabGroupsAPI.AcceptGroupApplication(request, OnAcceptApplication, OnSharedError);
    }
    public void OnAcceptApplication(EmptyResponse response)
    {
        var prevRequest = (AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
    }


    public void KickMember(string groupId, EntityKey entityKey)
    {
        var request = new RemoveMembersRequest { Group = EntityKeyMaker(groupId), Members = new List<EntityKey> { entityKey } };
        PlayFabGroupsAPI.RemoveMembers(request, OnKickMembers, OnSharedError);
    }
    private void OnKickMembers(EmptyResponse response)
    {
        var prevRequest = (RemoveMembersRequest)response.Request;

        Debug.Log("Entity kicked from Group: " + prevRequest.Members[0].Id + " to " + prevRequest.Group.Id);
        EntityGroupPairs.Remove(new KeyValuePair<string, string>(prevRequest.Members[0].Id, prevRequest.Group.Id));
    }
}
