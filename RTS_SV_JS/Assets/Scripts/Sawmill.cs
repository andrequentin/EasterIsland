using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawmill : MonoBehaviour
{
    public bool isEnemy;
    // Start is called before the first frame update
    void Start()
    {
        if (!isEnemy)
            GameManager._instance.GetDropPoint().GetComponent<DropPoint>().ChangeWoodMaxCapacity(50);
        else
            EasyAI._instance.ChangeWoodMaxCapacity(50);
    }

}
