using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
//using PlayFab.SharedModels;
//using ExitGames.Client.Photon.StructWrapping;
//using System.Threading.Tasks;
//using UnityEditor.PackageManager.Requests;
//using System;
//using static UnityEditor.Progress;
//using System.Threading;

public class PlayFabUserMgt : MonoBehaviour
{
    [SerializeField] public InputField userEmail, userPassword, userName, displayName, currentScore, XP_Input, displayName_postRegister
        , userEmailPasswordReset;
    [SerializeField]public Text Msg, XPDisplay, UserText, ErrMsg, LeaderboardText, PasswordResetEmailText;
    [SerializeField] public string LoginScene, GameScene, ShopScene;

    
    [SerializeField] GameObject messagePanel, LoginRegPanel, UserPanel, displaynameupdatepanel, passwordresetpanel;


    //NAME TO DISPLAY IN LEADERBOARD
    string nametodisplay = "NIL";

    float resetpasswordmessage_timer;


    void Awake()
    {
        resetpasswordmessage_timer = 0.0f;
        messagePanel.SetActive(false);

        //CHECK IF USER IA ALREADY LOGGED IN
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            UserPanel.SetActive(true);
            LoginRegPanel.SetActive(false);

        }
        else
        {
            UserPanel.SetActive(false);
            LoginRegPanel.SetActive(true);
            //InstantRegister();
            //InstantLogin();
        }
        
    }


    //FOR TESTING PURPOSES
    void InstantLogin()
    {
        //userEmail.text = "yuzhe100903@gmail.com";
        userName.text = "reagan";

        userPassword.text = "1234567890";

        //OnButtonLogin();

        //SceneManager.LoadScene(ShopScene);
        //SceneManager.LoadScene(GameScene);
    }
    //

    void InstantRegister()
    {
        userEmail.text = "yuzhe100903@gmail.com";
        userName.text = "Reagan";
        userPassword.text = "1234567890";

        OnButtonRegUser();

    }


    public void closeMessagePanel()
    {
        messagePanel.SetActive(false);
    }


    public void OnError(PlayFabError e){
        //Debug.Log("Error"+e.GenerateErrorReport());
        messagePanel.SetActive(true);

        UpdateMsg(ErrMsg, "Error"+e.GenerateErrorReport());
    }
    public void UpdateMsg(Text text,string msg){ //to display in console and messagebox
        Debug.Log(msg);
        text.text=msg;
        //text.text = msg;
    }

    ///FOR PASSWORD RESET
    public void PasswordResetRequest()
    {
        var req = new SendAccountRecoveryEmailRequest
        {
            Email = userEmailPasswordReset.text,
            TitleId = PlayFabSettings.TitleId //no need hardcode!
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(req, OnPasswordReset, OnError);


    }
    void OnPasswordReset(SendAccountRecoveryEmailResult r)
    {
        Msg.text = "An email request has beent sent to you";
        PasswordResetEmailText.text = "An email request has beent sent to you";
        resetpasswordmessage_timer = 2.0f;
        //togglepasswordresetpanel();
        ClearFields();
    }
    /// 


    public void Update()
    {
        if(resetpasswordmessage_timer > 0)
        {
            resetpasswordmessage_timer -= 1 * Time.deltaTime;
            if(resetpasswordmessage_timer <= 0)
            {
                PasswordResetEmailText.text = "Enter Your Email";
            }
        }
        //if
    }


    public void ClearFields()
    {
        userEmail.text = "";
        userPassword.text = "";
        userName.text = "";
        displayName.text = "";
        currentScore.text = "";
        XP_Input.text = "";
        displayName_postRegister.text = "";
    }


    public void startGame()
    {
        SceneManager.LoadScene(ShopScene);
    }





    /// REGISTRATION
    //WHEN USER CLICKS REGISTER BUTTON
    public void OnButtonRegUser()
    { //for button click
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = userEmail.text,
            Password = userPassword.text,
            Username = userName.text
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegSuccess, OnError);
    }
    void OnRegSuccess(RegisterPlayFabUserResult r)
    { //callback function if success

        string name = r.Username;

        //IF DISPLAYNAME HAS NO LETTERS, THEN USERNAME WILL BECOME THE USER'S DISPLAYNAME
        if (!hasNoletters(displayName.text))
        {
            name = displayName.text;
        }
        UpdateMsg(Msg, "DisplayName Updated!");
        var req = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);

        ClearFields();
    }
    public void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult r)
    {
        UpdateMsg(Msg, "display name updated!" + r.DisplayName);
        ClearFields();
        toggledisplaynameupdatepanel();
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
            UnityEngine.Debug.Log("The string has no letters.");
        }
        else
        {
            UnityEngine.Debug.Log("The string contains at least one letter.");
        }

        return hasNoLetters;
    }

    
    ///


    public void toggledisplaynameupdatepanel()
    {
        displaynameupdatepanel.SetActive(!displaynameupdatepanel.activeSelf);
    }

    public void togglepasswordresetpanel()
    {
        PasswordResetEmailText.text = "Enter Your Email";
        ClearFields();
        passwordresetpanel.SetActive(!passwordresetpanel.activeSelf);
    }



    //SPECIFICALLY WHEN USER WANTS TO UPDATE THEIR NAME
    public void OnButtonUpdateDisplayName()
    {
        string name = "";

        if (!hasNoletters(displayName_postRegister.text))
        {
            name = displayName_postRegister.text;
            UpdateMsg(Msg, "DisplayName Updated!");
            var req = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = name,
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(req, OnDisplayNameUpdate, OnError);
        }
        else
        {
            PasswordResetEmailText.text = "PLEASE INPUT DISPLAY NAME";
            //Debug.Log("PLEASE INPUT DISPLAY NAME");
        }
    }



    ///LOGIN <summary>
  
    //user can either login with username or email
    public void OnButtonLogin()
    {
        if(userEmail.text != "")
        {
            OnButtonLoginEmail();
        }
        else
        {
            OnButtonLoginUserName();
        }
    }

    void OnLoginSuccess(LoginResult r )
    {
        UpdateMsg(Msg, "Login Success!");
        ClearFields();
        UserPanel.SetActive(true);

        //Debug.Log("LoginResult: " + r.PlayFabId);

        var req = new GetAccountInfoRequest
        {
            PlayFabId = r.PlayFabId
        };

        PlayFabClientAPI.GetAccountInfo(req, GetUserName, OnError);

        ClearFields();
        LoginRegPanel.SetActive(false);
    }

    void GetUserName(GetAccountInfoResult r)
    {
        string username = "WELCOME ";
        UserText.text = username;

        if (r != null
            && r.AccountInfo.Username != null)
        {
            UserText.text += r.AccountInfo.Username;
            //r.AccountInfo = "gos";
            //Debug.Log("Username is " + r.AccountInfo.Username);
        }
    }
    



    



    //LOGIN WITH EMAIL
    void OnButtonLoginEmail()
    {
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = userEmail.text,
            Password = userPassword.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnError);
    }
   
    void OnButtonLoginUserName()
    {
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = userName.text,
            Password = userPassword.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(loginRequest, OnLoginSuccess, OnError);

    }
    public void OnButtonLogout()
    {
        PlayFabClientAPI.ForgetAllCredentials(); //clear all login credentials
        Debug.Log("logged out");
        if (LoginScene != null)
        {
            SceneManager.LoadScene(LoginScene);
        }
    }
    public void OnButtonDeviceLogin()
    { //login with device id
        var req0 = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(req0, OnLoginSuccess, OnError);
        //SceneManager.LoadScene(GameScene);
    }
    ///



    //REAGA



    ///DATA
    public void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(
            new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {"XP", XP_Input.text.ToString()},
                }
            },
            result => Debug.Log("Successfully updated user data"),
            error => {
                Debug.Log("Got error setting user data XP");
                Debug.Log(error.GenerateErrorReport());
            }
        );
        ClearFields();
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
                XPDisplay.text = "XP: " + result.Data["XP"].Value;
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



   


    ///LEADERBOARD
    public void OnButtonGetLeaderboard()
    {
        var lbreq = new GetLeaderboardRequest
        {
            StatisticName = "highscore",
            StartPosition = 0,
            MaxResultsCount = 10,
        };
        PlayFabClientAPI.GetLeaderboard(lbreq, OnLeaderboardGet, OnError);

        UpdateMsg(LeaderboardText, LeaderboardStr);
    }



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




    //void OnLeaderboardGet(GetLeaderboardResult r)
    //{
    //    string LeaderboardStr = "Leaderboard\n";
    //    foreach (var item in r.Leaderboard)
    //    {
    //        var req = new GetAccountInfoRequest
    //        {
    //            PlayFabId = item.PlayFabId
    //        };            
    //        PlayFabClientAPI.GetAccountInfo(req, GetLBName, OnError);


    //        if (item.DisplayName != null)
    //        {
    //            if (!hasNoletters(item.DisplayName))
    //            {
    //                nametodisplay = item.DisplayName;
    //                Debug.Log("USE DISPLAYNAME");
    //            }
    //        }
    //        Debug.Log("NAME TO DISPLAY " + item.PlayFabId + " " + nametodisplay);

    //        string onerow = item.Position + "-" +
    //            nametodisplay + "-" + item.StatValue + "\n";
    //        LeaderboardStr += onerow;
    //    }
    //    UpdateMsg(LeaderboardText, LeaderboardStr);
    //}

    //void GetLBName(GetAccountInfoResult accountInfo)
    //{
    //    nametodisplay = "NIL";
    //    if (accountInfo != null && accountInfo.AccountInfo.Username != null)
    //    {
    //        nametodisplay = accountInfo.AccountInfo.Username;
    //        Debug.Log("Username is " + nametodisplay + " " + accountInfo.AccountInfo.PlayFabId);

    //        // After getting the account info, you can update the leaderboard here
    //        UpdateLeaderboard();
    //    }
    //}

    //void UpdateLeaderboard()
    //{
    //    string LeaderboardStr = "Leaderboard\n";
    //    foreach (var item in leaderboardData) // Use the correct leaderboard data
    //    {
    //        // Retrieve the account info here if needed

    //        if (item.DisplayName != null)
    //        {
    //            if (!hasNoletters(item.DisplayName))
    //            {
    //                nametodisplay = item.DisplayName;
    //                Debug.Log("USE DISPLAYNAME");
    //            }
    //        }

    //        Debug.Log("NAME TO DISPLAY " + item.PlayFabId + " " + nametodisplay);

    //        string onerow = item.Position + "-" + nametodisplay + "-" + item.StatValue + "\n";
    //        LeaderboardStr += onerow;
    //    }

    //    // Update the leaderboard text
    //    UpdateMsg(LeaderboardText, LeaderboardStr);
    //}

    //void OnLeaderboardGet(GetLeaderboardResult leaderboardResult)
    //{
    //    // Store the leaderboard data for later use
    //    leaderboardData = leaderboardResult.Leaderboard;

    //    // Call the GetLBName method with the retrieved account info
    //    GetLBName(accountInfo);
    //}

    //// Define a variable to store leaderboard data
    //private List<PlayerLeaderboardEntry> leaderboardData = new List<PlayerLeaderboardEntry>();
    //// Define a variable to store account info
    //private GetAccountInfoResult accountInfo = null;

    public void OnButtonSendLeaderBoard()
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "highscore",
                    Value = int.Parse(currentScore.text)
                }
            }
        };
        UpdateMsg(Msg,"Submitting score:" + currentScore.text);
        PlayFabClientAPI.UpdatePlayerStatistics(req, OnLeaderboardUpdate, OnError);
        ClearFields();
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult r)
    {
        UpdateMsg(Msg, "Successful leaderboard sent:" + r.ToString());
    }
    ///



    //public void StartGame()
    //{
    //    SceneManager.LoadScene(GameScene);

    //}




    //REVEAL PASSWORD - DECIDE IF USER WANTS TO * THEIR PASSWORD OR NOT
    public void OnRevealPasswordClicked()
    {

        if (userPassword.contentType != InputField.ContentType.Standard)
        {
            //Debug.Log("REVEAL PASSWORD");
            userPassword.contentType = InputField.ContentType.Standard;
        }
        else if (userPassword.contentType != InputField.ContentType.Password)
        {
            //Debug.Log("HIDE PASSWORD");
            userPassword.contentType = InputField.ContentType.Password;
        }

        // Force the input field to refresh its content
        userPassword.ForceLabelUpdate();
    }

}





