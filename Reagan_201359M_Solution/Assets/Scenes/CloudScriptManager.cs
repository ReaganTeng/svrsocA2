using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.GroupsModels;
using PlayFab.Json;
using System.Linq;




[Serializable]
public class PlayerInfo
{
    public string playFabId;
    public string titlePlayerAccountId;
    public string displayName;

    public PlayerInfo(string playFabId, string titlePlayerAccountId, string displayName)
    {
        this.playFabId = playFabId;
        this.titlePlayerAccountId = titlePlayerAccountId;
        this.displayName = displayName;
    }
}


public class CloudScriptManager : MonoBehaviour
{
     List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

    private void Awake()
    {
        // Fetch playerInfoList from cloudscript when the game starts
        //GetPlayerInfoList();
    }

    // Function to fetch playerInfoList from cloudscript
    private void GetPlayerInfoList()
    {
        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest
            {
                FunctionName = "GetPlayerInfoList"
            },
            result =>
            {
                if (result.FunctionResult is Dictionary<string, object> functionResultDict)
                {
                    // Process retrieved playerInfoList
                    object playerInfoListObject;
                    if (functionResultDict.TryGetValue("PlayerInfoList", out playerInfoListObject))
                    {
                        playerInfoList = PlayFabSimpleJson.DeserializeObject<List<PlayerInfo>>(playerInfoListObject.ToString());
                        Debug.Log("PlayerInfoList fetched successfully.");
                    }
                    else
                    {
                        Debug.LogError("PlayerInfoList key not found in CloudScript response.");
                    }
                }
                else
                {
                    Debug.LogError("Invalid format for CloudScript response. Result: " + PlayFabSimpleJson.SerializeObject(result));
                }
            },
            error =>
            {
                Debug.LogError("Failed to get PlayerInfoList. Error: " + error.GenerateErrorReport());
            }
        );
    }
    // Function to check if a player is in the cloudscript list
    public bool IsPlayerInCloudScriptList(string playFabId)
    {
        return playerInfoList.Any(player => player.playFabId == playFabId);
    }

    // Function to add a player to the cloudscript list
    public void AddPlayerToCloudScriptList(string playFabId, string tpaid, string dm)
    {

        // Assuming you have values for playFabId, titlePlayerAccountId, and displayName
        string playFabIdValue = playFabId;
        string titlePlayerAccountIdValue = tpaid;
        string displayNameValue = dm;

        // Add a new PlayerInfo object with the provided values
        playerInfoList.Add(new PlayerInfo(playFabIdValue, titlePlayerAccountIdValue, displayNameValue));

        //playerInfoList.Add(new PlayerInfo
        //{ playFabId = playFabId, 
        //   titlePlayerAccountId = "", 
        //   displayName = "" }
        //);
    }



    public void SendGroupInvitation(string groupId, string targetPlayerId)
    {
        var cloudScriptParams = new { groupId, targetPlayerId };

        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest
            {
                FunctionName = "SendGroupInvitation",
                FunctionParameter = cloudScriptParams
            },
            result =>
            {
                Debug.Log("Invitation sent successfully!");
                // After sending the invitation, fetch the updated list of invitations
                GetGroupInvitations();
            },
            error =>
            {
                Debug.LogError("Failed to send invitation: " + error.ErrorMessage);
            }
        );
    }

    public void GetGroupInvitations()
    {
            PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest
            {
                FunctionName = "GetGroupInvitations"
            },
            result =>
            {
                //Debug.Log(result.FunctionResult);
                if (result.FunctionResult is Dictionary<string, object> functionResultDict)
                {
                    // Process retrieved invitations
                    object invitationsObject;
                    if (functionResultDict.TryGetValue("Invitations", out invitationsObject))
                    {
                        List<GroupInvitation> invitations = PlayFabSimpleJson.DeserializeObject<List<GroupInvitation>>(invitationsObject.ToString());
                        // Handle invitations
                        if (invitations != null && invitations.Count > 0)
                        {
                            foreach (var invitation in invitations)
                            {
                                Debug.Log($"Received invitation from {invitation.InvitedByEntity.Key.Id} to group {invitation.Group.Id}");
                            }
                        }
                        else
                        {
                            Debug.Log("No invitations found.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Invitations key not found in CloudScript response.");
                    }
                }
                else
                {
                    Debug.LogError("Invalid format for CloudScript response. Result: " + PlayFabSimpleJson.SerializeObject(result));
                }
            },
            error =>
            {
                Debug.LogError("Failed to get invitations. Error: " + error.GenerateErrorReport());
            }
        );
    }


   
}
