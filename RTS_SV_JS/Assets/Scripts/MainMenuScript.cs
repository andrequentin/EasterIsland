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
    [SerializeField]
    TMP_Text scoreboardParagraph;
    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
    }

    private bool isscorebset = false;
    // Update is called once per frame
    void Update()
    {
        if (!isscorebset && GetNetwork().LoggedGamer != null)
        {
            GetNetwork().SetUpScoreboard(scoreboardParagraph);
            isscorebset = true;
        }
    }

    private NetworkBehavior GetNetwork() { return GameManager._instance.GetComponent<NetworkBehavior>(); }



    //LoginButton Action - Xtralife


    public void Login()
    {
        Debug.Log("Trying to log in with (ID = " + idText.text + ", PW = "+ passwordText.text + " )");

        GetNetwork().Login(idText.text, passwordText.text,this);
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
        GetNetwork().Logout(this);

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
