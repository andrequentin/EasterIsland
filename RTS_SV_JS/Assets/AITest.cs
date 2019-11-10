﻿using System.Collections;
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
    private const string POSITION_TAG = "Position";

    private Rigidbody2D rb2d;
    public Transform target;
    public Transform oldTarget;
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
    private Seeker seeker;

    public bool canMove = true;
    private bool atDestination = false;

    
    public float consumeCounter = 0f;
    public float attackCounter = 0f;

    private bool isSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        destination = nullVector;
        oldDestination = nullVector;
        InvokeRepeating("UpdatePath",0f,0.5f);
        this.health = unitInfo.health;
        
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

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(canMove && (target != null || destination != nullVector))
        {
            ComputeMovement();
        }
        
        //DO STUFF DEPENDING ON TYPE
        if(atDestination)
        {   
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

        if(consumeCounter >= unitInfo.gatherSpeed)
        {
            target.SendMessage("Consume",unitInfo.gatherTick);
            RessourceTypes rt = target.GetComponent<Ressource>().GetRessourceType();

            switch(rt)
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
        if(!target.GetComponent<DropPoint>().IsFull() && (target.GetComponent<DropPoint>().GetTotalCapacity() + totalCarry) < target.GetComponent<DropPoint>().GetMaxCapacity())
        {
            target.SendMessage("SetRessource", this.ressourcesQuantity);
            ressourcesQuantity[0] = 0;
            ressourcesQuantity[1] = 0;
            ressourcesQuantity[2] = 0;
        }
    }

    void AttackTarget(Transform target)
    {
        attackCounter += Time.deltaTime;

        if(attackCounter >= unitInfo.attackSpeed)
        {
            target.SendMessage("TakeDamage", unitInfo.damage);
            attackCounter = 0f;
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

        Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rb2d.position).normalized;
        Vector2 force = direction * unitInfo.speed * Time.deltaTime;
        
        rb2d.AddForce(force);

        float distance = Vector2.Distance(rb2d.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

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
        if(seeker.GetCurrentPath() != null)
        {
            seeker.CancelCurrentPathRequest();
        }

        destination = nullVector;
        oldDestination = nullVector;    

        this.oldTarget = this.target;
        this.target = t;

        if(this.target != this.oldTarget)
        {
            //Debug.Log("test");
            atDestination = false;
            canMove = true;
        }

        InvokeRepeating("UpdatePath",0f,0.5f);
    }

    public void SetDestination(Vector2 dest)
    {
        //Cancel path if existing
        if(seeker.GetCurrentPath() != null)
        {
            seeker.CancelCurrentPathRequest();
        }

        if(target != null)
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
}
