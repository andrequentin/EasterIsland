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
    TMP_InputField idText;
    [SerializeField]
    TMP_InputField passwordText;
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
    [SerializeField]
    TMP_Text scoreboardParagraph;
    [SerializeField]
    public TMP_Text achievementParagraph;
    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
    }

    private bool isScoreAndAchievementSet = false;
    // Update is called once per frame
    void Update()
    {
        if (GetNetwork().LoggedGamer == null)
        {
            if (!PlayerPrefs.HasKey("GamerId") || !PlayerPrefs.HasKey("GamerSecret"))
            {
                GetNetwork().LoginAnonymously();
            }
            else
            {
                GetNetwork().LoginAnonymouslyWithPlayerPref(PlayerPrefs.GetString("GamerId"), PlayerPrefs.GetString("GamerSecret"));
            }
        }
        if (!isScoreAndAchievementSet && GetNetwork().LoggedGamer != null)
        {
            GetNetwork().SetUpScoreboard(scoreboardParagraph);
            isScoreAndAchievementSet = true;
        }
    }

    private NetworkBehavior GetNetwork() { return GameManager._instance.GetComponent<NetworkBehavior>(); }



    //LoginButton Action - Xtralife


    public void Login()
    {
        Debug.Log("Trying to log in with (ID = " + idText.text + ", PW = "+ passwordText.text + " )");

        GetNetwork().LoginWithMailOrConvert(idText.text, passwordText.text);
    }
    public void LogedIn(string id)
    {
        LoginEntryID.SetActive(false);
        LoginEntryPW.SetActive(false);
        LoginButton.SetActive(false);
        LogoutButton.SetActive(true);
        LogText.SetActive(true);
        Logedtext.GetComponent<TextMeshProUGUI>().text = id;
    }
    public void Logout()
    {
        GetNetwork().Logout();

    }
    public void LogedOut()
    {
        LoginEntryID.SetActive(true);
        LoginEntryPW.SetActive(true);
        LoginButton.SetActive(true);
        LogoutButton.SetActive(false);
        LogText.SetActive(false);
    }
    public void StartGameButton()
    {
        //SceneManager.LoadScene(1);

        paramPanel.SetActive(true);
    }

    public void LaunchGameButton()
    {
        if(GameManager._instance.GetComponent<NetworkBehavior>().LoggedGamer == null)
        {
            GameManager._instance.GetComponent<NetworkBehavior>().LoginAnonymously();
        }
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
