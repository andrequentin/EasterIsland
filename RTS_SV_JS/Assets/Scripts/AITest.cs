using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//For instantianting units, the spawner will have an array with all the scriptable object types and will 
//assignate the Unit attribute of this class with the corresponding unit type

//TODO is capacity is full -> go to droppoint

    public enum UnitTypes
    {
        NORMAL,
        WARRIOR,
        GATHERER,
        LUMBERJACK
    }
public class AITest : MonoBehaviour
{
    //STATS
    public Unit unitInfo;
    private int health;
    private int currentCapacity;
    private bool isFull = false;
    public float range = 1.5f;




    //OTHER STUFF
    //0 is for wood, 1 is for animal, 2 is for vegetal
    private int[] ressourcesQuantity = new int[3];

    [SerializeField]
    private Material highlightedMaterial;
    [SerializeField]
    private Material defaultMaterial;

    private const string RESSOURCE_TAG = "Ressource";
    private const string DROPPOINT_TAG = "DropPoint";
    private const string ENEMY_TAG = "Enemy";
    private const string PREY_TAG = "Prey";
    private const string POSITION_TAG = "Position";
    private const string BUILDABLE_TAG = "Buildable";
    private const string ENEMYBUILDING_TAG = "EnemyBuilding";
    private const string ENEMYNEXUS_TAG = "EnemyDropPoint";
    private Rigidbody2D rb2d;
    public Transform target;
    public Transform oldTarget;
    public Vector2 destination;
    public Vector2 oldDestination;
    
    private Vector2 nullVector { get { return new Vector2(-9999, -9999); } }
    public float nextWaypointDistance = 1.5f;
    public RessourceTypes lookingFor=RessourceTypes.WOOD;


    public Transform gfx;
    

    [SerializeField]
    private GameObject infoPanel;
    private bool infoPanelActive = false;

    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;
    private Seeker seeker;

    public bool canMove = true;
    private bool atDestination = false;
    
    public float consumeCounter = 0f;
    public float attackCounter;
    public float buildCounter = 0f;

