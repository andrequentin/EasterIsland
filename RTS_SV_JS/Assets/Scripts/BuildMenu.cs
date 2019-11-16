using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    private static string GAMEMANAGER_TAG = "GameManager";
    [SerializeField]
    private GameObject houseFoundation;
    private int houseCost = 20;
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
        GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).SendMessage("SetBuildingToPutCost", 20);
        Debug.Log("Build House");
    }
}
