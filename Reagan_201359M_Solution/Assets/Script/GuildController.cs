using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PFGroups = PlayFab.GroupsModels;
using PFClient = PlayFab.ClientModels;
using System;

using UnityEngine.UI;
using TMPro;

using PlayFab.ClientModels;
using PlayFab.AuthenticationModels;
using System.Text.RegularExpressions;
using PlayFab.GroupsModels;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class GuildController : MonoBehaviour
{
    public TMP_Dropdown optionlist;

    public GameObject panel;
    Action playerCallback;


    public GameObject closebuttonfriendspanel;
    public GameObject guildPanelCloseButton;


    public TMP_InputField newguildIF;

    public TextMeshProUGUI GroupName;

    // A local cache of some bits of PlayFab data
    // This cache pretty much only serves this example , and assumes that entities are uniquely identifiable by EntityId alone, which isn't technically true. Your data cache will have to be better.
    public readonly HashSet<KeyValuePair<string, string>> EntityGroupPairs = new HashSet<KeyValuePair<string, string>>();
    public readonly Dictionary<string, string> GroupNameById = new Dictionary<string, string>();


    string creatorTitle = "Administrators";

    //string playersownId;
    string playersownTitleID;
    //bool grpcreated;
    public GameObject guildUI;
    public GameObject guildMemberUI;


    // Start is called before the first frame update
    public GameObject guildsCreatedContent;
    public GameObject guildsJoinedContent;
    public GameObject membersContent;
    public GameObject GuildInvitationContent;
    public GameObject ViewGuildsInvitedContent;

    public GameObject guildChoicePanel;


    public GameObject yourGuildPanel;
    public GameObject guildsJoinedPanel;
    public GameObject viewMembersPanel;
    public GameObject viewGuildInvitedPanel;


    [HideInInspector]
    public string titleIDChosen = "";

    List<GameObject> guildPanels;


    CloudScriptManager cloudScriptManager;
    public void togglePanels(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = !canvasGrp.interactable;
        canvasGrp.blocksRaycasts = !canvasGrp.blocksRaycasts;
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



    void setPanelToTrue(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = true;
        canvasGrp.blocksRaycasts = true;
        canvasGrp.alpha = 1;
        
    }

    void setPanelToFalse(GameObject Panel)
    {
        CanvasGroup canvasGrp = Panel.GetComponent<CanvasGroup>();
        canvasGrp.interactable = false;
        canvasGrp.blocksRaycasts = false;
        canvasGrp.alpha = 0;
    }


    public void onOptionsChanged()
    {
        int selectedOption = optionlist.value;
        setPanelToFalse(viewMembersPanel);

        // Validate selectedOption to prevent index out of range
        if (selectedOption < 0 || selectedOption >= guildPanels.Count)
        {
            //Debug.LogError("Invalid selected option");
            return;
        }

        Debug.Log($"SELECTED OPTION {selectedOption}");


        // Set all panels to false
        foreach (var panel in guildPanels)
        {
            setPanelToFalse(panel);
        }

        // Set the selected panel to true
        setPanelToTrue(guildPanels[selectedOption]);

        guildPanelCloseButton.SetActive(true);
    }


    void Awake()
    {
        cloudScriptManager = GetComponent<CloudScriptManager>();


        guildPanels = new List<GameObject>
        {
            yourGuildPanel,
            guildsJoinedPanel,
            viewGuildInvitedPanel
        };


        //playersownId = PlayerPrefs.GetString("PLAYFABID");
        playersownTitleID = PlayerPrefs.GetString("PLAYFABTITLEID");
        ListGroupsCreated(playersownTitleID);
        ListInvitationGrp(playersownTitleID);

        setPanelToFalse(viewMembersPanel);
        setPanelToFalse(guildChoicePanel);


        ListGroupsInvited();

        ViewJoinedGroups();
        ClosePanel();

        //cloudScriptManager.GetGroupInvitations();
    }


    public void CloseViewMembersPanel()
    {
        setPanelToFalse(viewMembersPanel);

        guildPanelCloseButton.SetActive(false);
    }

    //LIST THE GROUPS YOUR INVITED TO
    #region ListGroupsInvited 
    void ListGroupsInvited()
    {

        foreach (GuildUI guiui in ViewGuildsInvitedContent.GetComponentsInChildren<GuildUI>())
        {
            Destroy(guiui.gameObject);
        }

        PFGroups.ListMembershipOpportunitiesRequest req = new PFGroups.ListMembershipOpportunitiesRequest
        {
            Entity = EntityKeyMaker(playersownTitleID),
           
        };

        PlayFabGroupsAPI.ListMembershipOpportunities(req,
            result =>
            {
                foreach (var invitations in result.Invitations)
                {
                    PFGroups.GetGroupRequest r = new PFGroups.GetGroupRequest
                    {
                        Group = invitations.Group,
                    };

                    PlayFabGroupsAPI.GetGroup(r, 
                    res =>
                    {
                        //Debug.Log($"\nINVITED TO {res.GroupName}\n");
                        GameObject GUI = Instantiate(guildUI, ViewGuildsInvitedContent.transform);
                        GuildUI gui = GUI.GetComponent<GuildUI>();
                        gui.GuildName.gameObject.SetActive(true);
                        gui.GuildName.text = res.GroupName;
                        gui.LeaderName.gameObject.SetActive(true);
                        
                        gui.RejectInvitation.gameObject.SetActive(true);
                        gui.RejectInvitation.onClick.AddListener(() =>
                        {
                            RejectInvitation(res.Group.Id, EntityKeyMaker(playersownTitleID));
                            ListGroupsInvited();
                        });

                        gui.AcceptInvitation.gameObject.SetActive(true);
                        gui.AcceptInvitation.onClick.AddListener(() =>
                        {
                            AcceptInvitation(res.Group.Id, EntityKeyMaker(playersownTitleID));
                        });
                    }, OnSharedError);
                }
            }, OnSharedError);
    }
    #endregion
    //

    // Update is called once per frame
    void Update()
    {

    }
    public void OpenPanel(Action callBack = null)
    {
       // panel.SetActive(true);
        togglePanels(panel);

        playerCallback = callBack;

    }

    public void ClosePanel()
    {
//        panel.SetActive(false);
        togglePanels(panel);
        setPanelToTrue(yourGuildPanel);
        setPanelToFalse(guildsJoinedPanel);
        setPanelToFalse(viewGuildInvitedPanel);

        if (playerCallback != null)
        {
            playerCallback();
        }
    }


    //VIEW THE GROUPS THAT PLAYER HAS JOINED
    #region ViewJoinedGroups
    public void ViewJoinedGroups()
    {
        PlayFabGroupsAPI.ListMembership(
            new ListMembershipRequest(),
            OnListMembershipsSuccess,
            OnSharedError
        );
    }

    private void OnListMembershipsSuccess(ListMembershipResponse result)
    {
        // Process the list of memberships
        List<GroupWithRoles> memberships = result.Groups;

        foreach (GuildUI guiui in guildsJoinedContent.GetComponentsInChildren<GuildUI>())
        {
            Destroy(guiui.gameObject);
        }

        var prevRequest = (ListMembershipRequest)result.Request;
        foreach (var pair in memberships)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;

            //DISPLAY GUILD IN PANELCONTENT
            GameObject guildui_GO = Instantiate(guildUI, guildsJoinedContent.transform);
            GuildUI guildui = guildui_GO.GetComponent<GuildUI>();
            guildui.GuildName.gameObject.SetActive(true);
            guildui.GuildName.text = pair.GroupName;
            string guildid = pair.Group.Id;
            guildui.guilid = guildid;

            guildui.ViewGuildButton.gameObject.SetActive(true);
            guildui.ViewGuildButton.
            onClick.AddListener(() =>
            {
                ViewMembersInOthergroup(EntityKeyMaker(guildid));
                togglePanels(viewMembersPanel);
                guildPanelCloseButton.gameObject.SetActive(false);
            });
        }

        
    }
    #endregion


    //VIEW MEMBERS IN OTHER GROUPS
    #region ViewMembersInOtherGroup
    public void ViewMembersInOthergroup(PFGroups.EntityKey guildid)
    {
        // First, get the group details
        var getRequest = new GetGroupRequest
        {
            Group = guildid
        };

        PlayFabGroupsAPI.GetGroup(
            getRequest,
            getRes =>
            {
                //hsgsgsh
                // Now that you have the group details, including the name, proceed to list members
                var listMembersRequest = new ListGroupMembersRequest
                {
                    Group = guildid,
                };

                PlayFabGroupsAPI.ListGroupMembers(
                    listMembersRequest,
                    listRes =>
                    {
                        //GET GROUP NAME BY FINDING THE GUILD PLAYER HAS JOINED
                        var LMR = new ListMembershipRequest
                        {
                            Entity = EntityKeyMaker(playersownTitleID),
                        };

                        PlayFabGroupsAPI.ListMembership(
                            LMR,
                            res =>
                            {
                                foreach (var group in res.Groups)
                                {
                                    //Debug.Log($"FOUND ID {group.GroupName}");
                                    if (group.Group.Id == guildid.Id)
                                    {
                                        Debug.Log("FOUND ID");
                                        GroupName.text = $"{group.GroupName} Members";
                                        break;
                                    }
                                }
                            },
                            OnSharedError
                        );
                        //

                        OnViewMembersInOtherGroup(listMembersRequest, listRes);
                    },
                    OnSharedError
                );
            },
            OnSharedError
        );
    }

    public void OnViewMembersInOtherGroup(ListGroupMembersRequest group, ListGroupMembersResponse r)
    {
        foreach (GuildMemberUI gmUI in membersContent.GetComponentsInChildren<GuildMemberUI>())
        {
            Destroy(gmUI.gameObject);
        }

        //var getgrprequest = new GetGroupRequest
        //{
        //    Group = EntityKeyMaker(group),
        //};

        foreach (var member in r.Members)
        {
            string membertitileid = "";
            GameObject guildmemUI = Instantiate(guildMemberUI, membersContent.transform);
            GuildMemberUI gmui = guildmemUI.GetComponent<GuildMemberUI>();

            // CYCLE THROUGH ALL THE IDS IN EACH MEMBER
            foreach (var mem in member.Members)
            {
                membertitileid = mem.Key.Id;
                string memberDisplayname = mem.Lineage["master_player_account"].Id;
                GetDisplayName(memberDisplayname, gmui.MemberName, member.RoleName);

                gmui.KickButton.gameObject.SetActive(false);

            }
        }
    }

    #endregion

    //TO DISPLAY IT IN THE FRIENDS PANEL, DISPLAY OPTIONS TO WHICH GROUP YOU WANT INVITEE TO JOIN
    #region DisplayGroupOption
    public void ListInvitationGrp(string entitykey)
    {
        var request = new PFGroups.ListMembershipRequest { Entity = EntityKeyMaker(entitykey),
       };
        PlayFabGroupsAPI.ListMembership(request, OnListInvitationGrp, OnSharedError);
    }
    private void OnListInvitationGrp(PFGroups.ListMembershipResponse response)
    {
        foreach (GuildUI guiui in GuildInvitationContent.GetComponentsInChildren<GuildUI>())
        {
            Destroy(guiui.gameObject);
        }

        var prevRequest = (PFGroups.ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));

            //DISPLAY GUILD IN PANELCONTENT
            GameObject guildui_GO = Instantiate(guildUI, GuildInvitationContent.transform);
            GuildUI guildui = guildui_GO.GetComponent<GuildUI>();
            guildui.GuildName.gameObject.SetActive(true);
            guildui.GuildName.text = pair.GroupName;
            string guildid = pair.Group.Id;
            guildui.guilid = guildid;

            guildui.InviteButton.gameObject.SetActive(true);
            guildui.InviteButton.onClick.AddListener(() =>
            {

                InviteToGroup(guildid, EntityKeyMaker(titleIDChosen));
                setPanelToFalse(guildChoicePanel);
                closebuttonfriendspanel.SetActive(true);
                //Debug.Log($"INVITED {titleIDChosen}");
            });
        }
    }
    #endregion
    //

    private void OnSharedError(PlayFabError error)
    {
        Debug.Log("GOT ERROR");
        Debug.LogError(error.GenerateErrorReport());
    }

    //LIST THE GROUPS THAT PLAYER HAS CREATED
    #region ListGroupsCreated
    public void ListGroupsCreated(string entityKey)
    {
        var request = new ListMembershipRequest { Entity =
                    EntityKeyMaker(entityKey),
        };
        PlayFabGroupsAPI.ListMembership(request, OnListGroupsCreated, OnSharedError);

        Debug.Log("LISTED");
    }
    void OnListGroupsCreated(ListMembershipResponse response)
    {
        foreach (GuildUI guiui in guildsCreatedContent.GetComponentsInChildren<GuildUI>())
        {
            Destroy(guiui.gameObject);
        }

        var prevRequest = (ListMembershipRequest)response.Request;
        foreach (var pair in response.Groups)
        {
            GroupNameById[pair.Group.Id] = pair.GroupName;
            EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, pair.Group.Id));
            
            //DISPLAY GUILD IN PANELCONTENT
            GameObject guildui_GO = Instantiate(guildUI, guildsCreatedContent.transform);
            GuildUI guildui = guildui_GO.GetComponent<GuildUI>();
            guildui.GuildName.gameObject.SetActive(true);
            guildui.GuildName.text = pair.GroupName;
            string guildid = pair.Group.Id;
            guildui.guilid = guildid;

            guildui.RemoveGuildButton.gameObject.SetActive(true);
            guildui.RemoveGuildButton.
            onClick.AddListener(() =>
            {
                DeleteGroup(guildid);
            });
            guildui.ViewGuildButton.gameObject.SetActive(true);
            guildui.ViewGuildButton.
            onClick.AddListener(() =>
            {
               ViewMembersInOwngroup(EntityKeyMaker(guildid));
                togglePanels(viewMembersPanel);
                guildPanelCloseButton.gameObject.SetActive(false);
            });

            //Debug.Log("LISTED");
        }
    }
    #endregion






    #region CreatingGuild
    public void CreateGroup()
    {
        string input = newguildIF.text;
        if (!hasNoletters(input))
        {
            // Create an EntityKey using the player's own ID
            PFGroups.EntityKey entityKey = EntityKeyMaker(playersownTitleID);
            // A player-controlled entity creates a new group
            var request = new PFGroups.CreateGroupRequest
            {
                GroupName = input,
                Entity = entityKey,
            };
            // Make the CreateGroup API call
            PlayFabGroupsAPI.CreateGroup(request, OnCreateGroup, OnSharedError);
        }
        //else
        //{
        //    Debug.Log("STRING HAS NO LETTER");
        //}
    }


    private void OnCreateGroup(PFGroups.CreateGroupResponse response)
    {
        //Debug.Log("Group Created: " + response.GroupName + " - " + response.Group.Id);
        var prevRequest = (PFGroups.CreateGroupRequest)response.Request;
        EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, response.Group.Id));
        GroupNameById[response.Group.Id] = response.GroupName;
        
        ListGroupsCreated(playersownTitleID);
    }
    #endregion


    #region RemoveGuild
    public void DeleteGroup(string groupId)
    {
        // A title, or player-controlled entity with authority to do so, decides to destroy an existing group
        var request = new PFGroups.DeleteGroupRequest { Group = EntityKeyMaker(groupId) };
        PlayFabGroupsAPI.DeleteGroup(request, OnDeleteGroup, OnSharedError);
    }
    private void OnDeleteGroup(PFGroups.EmptyResponse response)
    {
        var prevRequest = (PFGroups.DeleteGroupRequest)response.Request;
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
        ListGroupsCreated(playersownTitleID);
    }
    #endregion



    //VIEW CURRENT MEMBERS IN PLAYER'S OWN GROUP
    #region ViewMembersInPlayerGroup
    public void ViewMembersInOwngroup(PFGroups.EntityKey guildid)
    {
        // First, get the group details
        var getRequest = new GetGroupRequest
        {
            Group = guildid
        };

        PlayFabGroupsAPI.GetGroup(
            getRequest,
            getRes =>
            {
                //hsgsgsh
                // Now that you have the group details, including the name, proceed to list members
                var listMembersRequest = new ListGroupMembersRequest
                {
                    Group = guildid,
                };

                PlayFabGroupsAPI.ListGroupMembers(
                    listMembersRequest,
                    listRes =>
                    {
                        //GET GROUP NAME BY FINDING THE GUILD PLAYER HAS JOINED
                        var LMR = new ListMembershipRequest
                        {
                            Entity = EntityKeyMaker(playersownTitleID),
                        };

                        PlayFabGroupsAPI.ListMembership(
                            LMR,
                            res =>
                            {
                                foreach(var group in res.Groups)
                                {
                                    //Debug.Log($"FOUND ID {group.GroupName}");
                                    if (group.Group.Id == guildid.Id)
                                    {
                                        Debug.Log("FOUND ID");
                                        GroupName.text = $"{group.GroupName} Members";
                                        break;
                                    }
                                }
                            },
                            OnSharedError
                        );
                        //

                        OnViewMembersInOwnGroup(listMembersRequest, listRes);
                    },
                    OnSharedError
                );
            },
            OnSharedError
        );
    }

    public void OnViewMembersInOwnGroup(ListGroupMembersRequest group, ListGroupMembersResponse r)
    {
        foreach (GuildMemberUI gmUI in membersContent.GetComponentsInChildren<GuildMemberUI>())
        {
            Destroy(gmUI.gameObject);
        }

        //var getgrprequest = new GetGroupRequest
        //{
        //    Group = EntityKeyMaker(group),
        //};

        foreach (var member in r.Members)
        {
            string membertitileid = "";
            GameObject guildmemUI = Instantiate(guildMemberUI, membersContent.transform);
            GuildMemberUI gmui = guildmemUI.GetComponent<GuildMemberUI>();

            // CYCLE THROUGH ALL THE IDS IN EACH MEMBER
            foreach (var mem in member.Members)
            {
                membertitileid = mem.Key.Id;
                string memberDisplayname = mem.Lineage["master_player_account"].Id;
                GetDisplayName(memberDisplayname, gmui.MemberName, member.RoleName);

                //IF MEMBER IS NOT ADMINISTRATOR OF THE GROUP
                if (member.RoleName != "Administrators"
                    && member.RoleName != "Creator")
                {
                    gmui.KickButton.onClick.AddListener(
                    () => {
                        KickMember(EntityKeyMaker(group.Group.Id),
                            EntityKeyMaker(membertitileid));
                        OnViewMembersInOwnGroup(group,
                           r);
                    });
                }
                else
                {
                    gmui.KickButton.enabled = false;
                    gmui.KickButton.interactable = false;
                    gmui.KickButton.gameObject.SetActive(false);
                }

            }
        }
    }

    void GetDisplayName(string playfabid, TextMeshProUGUI membername, string rolename)
    {
        PlayFabClientAPI.GetLeaderboard(
           new GetLeaderboardRequest
           {
               StatisticName = "highscore",
               MaxResultsCount = 10,
           },
        r =>
        {
            for (int item = 0; item < r.Leaderboard.Count; item++)
            {
                if (r.Leaderboard[item].PlayFabId == playfabid)
                {
                    membername.text = $"{r.Leaderboard[item].DisplayName} {rolename}";
                    break;
                }
            }
        }

       , OnSharedError);
    }

    #endregion
    //


    public void InviteToGroup(string groupId, PFGroups.EntityKey entityKey)
    {
        var request = new PFGroups.InviteToGroupRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.InviteToGroup(request,

            result =>
            {
                //cloudScriptManager.SendGroupInvitation(groupId, $"{entityKey}");

            }
            , 
            OnSharedError);
    }



    #region AcceptOrRejectInvitation
    public void AcceptInvitation(string groupId, PFGroups.EntityKey entityKey)
    {
        var request = new PFGroups.AcceptGroupInvitationRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.AcceptGroupInvitation(request, 
            result=>
            {
                var prevRequest = (PFGroups.AcceptGroupInvitationRequest)result.Request;
                EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, prevRequest.Group.Id));
                
                //SAJHASJAS
                //REFRESH JOINED GUILDS PANEL
                ViewJoinedGroups();

            }
            , OnSharedError);
    }


    public void RejectInvitation(string groupId, PFGroups.EntityKey entityKey)
    {
        var request = new PFGroups.RemoveGroupInvitationRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.RemoveGroupInvitation(request,
            result =>
            {
                var prevRequest = (PFGroups.RemoveGroupInvitationRequest)result.Request;
                EntityGroupPairs.Add(new KeyValuePair<string, string>(prevRequest.Entity.Id, prevRequest.Group.Id));
            }
            , OnSharedError);
    }
    #endregion



    #region GetMemberDisplayName
    public void GetDisplayNameForPlayerInGroup(string groupId, string titlePlayerAccountId)
    {
        
    }
    #endregion


    public void ApplyToGroup(string groupId, PFGroups.EntityKey entityKey)
    {
        // A player-controlled entity applies to join an existing group (of which they are not already a member)
        var request = new PFGroups.ApplyToGroupRequest { Group = EntityKeyMaker(groupId), Entity = entityKey };
        PlayFabGroupsAPI.ApplyToGroup(request, 
            result =>
            {
                //var prevRequest = (ApplyToGroupRequest)result.Request;
                //// Presumably, this would be part of a separate process where the recipient reviews and accepts the request
                //var request = new AcceptGroupApplicationRequest { Group = prevRequest.Group, Entity = prevRequest.Entity };

                ////INSTANT ACCEPT
                //PlayFabGroupsAPI.AcceptGroupApplication(request, OnAcceptApplication, OnSharedError);
            }
            , OnSharedError);
    }
    
    public void OnAcceptApplication(PFGroups.EmptyResponse response)
    {
        var prevRequest = (PFGroups.AcceptGroupApplicationRequest)response.Request;
        Debug.Log("Entity Added to Group: " + prevRequest.Entity.Id + " to " + prevRequest.Group.Id);
    }


    public void KickMember(PFGroups.EntityKey groupId, PFGroups.EntityKey entityKey)
    {
        var request = new PFGroups.RemoveMembersRequest { Group = groupId, Members = new List<PFGroups.EntityKey> { entityKey } };
        PlayFabGroupsAPI.RemoveMembers(request, OnKickMembers, OnSharedError);
    }
    private void OnKickMembers(PFGroups.EmptyResponse response)
    {
        var prevRequest = (PFGroups.RemoveMembersRequest)response.Request;

        Debug.Log("Entity kicked from Group: " + prevRequest.Members[0].Id + " to " + prevRequest.Group.Id);
        EntityGroupPairs.Remove(new KeyValuePair<string, string>(prevRequest.Members[0].Id, prevRequest.Group.Id));
    }

    public static PFGroups.EntityKey EntityKeyMaker(string titleId)
    {
        return new PFGroups.EntityKey
        {
            Id = titleId,
            Type = "title_player_account",            
        };
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
}
