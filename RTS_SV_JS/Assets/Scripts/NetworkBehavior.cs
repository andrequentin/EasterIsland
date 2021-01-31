using CotcSdk;
using UnityEngine;

public class NetworkBehavior : MonoBehaviour
{

    //Cloud implementation for technical assignement at XtraLife
    [SerializeField]
    public GameObject CotcSdk;
    private CotcGameObject cb;
    public Cloud Cloud;
    public Gamer LoggedGamer;
    [SerializeField]
    private MainMenuScript mms;

    // Start is called before the first frame update
    void Start()
    {
        cb = CotcSdk.GetComponent<CotcGameObject>();
        cb.GetCloud().Done(cloud => { Cloud = cloud; });



    }


    //Called by the GameManager to register win achievement and by Dropoint when ressources are gathered for Collecting ressources acheivement
    public void postAchievementProgression(string unit, int toAdd)
    {
        
        LoggedGamer.Transactions.Post(Bundle.CreateObject(unit, +toAdd))
            .Done(txResult => {
                foreach(var achievement in txResult.TriggeredAchievements)
                {
                    MessageDisplayer._instance.DisplayMessage(achievement.Key);

                }
            });
    }

    //Called by the Gamemanager when game end (post score + extra point if win)
    public void PostScore(int score)
    {
        LoggedGamer.Scores.Domain("private").Post(score, "EasterIslandScoreboard", ScoreOrder.HighToLow,
                "context for score", false)
                .Done(postScoreRes => {
                    Debug.Log("Post score: " + postScoreRes.ToString());
                }, ex => {
                    // The exception should always be CotcException
                    CotcException error = (CotcException)ex;
                    Debug.LogError("Could not post score: " + error.ErrorCode + " (" + error.ErrorInformation + ")");
                });
    }
    //Called by the MainMenuScript when login/Anonymous login is done
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
    //Called by Logins functions (LoginAnonymously, LoginAnonymouslyWithPlayerPref,LoginWithMailOrConvert)
    public void SetUpAchievements(TMPro.TMP_Text achievementPanel)
    {
        achievementPanel.text = "";
        LoggedGamer.Achievements.List().Done(achievements => {
            foreach (var pair in achievements)
            {
                string achName = pair.Key;
                AchievementDefinition ach = pair.Value;

                if (pair.Value.Progress >= 1)
                {
                    achievementPanel.text += pair.Key + '\n';
                }

            }
});
    }
    //Called by MainMenuScript to ensure we are always "logged" (even after a logout) for Scoreboard display
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
                           SetUpAchievements(mms.achievementParagraph);
                       });
    }


    //Called by MainMenuScript to log with saved PlayerPref
    public void LoginAnonymouslyWithPlayerPref(string idGamer, string secretGamer)
    {
        Cloud.Login(
            network: "anonymous",
            networkId: idGamer,
            networkSecret: secretGamer)
                       .Catch(ex2 => {
                           Debug.LogError("Login failed: " + ex2.ToString());
                       }).Done(gamer => {
                           LoggedGamer = gamer;
                           Debug.Log(" Signed anonymously successfully (gamer = " + gamer.ToString() + ")");
                           if (LoggedGamer.Network=="email")
                           {
                               mms.LogedIn(LoggedGamer.NetworkId);
                           }
                           SetUpAchievements(mms.achievementParagraph);
                       });
    }



    //Called by MainMenuScript when login button is used
    public void LoginWithMailOrConvert(string idGamer, string idPass)
    {
        //The user is always loged in anonymously on start, so if he login it's either a new account so we convert to an account

        //or if the account is already created we just login
                   Cloud.Login(
                    network: "email",
                    networkId: idGamer,
                    networkSecret: idPass)
                      .Done(gamer => {
                          LoggedGamer = gamer;
                          mms.LogedIn(LoggedGamer.NetworkId);
                          PlayerPrefs.SetString("GamerId", LoggedGamer.GamerId);
                          PlayerPrefs.SetString("GamerSecret", LoggedGamer.GamerSecret);
                          SetUpAchievements(mms.achievementParagraph);

                      }, ex2 => {
                          Debug.LogError("Login failed: " + ex2.ToString());
                      });
    }


    //Called by MainMenuScript when logout button is used
    public void Logout()
    {
        Cloud.Logout(LoggedGamer).Done(result =>
        {
            Debug.Log("Logout succeeded");
            mms.LogedOut();
            PlayerPrefs.DeleteKey("GamerId");
            PlayerPrefs.DeleteKey("GamerSecret");
            LoggedGamer = null; 
        }, ex =>
        {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to logout: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
    }


    

    
}
