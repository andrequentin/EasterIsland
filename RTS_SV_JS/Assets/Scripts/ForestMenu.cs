using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestMenu : MonoBehaviour
{
    private static string GAMEMANAGER_TAG = "GameManager";

    private InputManager im;
    private void Start()
    {
        im = GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).GetComponent<InputManager>();

    }
    // Start is called before the first frame update
    public void setRessourceWood()
    {
        //Debug.Log("wud");
        foreach (GameObject u in im.selectedObjects)
        {
            if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
            {
                u.GetComponent<AITest>().lookingFor = RessourceTypes.WOOD;
            }
        }
        GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleForestPanel");
    }
    public void setRessourceVegetal()
    {
        //Debug.Log("vegetal");
        foreach (GameObject u in im.selectedObjects)
        {
            if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
            {
                u.GetComponent<AITest>().lookingFor = RessourceTypes.VEGETAL;
            }
        }
        GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleForestPanel");

    }

}