    private bool isSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        destination = nullVector;
        oldDestination = nullVector;
        CancelInvoke("UpdatePath");
        InvokeRepeating("UpdatePath",0f,0.5f);
        this.health = unitInfo.health;
        attackCounter = unitInfo.attackSpeed;
        consumeCounter = unitInfo.gatherSpeed;
        this.gfx.GetComponent<SpriteRenderer>().sprite = unitInfo.sprite;
    }

    void UpdatePath()
    {   
        if(seeker.IsDone() && target != null)
            seeker.StartPath(rb2d.position, target.position, OnPathComplete);

        else if((seeker.IsDone() &&  destination != nullVector))
        {
            seeker.StartPath(rb2d.position, destination, OnPathComplete);
            
        }
    }

    private void Update()
    {
        CapacityCheck();
       
    }

    public bool IsSelected()
    {
        return this.isSelected;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        attackCounter += Time.deltaTime; 

        if (canMove && (target != null || destination != nullVector))
        {
            ComputeMovement();
        }
        
        //DO STUFF DEPENDING ON TYPE
        if(atDestination)
        {
            rb2d.velocity = Vector2.zero;
           
            if(target != null && target.gameObject.tag == RESSOURCE_TAG && !isFull)
            {
   
                    ConsumeRessource(target);
                
            }

            else if(target != null && target.gameObject.tag == DROPPOINT_TAG)
            {
                DropRessources(target);
            }
            else if(target != null && target.gameObject.tag == ENEMY_TAG)
            {
                AttackTarget(target);
            }
            else if (target != null && target.gameObject.tag == PREY_TAG)
            {
                lookingFor = RessourceTypes.ANIMAL;
                if (target.GetComponent<preyAI>().GetHealth() > 0)
                {
                    AttackTarget(target);
                }
                else
                {
                    ConsumeRessource(target);
                }
            }
            else if(target != null && target.gameObject.tag == ENEMYBUILDING_TAG)
            {
                AttackTarget(target);
            }
            else if(target != null && target.gameObject.tag == BUILDABLE_TAG)
            {
                BuildTarget(target);
            }
            else if(target != null && target.gameObject.tag == ENEMYNEXUS_TAG)
            {
                AttackTarget(target);
            }
            else if(destination != nullVector)
            { 
                destination = nullVector;   
            }

            
        }
        //Debug.Log(seeker.GetCurrentPath());
        UpdateGFX();
    }

    void ConsumeRessource(Transform target)
    {
        consumeCounter += Time.deltaTime;

        if (consumeCounter >= unitInfo.gatherSpeed)
        {
            if (target.GetComponent<Ressource>())
            {

                if (target.GetComponent<Ressource>().getYield() <= 0) {
                    target.GetComponent<preyAI>().Die(); this.target = null;

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
            }
            object[] a = new object[2];
            a[0] = unitInfo.gatherTick;
            a[1] = lookingFor;

            target.SendMessage("Consume", a);

            consumeCounter = 0f;
        }

        if (target.GetComponent<Ressource>().getYield() <= 0 || target == null) {
            SetTarget(GetNearestRessourcePoint(lookingFor).transform);
        }

        CapacityCheck();
        if (isFull)
        {
            SetTarget(GameObject.FindGameObjectWithTag("DropPoint").GetComponent<DropPoint>().transform);
        }
    }

    void DropRessources(Transform target)
    {
        int totalCarry = ressourcesQuantity[0] + ressourcesQuantity[1] + ressourcesQuantity[2];
        if(!target.GetComponent<DropPoint>().IsFull() && (target.GetComponent<DropPoint>().GetTotalCapacity() + totalCarry) < target.GetComponent<DropPoint>().GetMaxCapacity())
        {
            Debug.Log(this.ressourcesQuantity);
            target.SendMessage("SetRessource", this.ressourcesQuantity);
            ressourcesQuantity[0] = 0;
            ressourcesQuantity[1] = 0;
            ressourcesQuantity[2] = 0;
        }
        SetTarget(GetNearestRessourcePoint(lookingFor)); 
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

    //SET A RANGE ATTRIBUTE AND TEST ON RANGE OR ELSE THEY WILL ATTACK FROM AFAR
    void AttackTarget(Transform target)
    {
        if (IsTargetInRange())
        {
            if (attackCounter >= unitInfo.attackSpeed)
            {
                if(target.CompareTag(ENEMY_TAG))
                {
                    target.GetComponent<MultiAgentFSM>().SetTarget(this.gameObject.transform);
                }
                target.SendMessage("TakeDamage", unitInfo.damage);
                if (target.CompareTag(ENEMY_TAG) && target.GetComponent<MultiAgentFSM>().GetCurrentHealth() <= 0)
                    this.target = null;
                else if (target.CompareTag(ENEMYBUILDING_TAG) && target.GetComponent<BuildingHealth>().GetCurrentHealth() <= 0)
                    this.target = null;
                else if (target.CompareTag(PREY_TAG) && target.GetComponent<preyAI>().GetHealth() <= 0) { 
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
            CancelInvoke("UpdatePath");
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }
    }

   

    void BuildTarget(Transform target)
    {
        buildCounter += Time.deltaTime;
        if(buildCounter >= 3f)
        {
            target.SendMessage("BuildMe", this.unitInfo.buildPower);
            if (target.GetComponent<Buildable>().GetBuildValue() >= target.GetComponent<Buildable>().GetMaxBuild())
                this.target = null;
            buildCounter = 0f;
        }


    }

    

    void ComputeMovement()
    {
        if(path == null)
            return;

        if(currentWaypoint >= path.vectorPath.Count)
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
        Debug.Log(force);
        //rb2d.AddForce(force);
        rb2d.velocity = force;

        float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        rb2d.velocity.Normalize();

    }

    void UpdateGFX()
    {
        //Flip updating
        if(rb2d.velocity.x >= 0.01f)
        {
            gfx.localScale = new Vector3(-1f, 1f, 1f);
        }    
        else if(rb2d.velocity.x <= -0.01f)
        {
            gfx.localScale = new Vector3(1f,1f,1f);
        }

        //Selection updating
        if(isSelected)
        {
            gfx.GetComponent<SpriteRenderer>().material = highlightedMaterial;
        }
        else 
        {
            gfx.GetComponent<SpriteRenderer>().material = defaultMaterial;
        }
    }

   

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void SetTarget(Transform t)
    {
        //Cancel path if existing
        /*if (seeker.GetCurrentPath() != null)
        {
            seeker.CancelCurrentPathRequest();
            this.rb2d.velocity = Vector2.zero;

        }*/
        //ClearPath();
        /*if (path != null)
        { 
            OnPathComplete(path);
            path = null;
        }*/

        destination = nullVector;
        oldDestination = nullVector;    

        this.oldTarget = this.target;
        this.target = t;
        if (t.tag == "Prey") lookingFor = RessourceTypes.ANIMAL;

        if (this.target != this.oldTarget && this.target.tag != "Enemy" && this.target.tag != "Prey")
        {
            //Debug.Log("test");
            atDestination = false;
            canMove = true;
        }
        else
        {
            atDestination = false;
            canMove = true;
        }
        CancelInvoke("UpdatePath");
        InvokeRepeating("UpdatePath",0f,0.5f);
    }

    public void SetDestination(Vector2 dest)
    {
        //Cancel path if existing
        /*if(seeker.GetCurrentPath() != null)
        {
            seeker.CancelCurrentPathRequest();
            this.rb2d.velocity = Vector2.zero;
            
        }*/



        //ClearPath();

        if (target != null)
            target = null;
        if(oldTarget != null)
            oldTarget = null; 

        this.oldDestination = this.destination;
        this.destination = dest;  

        if(this.destination != this.oldDestination)
        {
            atDestination = false;
            canMove = true;
        }
        CancelInvoke("UpdatePath");
        InvokeRepeating("UpdatePath",0f,0.5f);
    }
   

    public void ToggleSelected()
    {
        isSelected = !isSelected;
    }

    public Unit GetUnitInfo()
    {
        return this.unitInfo;
    }

    public int GetWood()
    {
        return this.ressourcesQuantity[0];
    }

    public int GetAnimal()
    {
        return this.ressourcesQuantity[1];
    }

    public int GetVegetal()
    {
        return this.ressourcesQuantity[2];
    }

    public int GetHealth()
    {
        return this.health;
    }

    public void ToggleInfoPanel()
    {
        infoPanelActive = !infoPanelActive;
        this.infoPanel.SetActive(infoPanelActive);
    }

    private void CapacityCheck()
    {
        currentCapacity = this.ressourcesQuantity[0] + this.ressourcesQuantity[1] + this.ressourcesQuantity[2];
        if(currentCapacity >= this.unitInfo.capacity)
        {
            isFull = true;
        }
        else
        {
            isFull = false;
        }
    }

    private bool IsTargetInRange()
    {
        return (Vector2.Distance(this.transform.position, target.position) <= range);
    }

    private void ClearPath()
    {
        // Abort any calculations in progress
        if (seeker != null) seeker.CancelCurrentPathRequest();
        canMove = false;
        atDestination = true;
        reachedEndOfPath = false;

        // Release current path so that it can be pooled
        /*if (path != null) path.Release(this);*/
        path = null;
        
    }

    public void UpgradeUnit(Unit newUnit)
    {
        this.unitInfo = newUnit;

        this.health = unitInfo.health;
        attackCounter = unitInfo.attackSpeed;
        consumeCounter = unitInfo.gatherSpeed;
        gfx.GetComponent<SpriteRenderer>().sprite = unitInfo.sprite;
    }

    public void TakeDamage(int dmg)
    {
       
        this.health -= dmg;
        if (this.health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager._instance.currentPopulation--;
        Destroy(this.gameObject);
    }
}
