﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    //0 Warrior 1 Lumber 2 Gatherer
    [SerializeField]
    Unit[] unitTypes;
    private static string GAMEMANAGER_TAG = "GameManager";
    private InputManager im;

    private int warriorCostWood = 5;
    private int warriorCostMeat = 5;

    private int lumberjackCostWood = 5;

    private int gathererCostVegetal = 5;
    private void Start()
    {
        im = GameObject.FindGameObjectWithTag(GAMEMANAGER_TAG).GetComponent<InputManager>();
    }
    public void MakeWarrior()
    {
        int totalWoodCost = 0;
        int totalMeatCost = 0;
        foreach(GameObject u in im.selectedObjects)
        {
            if(u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
            {
                totalWoodCost += warriorCostWood;
                totalMeatCost += warriorCostMeat;
            }
        }
        if(totalWoodCost > GameManager._instance.wood || totalMeatCost > GameManager._instance.animal)
        {
            MessageDisplayer._instance.DisplayMessage("Not enough ressources");
        }
        else
        {
            foreach (GameObject u in im.selectedObjects)
            {
                u.GetComponent<AITest>().unitInfo = unitTypes[0];
                GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementWood(warriorCostWood);
                GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementAnimal(warriorCostMeat);
            }
        }
    }

    public void MakeLumberjack()
    {

    }

    public void MakeGatherer()
    {

    }

   
}