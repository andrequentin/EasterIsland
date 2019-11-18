using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAI : MonoBehaviour
{
    private const string ENEMY_DROPPOINT = "EnemyDropPoint";
    public static EasyAI _instance;

    DropPoint AIDropPoint;

    public int scarceRessourceThreshold;

    private int maxPopulation = 5;
    private int currentPopulation;
    private List<GameObject> units = new List<GameObject>();

    [SerializeField]
    private GameObject houseFoundation;
    private int houseCost = 20;

    private Vector2 currentBuildSlot = new Vector2(-23f, 23);

    public List<GameObject> buildList = new List<GameObject>();

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
        GameObject[] unitsTab = GameObject.FindGameObjectsWithTag("Enemy");
        units.AddRange(unitsTab);
        currentPopulation = units.Count;
    }

    // Update is called once per frame
    void Update()
    {
        BrainWork();
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

    public void BrainWork()
    {
       if(/*this.currentPopulation == (this.maxPopulation - 1) &&*/ this.houseCost <= this.AIDropPoint.GetWood())
       {
            //Build a house
            BuildHouse();
       }



    }

    private void BuildHouse()
    {
        GameObject temp = Instantiate(houseFoundation, currentBuildSlot, Quaternion.identity);
        this.AIDropPoint.DecrementWood(houseCost);
        this.currentBuildSlot.x += 2.5f;
        this.buildList.Add(temp);
    }

    public void RemoveFirstFoundationFromList()
    {
        this.buildList.RemoveAt(0);
    }

    public void IncreasePopulation(int pop)
    {
        this.maxPopulation += pop;
    }

    public void ChangeWoodMaxCapacity(int d)
    {
        this.AIDropPoint.ChangeWoodMaxCapacity(d);
    }
}
