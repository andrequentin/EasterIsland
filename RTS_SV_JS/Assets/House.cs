using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public bool isEnemy;
    // Start is called before the first frame update
    void Start()
    {
        if (!isEnemy)
            GameManager._instance.IncreasePopulation(2);
        else
            EasyAI._instance.IncreasePopulation(2);
    }

  
}
