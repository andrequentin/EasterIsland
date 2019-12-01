using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAI : MonoBehaviour
{

    //0 Warrior 1 Lumber 2 Gatherer
    [SerializeField]
    Unit[] unitTypes;


    private const string ENEMY_DROPPOINT = "EnemyDropPoint";
    public static EasyAI _instance;

    DropPoint AIDropPoint;

    public int scarceRessourceThreshold;

    private int maxPopulation = 5;
    private int currentPopulation;
    private List<GameObject> units = new List<GameObject>();

    [SerializeField]
    private GameObject houseFoundation;
    [SerializeField]
    private GameObject sawMillFoundation;
    [SerializeField]
    private GameObject granaryFoundation;

    private int houseCost = 20;
    private int sawMillCost = 30;
    private int granaryCost = 30;

    [SerializeField]
    private GameObject citizenPrefab;

    private int citizenCost = 10;
    private int warriorWoodCost = 5;
    private int warriorMeatCost = 5;
    private int lumberjackWoodCost = 5;
    private int gathererVegetalCost = 5;

    private Vector2 currentBuildSlot = new Vector2(-90f, 95);

    public List<GameObject> buildList = new List<GameObject>();
    public List<GameObject> pendingCitizens = new List<GameObject>();

    private Vector3 citizenSpawnPosition = new Vector3(-90, 95, -1);

    //Increment this variable modulo 3 to determine what citizen we gonna create, 0 for warrior, 1 for lumber and 2 for gatherer
    private int citizenCounter = 0;

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
        scarceRessourceThreshold = GameManager._instance.ressourceThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        currentPopulation =  FindObjectsOfType<MultiAgentFSM>().Length;

        BrainWork();
        if (Input.GetKeyDown(KeyCode.G))
            BuildGranary();
        if (Input.GetKeyDown(KeyCode.H))
            BuildSawmill();

    }

    private int GetCitizenNumber()
    {
        
        return citizenCounter++ % 3; ;
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


    //Make a counter to not call this every frame
    //Every 2 seconds Check citizen creation
    //Every 3 seconds check citizen upgrade
    //no need to do this for buildings for now
    public void BrainWork()
    {
       if(this.currentPopulation == (this.maxPopulation - 1) && this.houseCost <= this.AIDropPoint.GetWood())
       {
            //Build a house
            BuildHouse();
       }

       if(this.AIDropPoint.GetWood() >= this.AIDropPoint.GetMaxWood() - 10 && this.AIDropPoint.GetWood() >= sawMillCost)
        {
            //Build sawmill
            BuildSawmill();
        }

        if (this.AIDropPoint.GetAnimal() >= (this.AIDropPoint.GetMaxAnimal() - 10) || this.AIDropPoint.GetVegetal() >= (this.AIDropPoint.GetMaxVegetal() - 10))
        {if(this.AIDropPoint.GetWood() >= granaryCost)
            //Build granary
            BuildGranary();
        }

       if(this.AIDropPoint.GetAnimal() >= citizenCost)
        {
            CreateCitizen();
        }

        CheckPendingCitizensForUpgrade();



    }

    void CheckPendingCitizensForUpgrade()
    {
        if(pendingCitizens.Count >0)
        UpgradeCitizen(pendingCitizens[0]);
    }

    private void CreateCitizen()
    {
        GameObject temp = Instantiate(citizenPrefab, citizenSpawnPosition, Quaternion.identity);
        this.AIDropPoint.DecrementAnimal(citizenCost);
        currentPopulation++;

        UpgradeCitizen(temp);
    }

    private void UpgradeCitizen(GameObject temp)
    {
        int cit = GetCitizenNumber();
        switch(cit)
        {
            //Warrior
            case 0:
                UpgradeToWarrior(temp);
                break;
            //Lumberjack
            case 1:
                UpgradeToLumberjack(temp);
                break;
            //Gatherer
            case 2:
                UpgradeToGatherer(temp);
                break;
        }
    }

    void UpgradeToWarrior(GameObject temp)
    {
        if(warriorWoodCost > this.AIDropPoint.GetWood() || warriorMeatCost > this.AIDropPoint.GetAnimal())
        {
            if(!pendingCitizens.Contains(temp))
                pendingCitizens.Add(temp);
            return;
        }
        temp.GetComponent<MultiAgentFSM>().UpgradeUnit(unitTypes[0]);
        this.AIDropPoint.DecrementAnimal(warriorMeatCost);
        this.AIDropPoint.DecrementWood(warriorWoodCost);

        if (pendingCitizens.Contains(temp))
            pendingCitizens.Remove(temp);
    }

    void UpgradeToLumberjack(GameObject temp)
    {
        if (warriorWoodCost > this.AIDropPoint.GetWood())
        {
            if (!pendingCitizens.Contains(temp))
                pendingCitizens.Add(temp);
            return;
        }
        temp.GetComponent<MultiAgentFSM>().UpgradeUnit(unitTypes[1]);
        this.AIDropPoint.DecrementWood(lumberjackWoodCost);

        if (pendingCitizens.Contains(temp))
            pendingCitizens.Remove(temp);

    }

    void UpgradeToGatherer(GameObject temp)
    {
        if (warriorWoodCost > this.AIDropPoint.GetAnimal())
        {
            if (!pendingCitizens.Contains(temp))
                pendingCitizens.Add(temp);
            return;
        }
        temp.GetComponent<MultiAgentFSM>().UpgradeUnit(unitTypes[2]);
        this.AIDropPoint.DecrementVegetal(gathererVegetalCost);

        if (pendingCitizens.Contains(temp))
            pendingCitizens.Remove(temp);

    }


    private void BuildHouse()
    {
        this.currentBuildSlot.x += 2.5f;
        if (this.currentBuildSlot.x >= 0)
        {
            this.currentBuildSlot.y -= 2.5f;
            this.currentBuildSlot.x = -90;
        }

        GameObject temp = Instantiate(houseFoundation, currentBuildSlot, Quaternion.identity);
        this.AIDropPoint.DecrementWood(houseCost);
        
        this.buildList.Add(temp);
    }

    private void BuildSawmill()
    {
        this.currentBuildSlot.x += 2.5f;
        if(this.currentBuildSlot.x >= 0)
        {
            this.currentBuildSlot.y -= 2.5f;
            this.currentBuildSlot.x = -90;
        }

        GameObject temp = Instantiate(sawMillFoundation, currentBuildSlot, Quaternion.identity);
        this.AIDropPoint.DecrementWood(sawMillCost);
        
        this.buildList.Add(temp);
    }

    private void BuildGranary()
    {
        this.currentBuildSlot.x += 2.5f;
        if (this.currentBuildSlot.x >= 0)
        {
            this.currentBuildSlot.y -= 2.5f;
            this.currentBuildSlot.x = -90;
        }

        GameObject temp = Instantiate(granaryFoundation, currentBuildSlot, Quaternion.identity);
        this.AIDropPoint.DecrementWood(granaryCost);
        
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

    public void ChangeAnimalMaxCapacity(int d)
    {
        this.AIDropPoint.ChangeAnimalMaxCapacity(d);
    }

    public void ChangeVegetalMaxCapacity(int d)
    {
        this.AIDropPoint.ChangeVegetalMaxCapacity(d);
    }
}
