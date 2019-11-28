using System.Collections;
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
        if(totalWoodCost == 0 || totalMeatCost == 0)
        {
            MessageDisplayer._instance.DisplayMessage("Can't upgrade units");
            return;
        }
        if(totalWoodCost > GameManager._instance.wood || totalMeatCost > GameManager._instance.animal)
        {
            MessageDisplayer._instance.DisplayMessage("Not enough ressources");
        }
        else
        {
            foreach (GameObject u in im.selectedObjects)
            {
                if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
                {
                    //u.GetComponent<AITest>().unitInfo = unitTypes[0];
                    u.GetComponent<AITest>().UpgradeUnit(unitTypes[0]);
                    GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementWood(warriorCostWood);
                    GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementAnimal(warriorCostMeat);
                }
            }
        }
    }

    public void MakeLumberjack()
    {
        int totalWoodCost = 0;
        foreach (GameObject u in im.selectedObjects)
        {
            if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
            {
                totalWoodCost += lumberjackCostWood;
            }
        }
        if (totalWoodCost == 0 )
        {
            MessageDisplayer._instance.DisplayMessage("Can't upgrade units");
            return;
        }
        if (totalWoodCost > GameManager._instance.wood )
        {
            MessageDisplayer._instance.DisplayMessage("Not enough ressources");
        }
        else
        {
            foreach (GameObject u in im.selectedObjects)
            {
                if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
                {
                    //u.GetComponent<AITest>().unitInfo = unitTypes[0];
                    u.GetComponent<AITest>().UpgradeUnit(unitTypes[1]);
                    GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementWood(lumberjackCostWood);
                }
            }
        }
    }


    public void MakeGatherer()
    {
        int totalVegetalCost = 0;
        foreach (GameObject u in im.selectedObjects)
        {
            if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
            {
                totalVegetalCost += gathererCostVegetal;
            }
        }
        if (totalVegetalCost == 0 )
        {
            MessageDisplayer._instance.DisplayMessage("Can't upgrade units");
            return;
        }
        if (totalVegetalCost > GameManager._instance.vegetal )
        {
            MessageDisplayer._instance.DisplayMessage("Not enough ressources");
        }
        else
        {
            foreach (GameObject u in im.selectedObjects)
            {
                if (u.GetComponent<AITest>().unitInfo.unitType == UnitTypes.NORMAL)
                {
                    //u.GetComponent<AITest>().unitInfo = unitTypes[0];
                    u.GetComponent<AITest>().UpgradeUnit(unitTypes[2]);
                    GameManager._instance.GetDropPoint().GetComponent<DropPoint>().DecrementWood(gathererCostVegetal);
                }
            }
        }
    }
    }

   

