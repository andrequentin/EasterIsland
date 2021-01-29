using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CotcSdk;

public class NetworkBehavior : MonoBehaviour
{

    //Cloud implementation for technical assignement at XtraLife
    [SerializeField]
    public GameObject CotcSdk;
    private CotcGameObject cb;
    public Cloud Cloud;
    public Gamer LoggedGamer;


    // Start is called before the first frame update
    void Start()
    {
        cb = CotcSdk.GetComponent<CotcGameObject>();
        cb.GetCloud().Done(cloud => { Cloud = cloud; });



    }
    public void PostScore(int score)
    {
        LoggedGamer.Scores.Domain("private").Post(score, "EasterIslandScoreboard", ScoreOrder.HighToLow,
                "context for score", true)
                .Done(postScoreRes => {
                    Debug.Log("Post score: " + postScoreRes.ToString());
                }, ex => {
                    // The exception should always be CotcException
                    CotcException error = (CotcException)ex;
                    Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
                });
    }
    public void SetUpScoreboard(TMPro.TMP_Text scoreboard)
    {
        scoreboard.text = "";
        LoggedGamer.Scores.Domain("private").BestHighScores("EasterIslandScoreboard", 10, 1)
        .Done(bestHighScoresRes => {
            foreach (var score in bestHighScoresRes)
            {
                scoreboard.text += score.Rank + ". " + score.GamerInfo["profile"]["displayName"] + ": " + score.Value + '\n';
                Debug.Log(score.Rank + ". " + score.GamerInfo["profile"]["displayName"] + ": " + score.Value);
            }
        }, ex => {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Could not get best high scores: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
        });
    }

    public void LoginAnonymously()
    {
        Cloud.LoginAnonymously()
                       .Catch(ex2 => {
                           Debug.LogError("Login failed: " + ex2.ToString());
                       }).Done(gamer => {
                           LoggedGamer = gamer;
                           PlayerPrefs.SetString("GamerId", LoggedGamer.GamerId);
                           PlayerPrefs.SetString("GamerSecret", LoggedGamer.GamerSecret);
                           Debug.Log(" Signed anonymously successfully (ID = " + gamer.GamerId + ")");
                       });
    }
    public void LoginAnonymously(string idGamer, string secretGamer)
    {
        Cloud.Login(
            network: "anonymous",
            networkId: idGamer,
            networkSecret: secretGamer)
                       .Catch(ex2 => {
                           Debug.LogError("Login failed: " + ex2.ToString());
                       }).Done(gamer => {
                           LoggedGamer = gamer;
                           Debug.Log(" Signed anonymously successfully (ID = " + gamer.GamerId + ")");
                       });
    }
    public void Login(string idGamer, string idPass,MainMenuScript mms)
    {
        LoggedGamer.Account.Convert(
           network: "email",
           networkId: idGamer,
           networkSecret: idPass)
               .Done(     convertRes => {
                   Debug.Log("Convert succeeded: " + convertRes.ToString());
                   mms.LogedIn(LoggedGamer.NetworkId);
                   Debug.Log("Signed in succeeded (ID = " + LoggedGamer.GamerId + ")");
                   Debug.Log("Login data: " + LoggedGamer);
                   Debug.Log("Server time: " + LoggedGamer["servertime"]);
               }, ex => {
                   Debug.LogError("Login failed: " + ex.ToString());
               });
    }
    public void Logout(MainMenuScript mms)
    {
        Cloud.Logout(LoggedGamer).Done(result =>
        {
            Debug.Log("Logout succeeded");
            mms.LogedOut();

        }, ex =>
        {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to logout: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }


    

    
}
