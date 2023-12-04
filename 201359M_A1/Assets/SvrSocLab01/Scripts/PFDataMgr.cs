using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PFDataMgr : MonoBehaviour
{
    [SerializeField] TMP_Text XPDisplay;
    [SerializeField] TMP_InputField XPInput;
    public void SetUserData() {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
                {"XP", XPInput.text.ToString()}                
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data XP");
            Debug.Log(error.GenerateErrorReport());
        });
        
    }
    public void GetUserData(/*string myPlayFabId*/) {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            // PlayFabId = myPlayFabId,
            // Keys = null
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("XP")) Debug.Log("No XP");
            else {
                Debug.Log("XP: "+result.Data["XP"].Value);
                XPDisplay.text="XP:"+result.Data["XP"].Value;

            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}
