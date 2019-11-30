using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granary : MonoBehaviour
{
    public bool isEnemy;
    // Start is called before the first frame update
    void Start()
    {
        if (!isEnemy)
        {
            GameManager._instance.GetDropPoint().GetComponent<DropPoint>().ChangeAnimalMaxCapacity(50);
            GameManager._instance.GetDropPoint().GetComponent<DropPoint>().ChangeVegetalMaxCapacity(50);
        }
        else
        {
            EasyAI._instance.ChangeAnimalMaxCapacity(50);
            EasyAI._instance.ChangeVegetalMaxCapacity(50);
        }
    }
}
