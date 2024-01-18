using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq.Expressions;
//using UnityEditor.PackageManager;

public class SkillboxManager : MonoBehaviour
{
    //public Dictionary<string, string> dataandskill = new Dictionary<string, string>();

    [SerializeField] Skillbox[] skillboxes;

    public string loginscene;

    public void SendJSON(string datatosend)
    {
        List<Skill> skilllist = new List<Skill>();
        foreach(var item in skillboxes)
        {
            skilllist.Add(item.ReturnClass());
        }
        string stringListAsJson = JsonUtility.ToJson(new JSListWrapper<Skill>(skilllist));
        Debug.Log("JSON data prepared: " + stringListAsJson);
        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {datatosend, stringListAsJson }
            }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("Data sent success!"), OnError);
    }
    public void LoadJSON()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataReceived, OnError);
    }
    void OnJSONDataReceived(GetUserDataResult r//, string datatosend
        )
    {
        Debug.Log("received JSON data");
        if(r.Data != null && r.Data.ContainsKey("Skills"))
        {
            Debug.Log(r.Data["Skills"].Value);
            JSListWrapper<Skill> jlw = JsonUtility.FromJson<JSListWrapper<Skill>>(r.Data["Skills"].Value);
            for(int i = 0; i<skillboxes.Length; i++)
            {
                skillboxes[i].SetUI(jlw.list[i]);
            }
        }
    }
    public void OnError(PlayFabError e)
    {
        Debug.Log("Error"+e.GenerateErrorReport());

        //UpdateMsg(ErrMsg, "Error" + e.GenerateErrorReport());
    }
    public void BacktoMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(loginscene);
    }

}



[System.Serializable]
public class JSListWrapper<T>
{
    public List<T> list;
    public JSListWrapper(List<T> list) => this.list = list;
    
}
