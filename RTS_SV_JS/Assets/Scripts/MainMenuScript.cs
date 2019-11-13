using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject optionsPanel;
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

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
