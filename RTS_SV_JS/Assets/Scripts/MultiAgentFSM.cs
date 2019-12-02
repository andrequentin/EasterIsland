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
    BUILDING,
    FLEE
}
public class MultiAgentFSM : MonoBehaviour
{
    //-----Unit stats----------
    public Unit unitInfo;
    private int health;
    private int currentCapacity;
    private bool isFull = false;
    private int[] ressourcesQuantity = new int[3];

    private float detectionRange = 8f;

    private bool isBeingAttacked = false;

    private States currentState;
    bool[] scarceRessources = new bool[3];

    private Rigidbody2D rb2d;
    Transform target;
    public Transform oldTarget;
    public Vector2 destination;
    public Vector2 oldDestination;
    float beingAttackedResetCounter = 0;

    private Vector2 nullVector { get { return new Vector2(-9999, -9999); } }
    public float nextWaypointDistance = 1.5f;
    public RessourceTypes lookingFor = RessourceTypes.WOOD;

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

    public float range = 1.5f;

    private const string RESSOURCE_TAG = "Ressource";
    private const string DROPPOINT_TAG = "EnemyDropPoint";

    private float randomMoveCounter = 0;
    public float randomMoveDelay = 5f;

    void Start()
    {
        this.health = unitInfo.health;
        currentState = States.IDLE;
        this.gfx.GetComponent<SpriteRenderer>().sprite = unitInfo.sprite;
        rb2d = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        destination = nullVector;
        oldDestination = nullVector;
        //CancelInvoke("UpdatePath");
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
        rb2d.velocity.Normalize();

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
        if (target.gameObject.CompareTag("Prey")) { lookingFor = RessourceTypes.ANIMAL; }

        if (this.target != this.oldTarget)
        {
            //Debug.Log("test");
            atDestination = false;
            canMove = true;
        }
        CancelInvoke("UpdatePath");
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null)
            this.currentState = States.IDLE;
        attackCounter += Time.deltaTime;
        //Reset being attacked after 10 seconds
        if(isBeingAttacked)
        {
            beingAttackedResetCounter += Time.deltaTime;
            if(beingAttackedResetCounter >= 10f)
            {
                beingAttackedResetCounter = 0;
                isBeingAttacked = false;
            }
        }

        CapacityCheck();
        UpdateGFX();

        switch (currentState)
        {
            case States.IDLE:
                IdleState();
                if (EasyAI._instance.buildList.Count > 0)
                {
                    //this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.MOVING:
                MovingState();
                if (EasyAI._instance.buildList.Count > 0 && !target.CompareTag("EnemyBuildable"))
                {
                    //this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.GATHERING:
                GatherState();
                if (EasyAI._instance.buildList.Count > 0 && !target.CompareTag("EnemyBuildable"))
                {
                    //this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.STOCKING:
                StockState();
                /*if (EasyAI._instance.buildList.Count > 0 && !target.CompareTag("EnemyBuildable"))
                {
                    //this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }*/
                break;

            case States.FIGHTING:
                FightState();
                if (EasyAI._instance.buildList.Count > 0 && !target.CompareTag("EnemyBuildable"))
                {
                    //this.currentState = States.BUILDING;
                    SetTarget(EasyAI._instance.buildList[0].transform);
                }
                break;

            case States.BUILDING:
                BuildState();
                break;

            case States.FLEE:
                FleeState();
                break;

        }
        
        
    }

    private void BuildState()
    {
        
        if (IsTargetInRange())
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
        else if(!IsTargetInRange())
        {
            reachedEndOfPath = false;
            atDestination = false;
            canMove = true;
            
            
            CancelInvoke("UpdatePath");
            InvokeRepeating("UpdatePath", 0f, 0.5f);
            currentState = States.MOVING;

        }

        if (this.unitInfo.unitType == UnitTypes.NORMAL || this.unitInfo.unitType == UnitTypes.LUMBERJACK || this.unitInfo.unitType == UnitTypes.GATHERER)
        {
            if (isBeingAttacked)
            {
                this.currentState = States.FIGHTING;


            }

        }
    }

    private void IdleState()
    {
        SetTarget(null);
        //Ressource Checking
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
                    if (GameObject.FindGameObjectsWithTag("Prey").Length >= 3)
                        SetTarget(GetNearestRessourcePoint(RessourceTypes.ANIMAL));
                    else
                        return;
                }
                else if (scarceRessources[2] && !scarceRessources[1])
                {
                    SetTarget(GetNearestRessourcePoint(RessourceTypes.VEGETAL));
                }
                else if (scarceRessources[1] && scarceRessources[2])
                {
                    int rando = Random.Range(0, 2);
                    if (rando == 0)
                    {
                        if (GameObject.FindGameObjectsWithTag("Prey").Length >= 3)
                            SetTarget(GetNearestRessourcePoint(RessourceTypes.ANIMAL));
                        else
                            return;
                    }

                    if (rando == 1)
                        SetTarget(GetNearestRessourcePoint(RessourceTypes.VEGETAL));
                }
            }

            else if (unitInfo.unitType == UnitTypes.NORMAL)
            {
                //if(scarceRessources[0] && GetNearestRessourcePoint(RessourceTypes.WOOD) != null)
                //    SetTarget(GetNearestRessourcePoint(RessourceTypes.WOOD));
                int temp = Random.Range(0, scarceRessources.Length);
                bool tempBool = scarceRessources[temp];
                while(!tempBool)
                {
                    temp = Random.Range(0, scarceRessources.Length);
                    tempBool = scarceRessources[temp];
                }
                
                switch(temp)
                {
                    case 0:
                        SetTarget( GetNearestRessourcePoint(RessourceTypes.WOOD));
                        
                        break;
                    case 1:
                        if (GameObject.FindGameObjectsWithTag("Prey").Length >= 3)
                            SetTarget(GetNearestRessourcePoint(RessourceTypes.ANIMAL));
                        else
                            return;
                        break;
                    case 2:
                        SetTarget(GetNearestRessourcePoint(RessourceTypes.VEGETAL));
                        break;
                }
                //print(temp);
               // print(target);
            }

            this.currentState = States.MOVING;
        }

