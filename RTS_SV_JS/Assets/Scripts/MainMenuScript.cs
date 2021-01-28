using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using CotcSdk;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject optionsPanel;

    [SerializeField]
    GameObject paramPanel;

    //Login parameter - Xtralife
    [SerializeField]
    TMP_Text idText;
    [SerializeField]
    TMP_Text passwordText;
    [SerializeField]
    TMP_Text Logedtext;
    [SerializeField]
    GameObject LoginEntryID;
    [SerializeField]
    GameObject LoginEntryPW;
    [SerializeField]
    GameObject LoginButton;
    [SerializeField]
    GameObject LogText;
    [SerializeField]
    GameObject LogoutButton;
    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //LoginButton Action - Xtralife
    public void Login()
    {
        string idGamer = idText.text;
        string idPass = passwordText.text;

        Debug.Log("Trying to log in with (ID = " + idGamer + ", PW = "+ idPass+ " )");
        GameManager._instance.Cloud.Login(
           network: "email",
           networkId: idGamer,
           networkSecret: idPass)
               .Done(gamer => {
                   GameManager._instance.LoggedGamer = gamer;
                   Debug.Log("Signed in succeeded (ID = " + gamer.GamerId + ")");
                   Debug.Log("Login data: " + gamer);
                   Debug.Log("Server time: " + gamer["servertime"]);
                   LoginEntryID.SetActive(false);
                   LoginEntryPW.SetActive(false);
                   LoginButton.SetActive(false);
                   LogoutButton.SetActive(true);
                   LogText.SetActive(true);
                   Logedtext.GetComponent<TextMeshProUGUI>().text = GameManager._instance.LoggedGamer.NetworkId;

               }, ex => {
                   GameManager._instance.Cloud.LoginAnonymously()
                       .Catch(ex2 => {
                           Debug.LogError("Login failed: " + ex2.ToString());
                       }).Done(gamer => {
                           GameManager._instance.LoggedGamer = gamer;
                           Debug.Log("Login failed, Signed anonymously successfully (ID = " + gamer.GamerId + ")");
                           LoginEntryID.SetActive(false);
                           LoginEntryPW.SetActive(false);
                           LoginButton.SetActive(false);
                           LogoutButton.SetActive(true);
                           LogText.SetActive(true);
                           Logedtext.GetComponent<TextMeshProUGUI>().text = GameManager._instance.LoggedGamer.GamerId;

                       });

               });

        

    }
    public void Logout()
    {
        GameManager._instance.Cloud.Logout(GameManager._instance.LoggedGamer).Done(result =>
        {
            Debug.Log("Logout succeeded");
            LoginEntryID.SetActive(true);
            LoginEntryPW.SetActive(true);
            LoginButton.SetActive(true);
            LogoutButton.SetActive(false);
            LogText.SetActive(false);
        }, ex =>
        {
            // The exception should always be CotcException
            CotcException error = (CotcException)ex;
            Debug.LogError("Failed to logout: " + error.ErrorCode + " (" + error.HttpStatusCode + ")");
        });
       
    }

    public void StartGameButton()
    {
        //SceneManager.LoadScene(1);
        paramPanel.SetActive(true);
    }

    public void LaunchGameButton()
    {
        SceneManager.LoadScene(1);
    }

    public void CancelStartGameButton()
    {
        paramPanel.SetActive(false);
    }

    public void OptionsButton()
    {
        optionsPanel.SetActive(true);
    }

    public void QuitOptionsButton()
    {
        optionsPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("myVolume", volume);
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
