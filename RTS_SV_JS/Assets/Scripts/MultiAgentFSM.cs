using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public enum States
{
    IDLE,
    MOVING,
    GATHERING,
    STOCKING,
    FIGHTING,
    BUILDING
}
public class MultiAgentFSM : MonoBehaviour
{
    //-----Unit stats----------
    public Unit unitInfo;
    private int health;
    private int currentCapacity;
    private bool isFull = false;
    private int[] ressourcesQuantity = new int[3];


    private States currentState;
    bool[] scarceRessources = new bool[3];

    private Rigidbody2D rb2d;
    Transform target;
    public Transform oldTarget;
    public Vector2 destination;
    public Vector2 oldDestination;

    private Vector2 nullVector { get { return new Vector2(-9999, -9999); } }
    public float nextWaypointDistance = 3f;


    public Transform gfx;

    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;
    private Seeker seeker;

    public bool canMove = true;
    private bool atDestination = false;


    public float consumeCounter = 0f;
    public float attackCounter = 0f;
    public float buildCounter = 0f;

    private const string RESSOURCE_TAG = "Ressource";
    private const string DROPPOINT_TAG = "EnemyDropPoint";

    void Start()
    {
        this.health = unitInfo.health;
        currentState = States.IDLE;

        rb2d = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        destination = nullVector;
        oldDestination = nullVector;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        
    }

    public int GetCurrentHealth()
    {
        return this.health;
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && target != null)
            seeker.StartPath(rb2d.position, target.position, OnPathComplete);

