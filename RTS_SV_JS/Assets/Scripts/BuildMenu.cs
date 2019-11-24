using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    private static string GAMEMANAGER_TAG = "GameManager";
    [SerializeField]
    private GameObject houseFoundation;
    [SerializeField]
    private GameObject sawMillFoundation;
    [SerializeField]
    private GameObject granaryFoundation;
    private int houseCost = 20;
    private int sawMillCost = 30;
    private int granaryCost = 30;
    private int citizenCost = 10;
    [SerializeField]
    private GameObject citizenPrefab;
    private Vector3 citizenSpawnPosition = new Vector3(15, -22, -1);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildHouse()
    {
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPut", houseFoundation);
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPutCost", houseCost);
        Debug.Log("Build House");
    }

    public void BuildSawMill()
    {
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPut", sawMillFoundation);
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPutCost", sawMillCost);
        Debug.Log("Build Sawmill");
    }

    public void BuildGranary()
    {
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPut", granaryFoundation);
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPutCost", granaryCost);
        Debug.Log("Build Granary");
    }

    public void BuildCitizen()
    {
        if(GameManager._instance.GetDropPoint().GetComponent<DropPoint>().GetAnimal() >= citizenCost)
        {
            Instantiate(citizenPrefab, citizenSpawnPosition, Quaternion.identity);
            GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementAnimal(citizenCost);
            GameManager._instance.IncreaseCurrentPopulation();
            Debug.Log("Create Citizen");
        }
        else
        {
            MessageDisplayer._instance.DisplayMessage("Not enough meat");
        }
    }
}
