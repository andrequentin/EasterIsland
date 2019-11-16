using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject optionsPanel;

    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
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