        /*else if ((seeker.IsDone() && destination != nullVector))
        {
            seeker.StartPath(rb2d.position, destination, OnPathComplete);

        }*/
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdateGFX()
    {
        //Flip updating
        if (rb2d.velocity.x >= 0.01f)
        {
            gfx.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (rb2d.velocity.x <= -0.01f)
        {
            gfx.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void ComputeMovement()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            atDestination = true;
            canMove = false;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb2d.position).normalized;
        //Vector2 force = direction * unitInfo.speed * Time.deltaTime;
        Vector2 force = direction * unitInfo.speed * Time.deltaTime;
        //rb2d.AddForce(force);
        rb2d.velocity = force;

        float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

    }

    public void SetTarget(Transform t)
    {
        //Cancel path if existing
        if (seeker.GetCurrentPath() != null)
        {
            seeker.CancelCurrentPathRequest();
        }

        //destination = nullVector;
        //oldDestination = nullVector;

        this.oldTarget = this.target;
        this.target = t;

        if (this.target != this.oldTarget)
        {
            //Debug.Log("test");
            atDestination = false;
            canMove = true;
        }

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(currentState)
        {
            case States.IDLE:
                IdleState();
                if (EasyAI._instance.buildList.Count > 0)
                {
                    this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.MOVING:
                MovingState();
                if (EasyAI._instance.buildList.Count > 0)
                {
                    this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.GATHERING:
                GatherState();
                if (EasyAI._instance.buildList.Count > 0)
                {
                    this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.STOCKING:
                StockState();
                if (EasyAI._instance.buildList.Count > 0)
                {
                    this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.FIGHTING:
                FightState();
                if (EasyAI._instance.buildList.Count > 0)
                {
                    this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.BUILDING:
                BuildState();
                break;

        }
        
        CapacityCheck();
        UpdateGFX();
    }

    private void BuildState()
    {
        buildCounter += Time.deltaTime;
        if (buildCounter >= 3f)
        {
            target.SendMessage("BuildMe", this.unitInfo.buildPower);
            if (target.GetComponent<Buildable>().GetBuildValue() >= target.GetComponent<Buildable>().GetMaxBuild())
            {
                this.target = null;
                this.currentState = States.IDLE;
                EasyAI._instance.RemoveFirstFoundationFromList();
            }
            buildCounter = 0f;
        }
    }

    private void IdleState()
    {
        scarceRessources = EasyAI._instance.GetScarceRessource();
        if (CheckScarceRessource())
        { 

            if (unitInfo.unitType == UnitTypes.LUMBERJACK)
            {
                if (scarceRessources[0])
                {
                    SetTarget(GetNearestRessourcePoint(RessourceTypes.WOOD));
                }
            }

            else if (unitInfo.unitType == UnitTypes.GATHERER)
            {
                if (scarceRessources[1] && !scarceRessources[2])
                {
                    SetTarget(GetNearestRessourcePoint(RessourceTypes.ANIMAL));
                }
                else if (scarceRessources[2] && !scarceRessources[1])
                {
                    SetTarget(GetNearestRessourcePoint(RessourceTypes.VEGETAL));
                }
                else if (scarceRessources[1] && scarceRessources[2])
                {
                    int rando = Random.Range(0, 2);
                    if (rando == 0)
                        SetTarget(GetNearestRessourcePoint(RessourceTypes.ANIMAL));
                    if (rando == 1)
                        SetTarget(GetNearestRessourcePoint(RessourceTypes.VEGETAL));
                }
            }

            else if (unitInfo.unitType == UnitTypes.NORMAL)
            {
                if(scarceRessources[0] && GetNearestRessourcePoint(RessourceTypes.WOOD) != null)
                    SetTarget(GetNearestRessourcePoint(RessourceTypes.WOOD));
                /*int temp = Random.Range(0, scarceRessources.Length);
                bool tempBool = scarceRessources[temp];
                while(!tempBool)
                {
                    temp = Random.Range(0, scarceRessources.Length);
                    tempBool = scarceRessources[temp];
                }

                switch(temp)
                {
                    case 0:
                        target = GetNearestRessourcePoint(RessourceTypes.WOOD);
                        break;
                    case 1:
                        target = GetNearestRessourcePoint(RessourceTypes.ANIMAL);
                        break;
                    case 2:
                        target = GetNearestRessourcePoint(RessourceTypes.VEGETAL);
                        break;
                }*/
            }

            this.currentState = States.MOVING;
        }
    }

    void ConsumeRessource(Transform target)
    {
        if (!isFull)
        {
            consumeCounter += Time.deltaTime;

            if (consumeCounter >= unitInfo.gatherSpeed)
            {
                target.SendMessage("Consume", unitInfo.gatherTick);
                if (target.GetComponent<Ressource>().getYield() <= 0)
                    this.target = null;
                RessourceTypes rt = target.GetComponent<Ressource>().GetRessourceType();

                switch (rt)
                {
                    case RessourceTypes.WOOD:
                        this.ressourcesQuantity[0] += unitInfo.gatherTick;
                        break;
                    case RessourceTypes.ANIMAL:
                        this.ressourcesQuantity[1] += unitInfo.gatherTick;
                        break;
                    case RessourceTypes.VEGETAL:
                        this.ressourcesQuantity[2] += unitInfo.gatherTick;
                        break;
                    default:
                        break;
                }

                consumeCounter = 0f;
            }
        }
    }

    void DropRessources(Transform target)
    {
        int totalCarry = ressourcesQuantity[0] + ressourcesQuantity[1] + ressourcesQuantity[2];
        if (!target.GetComponent<DropPoint>().IsFull() && (target.GetComponent<DropPoint>().GetTotalCapacity() + totalCarry) < target.GetComponent<DropPoint>().GetMaxCapacity())
        {
            target.SendMessage("SetRessource", this.ressourcesQuantity);
            ressourcesQuantity[0] = 0;
            ressourcesQuantity[1] = 0;
            ressourcesQuantity[2] = 0;
        }
    }

    private void MovingState()
    {
        if (canMove && target != null)
        {
            ComputeMovement();
        }
        if(atDestination)
        {
            if (target != null && target.gameObject.tag == RESSOURCE_TAG && !isFull)
            {
                this.currentState = States.GATHERING;
            }

            else if (target != null && target.gameObject.tag == DROPPOINT_TAG)
            {
                this.currentState = States.STOCKING;
            }
        }
    }

    private void GatherState()
    {
        ConsumeRessource(target);
        if(isFull)
        {
            SetTarget(GameObject.FindGameObjectWithTag(DROPPOINT_TAG).gameObject.transform);
            this.currentState = States.MOVING;
        }
    }

    private void StockState()
    {
        DropRessources(target);
        currentState = States.IDLE;
    }

    private void FightState()
    {

    }

    public void TakeDamage(int dmg)
    {
        this.health -= dmg;
        if(this.health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    private bool CheckScarceRessource()
    {
        bool toReturn = false;
        for(int i = 0; i < scarceRessources.Length; i++)
        {
            if (scarceRessources[i])
            {
                toReturn = true;
                break;
            }
        }
        return toReturn;
    }

    private Transform GetNearestRessourcePoint(RessourceTypes ressourceType)
    {
        if (FindObjectsOfType<Ressource>().Length > 0)
        {
            Ressource[] temp = FindObjectsOfType<Ressource>();
            List<Ressource> ressourceList = new List<Ressource>();

            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].GetRessourceType() == ressourceType)
                    ressourceList.Add(temp[i]);
            }

            Transform nearestRessource = ressourceList[0].gameObject.transform;
            float nearestDistance = Vector2.Distance(this.gameObject.transform.position, nearestRessource.position);

            foreach (Ressource r in ressourceList)
            {
                float tempDistance = Vector2.Distance(this.gameObject.transform.position, r.gameObject.transform.position);
                if (tempDistance < nearestDistance)
                {
                    nearestDistance = tempDistance;
                    nearestRessource = r.gameObject.transform;
                }
            }

            return nearestRessource;
        }
        else return null;
    }

    private void CapacityCheck()
    {
        currentCapacity = this.ressourcesQuantity[0] + this.ressourcesQuantity[1] + this.ressourcesQuantity[2];
        if (currentCapacity >= this.unitInfo.capacity)
        {
            isFull = true;
        }
        else
        {
            isFull = false;
        }
    }
}
