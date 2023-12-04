using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using TMPro;

public class PlayFabGameMgt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score, timer;


    string nametodisplay;

    ///DATA
    public void SetUserData(string DataType)
    {
        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {DataType, score.text.ToString()},
                }
            },
            result => Debug.Log("Successfully updated user data"),
            error => {
                Debug.Log("Got error setting user data XP");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(
        new GetUserDataRequest()
        {

        },
        result =>
        {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("XP"))
            {
                Debug.Log("No XP");
            }
            else
            {
                Debug.Log("XP: " + result.Data["XP"].Value);
                score.text = "XP: " + result.Data["XP"].Value;
            }
        },
        error =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        }

        );
    }
    ///



    public void OnError(PlayFabError e)
    {
        //UpdateMsg(ErrMsg, "Error" + e.GenerateErrorReport());
        Debug.Log("Error" + e.GenerateErrorReport());
    }
    public void UpdateMsg(Text text, string msg)
    { //to display in console and messagebox
        Debug.Log(msg);
        //text.text = msg;
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

        if (hasNoLetters)
        {
            Debug.Log("The string has no letters.");
        }
        else
        {
            Debug.Log("The string contains at least one letter.");
        }

        return hasNoLetters;
    }


    ///LEADERBOARD
    //public void OnButtonGetLeaderboard()
    //{
    //    var lbreq = new GetLeaderboardRequest
    //    {
    //        StatisticName = "highscore",
    //        StartPosition = 0,
    //        MaxResultsCount = 10,
    //    };
    //    PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);

    //    //UpdateMsg(LeaderboardText, LeaderboardStr);
    //}

    bool namedecided = false;
    void GetLBName(GetAccountInfoResult r)
    {
        nametodisplay = "NIL";
        if (r != null
            && r.AccountInfo.Username != null)
        {
            nametodisplay = r.AccountInfo.Username;
            Debug.Log("Username is " + nametodisplay + " " + r.AccountInfo.PlayFabId);
        }
    }
    string LeaderboardStr;

    //ONLEADERBOARDGET NEW
    void UpdateLB(GetLeaderboardResult r)
    {
        LeaderboardStr = "Leaderboard\n";

        // Use a counter to keep track of how many callbacks have completed
        int completedCallbacks = 0;

        foreach (var item in r.Leaderboard)
        {
            var req = new GetAccountInfoRequest
            {
                PlayFabId = item.PlayFabId
            };

            // Use a lambda function to capture the item variable
            PlayFabClientAPI.GetAccountInfo(req, (result) =>
            {
                GetLBName(result);

                // Increment the completedCallbacks count
                completedCallbacks++;

                // Check if all callbacks have completed
                if (completedCallbacks == r.Leaderboard.Count)
                {
                    // All callbacks have completed, so you can now update the UI or log the information
                    Debug.Log("NAME TO DISPLAY " + item.PlayFabId + " " + nametodisplay);
                }
            }, OnError);

            if (item.DisplayName != null)
            {
                if (!hasNoletters(item.DisplayName))
                {
                    nametodisplay = item.DisplayName;
                    Debug.Log("USE DISPLAY NAME");
                }
            }

            string onerow = item.Position + "-" + nametodisplay + "-" + item.StatValue + "\n";
            LeaderboardStr += onerow;
        }
    }

    void OnLeaderboardGet(GetLeaderboardResult r)
    {
        UpdateLB(r);

    }
    public void OnButtonSendLeaderBoard(string statistic, float data)
    {
        

        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statistic,
                    //StatisticName = "Time",
                    Value = int.Parse(((int)data).ToString()),
                }
            }
        };
        //UpdateMsg(Msg, "Submitting score:" + currentScore.text);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnLeaderboardUpdate, OnError);
        //ClearFields();
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        //UpdateMsg(Msg, "Successful leaderboard sent:" + r.ToString());
        Debug.Log("Successful leaderboard sent:" + r.ToString());
    }
    ///


    ///INVENTORY
    public void GetPlayerInventory()
    {
        var UserInv = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(UserInv, r =>
        {
            List<ItemInstance> ii = r.Inventory;
            //UpdateMsg(ref nullText, "Player Inventory");
            foreach (ItemInstance item in ii)
            {

                
                //UpdateMsg(ref nullText, item.DisplayName + ", "
                //    + item.ItemId + ", " + item.ItemInstanceId);
            }
        }
        , OnError);


        //PlayFabClientAPI.Inven
    }
    ///

}





