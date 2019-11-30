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
        endGameParagraph.text = "Won paragraph";
    }

    public void SetLostPanel()
    {
        endGameTitle.text = "You Lose!";
        endGameParagraph.text = "Lost paragraph";
    }
}
