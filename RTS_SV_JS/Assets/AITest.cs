using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//For instantianting units, the spawner will have an array with all the scriptable object types and will 
//assignate the Unit attribute of this class with the corresponding unit type

//TODO is capacity is full -> go to droppoint
public class AITest : MonoBehaviour
{
    public Unit unitInfo;

    //0 is for wood, 1 is for animal, 2 is for vegetal
    private int[] ressourcesQuantity = new int[3];

    [SerializeField]
    private Material highlightedMaterial;
    [SerializeField]
    private Material defaultMaterial;
    private const string RESSOURCE_TAG = "Ressource";
    private const string DROPPOINT_TAG = "DropPoint";
    private Rigidbody2D rb2d;
    public Transform target;
    public Transform oldTarget;
    
    public float nextWaypointDistance = 3f;

    
    public Transform gfx;
    private Path path;
    private int currentWaypoint;
    private bool reachedEndOfPath = false;
    private Seeker seeker;

    public bool canMove = true;
    private bool atDestination = false;

    
    public float consumeCounter = 0f;

    private bool isSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath",0f,0.5f);
        
    }

    void UpdatePath()
    {
        if(seeker.IsDone() && target != null)
            seeker.StartPath(rb2d.position, target.position, OnPathComplete);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canMove && target != null)
        {
            ComputeMovement();
        }
        
        //DO STUFF DEPENDING ON TYPE
        if(atDestination)
        {   
            if(target != null && target.gameObject.tag == RESSOURCE_TAG)
            {
                ConsumeRessource(target);
            }

            else if(target != null && target.gameObject.tag == DROPPOINT_TAG)
            {
                DropRessources(target);
            }
        }
        
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
        this.oldTarget = this.target;
        this.target = t;

        if(this.target != this.oldTarget)
        {
            Debug.Log("test");
            atDestination = false;
            canMove = true;
        }

        InvokeRepeating("UpdatePath",0f,0.5f);
    }

    public void ToggleSelected()
    {
        isSelected = !isSelected;
    }
}