        //Fighting
        //if normal unit,lumberjack or gatherer, fight back but flee when low hp
        if (this.unitInfo.unitType == UnitTypes.NORMAL || this.unitInfo.unitType == UnitTypes.LUMBERJACK || this.unitInfo.unitType == UnitTypes.GATHERER)
        {
            if(isBeingAttacked)
            {
                this.currentState = States.FIGHTING;

              
            }
         
        }
    
        //if warrior, detect ennemies and fight till the end 
        if(this.unitInfo.unitType == UnitTypes.WARRIOR)
        {
            Collider2D[] detected = Physics2D.OverlapCircleAll(this.transform.position, detectionRange);
            foreach(Collider2D c in detected)
            {
                if(c.gameObject.GetComponent<AITest>())
                {
                    this.target = c.gameObject.transform;
                    this.currentState = States.MOVING;
                    break;
                }
            }
        }

        randomMoveCounter += Time.deltaTime;
        if(randomMoveCounter >= randomMoveDelay)
        {
            Vector2 mv = new Vector2(Random.Range(-unitInfo.speed, unitInfo.speed), Random.Range(-unitInfo.speed, unitInfo.speed));
            rb2d.velocity = mv.normalized;
            randomMoveCounter = 0;
        }
    }

    void ConsumeRessource(Transform target)
    {
        if (!isFull)
        {
            consumeCounter += Time.deltaTime;

            if (consumeCounter >= unitInfo.gatherSpeed)
            {
                foreach (Ressource u in target.GetComponents<Ressource>())
                {
                    if (u.ressourceType == lookingFor)
                    {
                        if (target.GetComponent<Ressource>().getYield() <= 0)
                        {
                            this.target = null;
                        }


                        switch (lookingFor)
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

                        object[] a = new object[2];
                        a[0] = unitInfo.gatherTick;
                        a[1] = lookingFor;

                        target.SendMessage("Consume", a);
                        consumeCounter = 0f;
                    }
                }

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
            rb2d.velocity = Vector2.zero;
            if (target != null && target.gameObject.tag == RESSOURCE_TAG && !isFull)
            {
                this.currentState = States.GATHERING;
            }

            else if (target != null && target.gameObject.tag == DROPPOINT_TAG)
            {
                this.currentState = States.STOCKING;
            }

            else if (target != null && target.gameObject.tag == "Selectable")
            {
                this.currentState = States.FIGHTING;
            }
            else if(target != null && target.gameObject.CompareTag("Prey"))
            {
                this.currentState = States.GATHERING;
            }
            else if(target != null && target.gameObject.CompareTag("EnemyBuildable"))
            {
                this.currentState = States.BUILDING;
            }
        }

        if (this.unitInfo.unitType == UnitTypes.NORMAL || this.unitInfo.unitType == UnitTypes.LUMBERJACK || this.unitInfo.unitType == UnitTypes.GATHERER)
        {
            if (isBeingAttacked)
            {
                this.currentState = States.FIGHTING;


            }

        }
    }

    private void GatherState()
    {
        if (IsTargetInRange())
        {
            if (target.gameObject.CompareTag("Prey"))
            {
                if (target.GetComponent<preyAI>().GetHealth() >= 0)
                {
                    AttackTarget(target);
                }
                else if (target.GetComponent<preyAI>().GetHealth() < 0)
                {
                    ressourcesQuantity[1] += target.GetComponent<Ressource>().GetAnimalYield();
                    target.GetComponent<preyAI>().Die();
                    target = null;

                }
            }
            else
                ConsumeRessource(target);
        }
        else if(!IsTargetInRange())
        {
            reachedEndOfPath = false;
            atDestination = false;
            canMove = true;
            
            
            CancelInvoke("UpdatePath");
            InvokeRepeating("UpdatePath", 0f, 0.5f);
            currentState = States.MOVING;
        }

        if(isFull)
        {
            reachedEndOfPath = false;
            atDestination = false;
            canMove = true;
            SetTarget(GameObject.FindGameObjectWithTag(DROPPOINT_TAG).gameObject.transform);
            CancelInvoke("UpdatePath");
            InvokeRepeating("UpdatePath", 0f, 0.5f);
            this.currentState = States.MOVING;
        }

        if (this.unitInfo.unitType == UnitTypes.NORMAL || this.unitInfo.unitType == UnitTypes.LUMBERJACK || this.unitInfo.unitType == UnitTypes.GATHERER)
        {
            if (isBeingAttacked)
            {
                this.currentState = States.FIGHTING;


            }

        }


    }

    private void AttackTarget(Transform target)
    {
        if (IsTargetInRange())
        {
            if (attackCounter >= unitInfo.attackSpeed)
            {
                target.SendMessage("TakeDamage", unitInfo.damage);
                if (target.CompareTag("Selectable") && target.GetComponent<AITest>().GetHealth() <= 0)
                    this.target = null;
                else if (target.CompareTag("PlayerBuilding") && target.GetComponent<BuildingHealth>().GetCurrentHealth() <= 0)
                    this.target = null;
                else if (target.CompareTag("Prey") && target.GetComponent<preyAI>().GetHealth() <= 0)
                {
                    ConsumeRessource(target);
                }
                attackCounter = 0f;
            }
        }

        else if (!IsTargetInRange())
        {
            reachedEndOfPath = false;
            atDestination = false;
            canMove = true;
            Debug.Log("Target gone far");
            currentState = States.MOVING;
            CancelInvoke("UpdatePath");
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }
    }

    private bool IsTargetInRange()
    {
        return (Vector2.Distance(this.transform.position, target.position) <= range);
    }

    private void StockState()
    {
        DropRessources(target);
        //lookingFor = null;
        currentState = States.IDLE;
    }

    private void FightState()
    {
        if(this.unitInfo.unitType == UnitTypes.WARRIOR)
        {
            AttackTarget(target);
        }
        else
        {
            AttackTarget(target);
            if(health <= 8)
            {
                this.currentState = States.FLEE;
            }

        }
    }

    private void FleeState()
    {
        List<MultiAgentFSM> agents = new List<MultiAgentFSM>();
        agents.AddRange(FindObjectsOfType<MultiAgentFSM>());
        agents.Remove(this);
        
        if (agents.Count > 0)
        {
            
            if (/*target != null && */!target.GetComponent<MultiAgentFSM>())
            { 
                foreach (MultiAgentFSM a in agents)
                {
                    if (a.unitInfo.unitType == UnitTypes.WARRIOR)
                    {
                        SetTarget(a.gameObject.transform);
                        //this.currentState = States.MOVING;
                        break;
                    }
                }
            }
            /*if (canMove && target != null)
            {
                ComputeMovement();
            }
            if (atDestination)
            {
                rb2d.velocity = Vector2.zero;
                this.currentState = States.FIGHTING;
            }*/

        }
        else if(agents.Count <= 0 && !target.CompareTag("EnemyDropPoint"))
        {
            //GO TO BASE
            Debug.Log("awahwah");
            SetTarget(GameObject.FindGameObjectWithTag("EnemyDropPoint").transform);
            
        }

        if (canMove && target != null)
        {
            ComputeMovement();
        }
        if (atDestination)
        {
            rb2d.velocity = Vector2.zero;

            this.currentState = States.FIGHTING;



        }
    }

    public void TakeDamage(int dmg)
    {
        isBeingAttacked = true;
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
            List<Transform> ressourceLocation = new List<Transform>();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].GetRessourceType() == ressourceType)
                    ressourceLocation.Add(temp[i].gameObject.transform);
            }

            
            if (ressourceLocation.Count > 0)
            {
                Transform nearestRessource = ressourceLocation[0];
                float nearestDistance = Vector2.Distance(this.gameObject.transform.position, nearestRessource.position);

                foreach (Transform r in nearestRessource)
                {
                    float tempDistance = Vector2.Distance(this.gameObject.transform.position, r.position);
                    if (tempDistance < nearestDistance)
                    {
                        nearestDistance = tempDistance;
                        nearestRessource = r.gameObject.transform;
                    }
                }
                print(nearestRessource);
                lookingFor = ressourceType;

                return nearestRessource;
            }
            else return null;
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

    public void UpgradeUnit(Unit newUnit)
    {
        this.unitInfo = newUnit;

        this.health = unitInfo.health;
        attackCounter = unitInfo.attackSpeed;
        consumeCounter = unitInfo.gatherSpeed;
        gfx.GetComponent<SpriteRenderer>().sprite = unitInfo.sprite;
    }

    public void ToggleBeingAttacked()
    {
        this.isBeingAttacked = !this.isBeingAttacked;
    }
}
