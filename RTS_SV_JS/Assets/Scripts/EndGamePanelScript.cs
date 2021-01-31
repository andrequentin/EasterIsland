using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndGamePanelScript : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI endGameTitle;
    [SerializeField]
    private TextMeshProUGUI endGameParagraph;

    public void SetWonPanel()
    {
        endGameTitle.text = "You Won!";
        endGameParagraph.text = "You won the game by defeating the enemy tribe and preserving the island's ressources!";
    }

    public void SetLostPanel()
    {
        endGameTitle.text = "You Lose!";
        endGameParagraph.text = "The enemy tribe defeated you!";
    }

    public void SetLostPanelByRessource()
    {
        endGameTitle.text = "You Lose!";
        endGameParagraph.text = "You couldn't preserve the island's ressources!";
    }
    
    public void GoToMainMenu()
    {
        Destroy(GameManager._instance.GetComponent<NetworkBehavior>().CotcSdk);
        Destroy(GameManager._instance.gameObject);
        Destroy(EasyAI._instance.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Time.timeScale++;
    }
}
