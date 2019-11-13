using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAI : MonoBehaviour
{
    private const string ENEMY_DROPPOINT = "EnemyDropPoint";
    public static EasyAI _instance;

    DropPoint AIDropPoint;

    public int scarceRessourceThreshold;

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AIDropPoint = GameObject.FindGameObjectWithTag(ENEMY_DROPPOINT).GetComponent<DropPoint>();         
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool[] GetScarceRessource()
    {
        bool[] toReturn = new bool[3];

        if (AIDropPoint.GetWood() <= scarceRessourceThreshold)
            toReturn[0] = true;
        if (AIDropPoint.GetAnimal() <= scarceRessourceThreshold)
            toReturn[1] = true;
        if (AIDropPoint.GetAnimal() <= scarceRessourceThreshold)
            toReturn[2] = true;

        return toReturn;
    }
}
