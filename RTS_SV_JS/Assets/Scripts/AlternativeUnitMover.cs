using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//For instantianting units, the spawner will have an array with all the scriptable object types and will 
//assignate the Unit attribute of this class with the corresponding unit type

//TODO is capacity is full -> go to droppoint


public class AlternativeUnitMover : MonoBehaviour
{
    //STATS
    public Unit unitInfo;
    private int health;
    private int currentCapacity;
    private bool isFull = false;
    public float range = 1.5f;

    [SerializeField]
    Transform blankTarget;

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

    private Rigidbody2D rb2d;
   
    public Vector2 destination;
    public Vector2 oldDestination;

    private Vector2 nullVector { get { return new Vector2(-9999, -9999); } }
    public float nextWaypointDistance = 3f;


    public Transform gfx;


    [SerializeField]
    private GameObject infoPanel;
    private bool infoPanelActive = false;

    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;
    //private Seeker seeker;

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
        //seeker = GetComponent<Seeker>();
        destination = nullVector;
        oldDestination = nullVector;
        //InvokeRepeating("UpdatePath", 0f, 0.5f);
        this.health = unitInfo.health;
        attackCounter = unitInfo.attackSpeed;
        consumeCounter = unitInfo.gatherSpeed;

    }

    /*void UpdatePath()
    {
        if (seeker.IsDone() && target != null)
            seeker.StartPath(rb2d.position, target.position, OnPathComplete);

        else if ((seeker.IsDone() && destination != nullVector))
        {
            seeker.StartPath(rb2d.position, destination, OnPathComplete);

        }
    }*/

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

        /*if (canMove && (target != null || destination != nullVector))
        {
            ComputeMovement();
        }*/

        //DO STUFF DEPENDING ON TYPE
        if (atDestination)
        {
            //rb2d.velocity = Vector2.zero;
            if (GetComponent<AIDestinationSetter>().target != null && GetComponent<AIDestinationSetter>().target.gameObject.tag == RESSOURCE_TAG && !isFull)
            {
                ConsumeRessource(GetComponent<AIDestinationSetter>().target);
            }

            else if (GetComponent<AIDestinationSetter>().target != null && GetComponent<AIDestinationSetter>().target.gameObject.tag == DROPPOINT_TAG)
            {
                DropRessources(GetComponent<AIDestinationSetter>().target);
            }
            else if (GetComponent<AIDestinationSetter>().target != null && GetComponent<AIDestinationSetter>().target.gameObject.tag == ENEMY_TAG)
            {
                AttackTarget(GetComponent<AIDestinationSetter>().target);
            }
            else if (GetComponent<AIDestinationSetter>().target != null && GetComponent<AIDestinationSetter>().target.gameObject.tag == PREY_TAG)
            {
                if (GetComponent<AIDestinationSetter>().target.GetComponent<preyAI>().GetHealth() > 0)
                {
                    AttackTarget(GetComponent<AIDestinationSetter>().target);
                }
                else
                {
                    ConsumeRessource(GetComponent<AIDestinationSetter>().target);
                }
            }
            else if (GetComponent<AIDestinationSetter>().target != null && GetComponent<AIDestinationSetter>().target.gameObject.tag == ENEMYBUILDING_TAG)
            {
                AttackTarget(GetComponent<AIDestinationSetter>().target);
            }
            else if (GetComponent<AIDestinationSetter>().target != null && GetComponent<AIDestinationSetter>().target.gameObject.tag == BUILDABLE_TAG)
            {
                BuildTarget(GetComponent<AIDestinationSetter>().target);
            }
            else if (destination != nullVector)
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
            target.SendMessage("Consume", unitInfo.gatherTick);
            if (target.GetComponent<Ressource>().getYield() <= 0)
            {
                target.GetComponent<preyAI>().Die(); this.GetComponent<AIDestinationSetter>().target = null;
            }
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

    void DropRessources(Transform target)
    {
        int totalCarry = ressourcesQuantity[0] + ressourcesQuantity[1] + ressourcesQuantity[2];
        if (!target.GetComponent<DropPoint>().IsFull() && (target.GetComponent<DropPoint>().GetTotalCapacity() + totalCarry) < target.GetComponent<DropPoint>().GetMaxCapacity())
        {
            //Debug.Log(this.ressourcesQuantity);
            target.SendMessage("SetRessource", this.ressourcesQuantity);
            ressourcesQuantity[0] = 0;
            ressourcesQuantity[1] = 0;
            ressourcesQuantity[2] = 0;
        }
    }


    //SET A RANGE ATTRIBUTE AND TEST ON RANGE OR ELSE THEY WILL ATTACK FROM AFAR
    void AttackTarget(Transform target)
    {
        if (IsTargetInRange())
        {
            if (attackCounter >= unitInfo.attackSpeed)
            {
                target.SendMessage("TakeDamage", unitInfo.damage);
                if (target.CompareTag(ENEMY_TAG) && target.GetComponent<MultiAgentFSM>().GetCurrentHealth() <= 0)
                    this.GetComponent<AIDestinationSetter>().target = null;
                else if (target.CompareTag(ENEMYBUILDING_TAG) && target.GetComponent<BuildingHealth>().GetCurrentHealth() <= 0)
                    this.GetComponent<AIDestinationSetter>().target = null;
                else if (target.CompareTag(PREY_TAG) && target.GetComponent<preyAI>().GetHealth() <= 0)
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
            //Debug.Log("Target gone far");
            InvokeRepeating("UpdatePath", 0f, 0.5f);
        }
    }



    void BuildTarget(Transform target)
    {
        buildCounter += Time.deltaTime;
        if (buildCounter >= 3f)
        {
            target.SendMessage("BuildMe", this.unitInfo.buildPower);
            if (target.GetComponent<Buildable>().GetBuildValue() >= target.GetComponent<Buildable>().GetMaxBuild())
                this.GetComponent<AIDestinationSetter>().target = null;
            buildCounter = 0f;
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

        //Selection updating
        if (isSelected)
        {
            gfx.GetComponent<SpriteRenderer>().material = highlightedMaterial;
        }
        else
        {
            gfx.GetComponent<SpriteRenderer>().material = defaultMaterial;
        }
    }



   

    public void SetTarget(Transform t)
    {
        GetComponent<AIDestinationSetter>().target = t;
    }

    public void SetDestination(Vector2 dest)
    {
        //Cancel path if existing
        /*if(seeker.GetCurrentPath() != null)
        {
            seeker.CancelCurrentPathRequest();
            this.rb2d.velocity = Vector2.zero;
            
        }*/

        blankTarget.parent = null;
        blankTarget.position = dest;
        
        GetComponent<AIDestinationSetter>().target = blankTarget;
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
        if (currentCapacity >= this.unitInfo.capacity)
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
        return (Vector2.Distance(this.transform.position, GetComponent<AIDestinationSetter>().target.position) <= range);
    }

   
}
