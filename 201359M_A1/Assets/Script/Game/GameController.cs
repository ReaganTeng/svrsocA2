using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
//using UnityEditor.PackageManager;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using PlayFab.MultiplayerModels;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    public GameObject gameOverObject;
    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalHighScoreText;
    public int difficultyMax = 5;


    public Slider healthbar;


    [HideInInspector]
    public bool isGameOver = false;
    public float scrollSpeed = -5f;

    private float score = 0;
    private int highestScore = 0;

    public Spawner spawner;


    public string TownScene;

    //THE TIME LASTED DURING THE GAME
    public float timelasted;
    public float longestTime;

    public int lives;



    public TextMeshProUGUI timerText;


    public GameObject playfab;

    JSListWrapper<Skill> jlw;

    bool resultsent;

    private void Awake()
    {
        resultsent = false;

        timelasted = 0;

        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        LoadHighScore();

        LoadLongestTime();

        scoreText.text = "0";
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOverObject.SetActive(false);
        //InvokeRepeating("AddScore", 0.25f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        //REAGAN SEND UPDATE
        if (!isGameOver)
        {
            timelasted += 1 * Time.deltaTime;

            AddScore();

            int minutes = Mathf.FloorToInt(timelasted / 60);
            int seconds = Mathf.FloorToInt(timelasted % 60);

            string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
            timerText.text = timeText;

            //if (timelasted >= longestTime)
            //{
            //    SaveLongestTime(longestTime);
            //}

            if (timelasted % 3 <= Time.deltaTime)
            {
                //INCREASE MONEY
                var buyreq = new AddUserVirtualCurrencyRequest
                {
                    VirtualCurrency = "RD",
                    Amount = 10 * jlw.list[2].level,
                };
                PlayFabClientAPI.AddUserVirtualCurrency(buyreq,
                    r =>
                    {
                        Debug.Log("Money Added");
                    }, OnError);
            }
        }


        
        //if (timelasted >= 10)
        //{
        //    GameOver();
        //}
    }

    void AddScore()
    {
        int multiplier = 1;

        if (jlw != null)
        {
            Debug.Log("JLW NAME " + jlw.list[1].name + jlw.list[1].level);
            multiplier = jlw.list[1].level;
        }

        score += 10 * Time.deltaTime * 0.25f * multiplier;
        scoreText.text = ((int)score).ToString();

        //Check and update the level
        CheckLevel();

        //if ((int)score >= highestScore)
        //{
        //    SaveHighScore((int)score);
        //}
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();

        //Check and update the level
        CheckLevel();

        //if ((int)score >= highestScore)
        //{
        //    SaveHighScore((int)score);
        //}
    }

    void CheckLevel()
    {
        //500 to 1000
        if (score > 500 && score < 1000)
        {
            spawner.SetLevel(1);
            scrollSpeed = -6;
        }
        else if (score > 1000 && score < 2500)
        {
            spawner.SetLevel(2);
            scrollSpeed = -7.5f;
        }
        else if (score > 2500)
        {
            spawner.SetLevel(3);
            scrollSpeed = -10.0f;
        }
    }

    public void ResetGame()
    {
        //Reset the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToLanding(int currencytype)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest(),
            r => {
                //RM, RD
                int coins = r.VirtualCurrency[currencytype];
                //UpdateMsg(ref shopText, "Coins: " + coins);
            }
            , playfab.GetComponent<PlayFabGameMgt>().OnError);


        //SceneManager.LoadScene("Landing");

        SceneManager.LoadScene(TownScene);
    }



    public void LoadJSON()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONDataReceived, OnError);
    }
    void OnJSONDataReceived(GetUserDataResult r)
    {
        //for (int x = 0; x < skillboxes.Length; x++)
        //{
        //Debug.Log("received JSON data");
        if (r.Data != null
            && r.Data.ContainsKey("Skills")
            //&& r.Data.ContainsKey(skillboxes[x].name)
            )
        {
            //Debug.Log(r.Data[skillboxes[x].name].Value);
            //Debug.Log(r.Data["Skills"].Value);
            //GET FROM JSON
            jlw = JsonUtility.FromJson<JSListWrapper<Skill>>(r.Data["Skills"].Value);

            //0 - lives
            //1 - scoremultiplier
            //2 - rdmultiplier
            //Debug.Log($"JLW {jlw.list[1].name}");

            //set lives
            lives = jlw.list[0].level;
            healthbar.maxValue = lives;
            healthbar.minValue = 0;
            healthbar.value = lives;
        }


       

        //}
    }


    public void OnError(PlayFabError e)
    {
        //UpdateMsg(ErrMsg, "Error" + e.GenerateErrorReport());
        Debug.Log("Error" + e.GenerateErrorReport());
    }

    public void GameOver()
    {
        gameOverObject.SetActive(true);
        isGameOver = true;
        CancelInvoke();

        finalScoreText.text = score.ToString();
        finalHighScoreText.text = highestScore.ToString();

        if (score >= highestScore)
        {
            Debug.Log("UPDATE SCORE");
            playfab.GetComponent<PlayFabGameMgt>().OnButtonSendLeaderBoard("highscore", score);
            SaveHighScore((int)score);
        }

        if (timelasted >= longestTime)
        {
            Debug.Log("UPDATE TIME");
            playfab.GetComponent<PlayFabGameMgt>().OnButtonSendLeaderBoard("Time", timelasted);
            SaveLongestTime((int)timelasted);
        }

        //REAGAN send score and time
        if (!resultsent)
        {
            

            resultsent = true;
        }

    }

    private void SaveHighScore(int score)
    {


        //highestScore = score;

        SendJSON("HighestScore", score);

        //PlayerPrefs.SetInt("highestScore", highestScore);
        //highScoreText.text = highestScore.ToString();
    }

    private void LoadHighScore()
    {
        //SendJSON("HighestScore", 0);        
        LoadHSJSON();  

        //Debug.Log("STAT " + r.Statistics[0].Value);
        //if(PlayerPrefs.HasKey("highestScore"))
        //{
        //    highestScore = PlayerPrefs.GetInt("highestScore");

        //    //highScoreText.text = highestScore.ToString();
        //}
    }



    private void SaveLongestTime(float time)
    {
        SendJSON("HighestTime", time);
    }

    public void SendJSON(string datatosend, float data)
    {
        //string stringListAsJson = JsonUtility.ToJson(data);
        //Debug.Log("JSON data prepared: " + stringListAsJson);
        var req = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {datatosend, ((int)data).ToString() }
            }
        };
        PlayFabClientAPI.UpdateUserData(req, result => Debug.Log("Data sent success!"), OnError);
    }




    //FOR HIGH SCORE
    void LoadHSJSON()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONHSDataReceived, OnError);
    }
    void OnJSONHSDataReceived(GetUserDataResult r)
    {
        //for (int x = 0; x < skillboxes.Length; x++)
        //{
        //Debug.Log("received JSON data");
        if (!r.Data.ContainsKey("HighestScore")
            //&& r.Data.ContainsKey(skillboxes[x].name)
            )
        {
            SendJSON("HighestScore", 0);
            highestScore = 0;
        }
        else
        {
            highestScore = int.Parse(r.Data["HighestScore"].Value);
        }
        //}
    }
    //


    //FOR TIME
    public void LoadTimeJSON()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnJSONTimeDataReceived, OnError);
    }
    void OnJSONTimeDataReceived(GetUserDataResult r)
    {
        //for (int x = 0; x < skillboxes.Length; x++)
        //{
        //Debug.Log("received JSON data");
        if (!r.Data.ContainsKey("HighestTime")
            //&& r.Data.ContainsKey(skillboxes[x].name)
            )
        {
            SendJSON("HighestTime", 0);
            longestTime = 0;
        }
        else
        {
            Debug.Log("TIME LOADED");
            longestTime = int.Parse(r.Data["HighestTime"].Value);
        }
        //}
    }
    //





    private void LoadLongestTime()
    {

        LoadTimeJSON();

        //if (PlayerPrefs.HasKey("longestTime"))
        //{
        //    longestTime = PlayerPrefs.GetFloat("longestTime");
        //}
        //else
        //{
        //    longestTime = 0;
        //}
    }
}
