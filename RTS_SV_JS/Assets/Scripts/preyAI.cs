using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyAI : MonoBehaviour
{
   
    private int health;
    public int lookingformate = 0;
    public int timeLeft; //grow and mate time
    public int timeLeftMax=60;
    private int mated = 0;
    private float range = 1.5f;
    private GameObject mate;
    private bool hasMate = false;
    private int time = 0; // movement time

    
    public GameObject pref;

    [SerializeField]
    private GameObject infoPanel;
    private bool infoPanelActive = false;
    private bool alive = true;
    private Rigidbody2D rb2d;
    public Transform gfx;

    private Vector2 nullVector { get { return new Vector2(-9999, -9999); } }

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.drag = 5;
        timeLeft = timeLeftMax;
        rb2d.gravityScale = 0;
        this.health = 2;
        this.time = 0;
        StartCoroutine("LoseTime");
        
    
    }   

    // Update is called once per frame
    void Update()
    {

        UpdateGFX();
    }
    void UpdateGFX()
    {
        if (lookingformate <= 0) { gfx.transform.localScale = new Vector3(0.5f, 0.5f, 1f); }
        else { gfx.transform.localScale = new Vector3(1f, 1f, 1f); }
        //Flip updating
        if (rb2d.velocity.x >= 0.01f)
        {
            Vector3 n = gfx.localScale;
            n.x = Mathf.Abs(n.x);
            gfx.localScale = n;
        }
        else if (rb2d.velocity.x <= -0.01f)
        {
            Vector3 n = gfx.localScale;
            n.x = Mathf.Abs(n.x)*-1;
            gfx.localScale = n;
        }
    }
    void FixedUpdate()
    {
        if (alive)
        {
            if (timeLeft <= 0) { lookingformate += 1; timeLeft = timeLeftMax; mate = null; hasMate = false; }


            if (lookingformate > 1 && lookingformate >= mated)
            {
                if (!hasMate || !mate.GetComponent<preyAI>().alive)
                {
                    GameObject[] possibleMates = GameObject.FindGameObjectsWithTag("Prey");
                    float minDIst = Mathf.Infinity;
                    foreach (GameObject m in possibleMates)
                    {
                        float dist = Vector3.Distance(m.GetComponent<Transform>().localPosition, this.transform.localPosition);
                        if (m.GetComponent<preyAI>().GetInstanceID() != this.GetInstanceID() && dist < minDIst && m.GetComponent<preyAI>().lookingformate > 1 && m.GetComponent<preyAI>().lookingformate > m.GetComponent<preyAI>().mated && m.GetComponent<preyAI>().alive)
                        {
                            mate = m;
                            hasMate = true;
                        }
                    }

                }
                else
                {
                    if (IsTargetInRange())
                    {
                        mated++;
                        hasMate = false;
                        mate = null;

                        Instantiate(pref);
                        
                    }
                    else
                    {
                        if (time >= 30)
                        {
                            Vector2 direction = mate.GetComponent<Transform>().localPosition - this.transform.position  ;
                            rb2d.velocity = direction.normalized;
                            time = 0;
                        }
                        else
                        {
                            time++;
                        }

                    }

                }
            }
            else
            {

                if (time >= 30)
                {
                    Vector2 mv = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    rb2d.velocity = mv.normalized;
                    time = 0;
                }
                else
                {
                    time++;
                }
            }
        }
    }
   
    public int GetHealth()
    {
        return this.health;
    }
    public void TakeDamage(int dmg)
    {
        this.health -= dmg;
        if (this.health <= 0)
        {
            //Die();
            alive = false;
        }
    }
    public void Die()
    {   
        if(!alive)  Destroy(this.gameObject);
    }
    private bool IsTargetInRange()
    {
        return (Vector2.Distance(this.transform.position, mate.GetComponent<Transform>().localPosition) <= range);
    }
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;

        }
    }
}
