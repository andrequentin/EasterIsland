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
    private int houseCost = 20;
    private int sawMillCost = 30;
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
}
