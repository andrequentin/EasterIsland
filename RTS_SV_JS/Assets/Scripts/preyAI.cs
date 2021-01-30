using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyAI : MonoBehaviour
{
   
    private int health;
    public bool lookingformate = false;

    public bool isAdult = false;

    public float growTimer;
    public float mateTimer;
    public float timeLeftMax=15f;

    [SerializeField]
    private int mated = 0;
    private float range = 1.5f;

    private GameObject mate;
    private bool hasMate = false;

    private int time = 0; // movement time

    
    public bool isMale;


    public GameObject pref;

  
    private bool alive = true;
    private Rigidbody2D rb2d;
    public Transform gfx;

    private Vector2 nullVector { get { return new Vector2(-9999, -9999); } }

    // Start is called before the first frame update
    void Start()
    {
        timeLeftMax = GameManager._instance.birdReprRate;
        mated = 0;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.drag = 5;
        growTimer = timeLeftMax;
        isAdult = false;
        rb2d.gravityScale = 0;
        this.health = 2;
        this.time = 0;
        //StartCoroutine("LoseTime");
        if (!isMale)
            gfx.GetComponent<SpriteRenderer>().color = new Color(255, 0, 151);
    
    }   

    // Update is called once per frame
    void Update()
    {

        UpdateGFX();
    }
    void UpdateGFX()
    {
        if (!isAdult) 
        {
            gfx.transform.localScale = new Vector3(0.5f, 0.5f, 1f); 
        }
        else 
        { 
            gfx.transform.localScale = new Vector3(1f, 1f, 1f);
        }

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
            if (!isAdult)
            {
                growTimer -= Time.deltaTime;
                if (growTimer <= 0)
                {
                    isAdult = true;
                    lookingformate = true;
                    mateTimer = 0;
                    mate = null;
                    hasMate = false;
                    mated = 0;
                }

            }

            mateTimer -= Time.deltaTime;
            if (isAdult && isMale && lookingformate && mated <= 10 && mateTimer <= 0)
            {
                if (!hasMate || mate==null || !mate.GetComponent<preyAI>().alive)
                {
                    GameObject[] possibleMates = GameObject.FindGameObjectsWithTag("Prey");
                    float minDIst = Mathf.Infinity;
                    foreach (GameObject m in possibleMates)
                    {
                        if (!m.GetComponent<preyAI>().isMale)
                        {
                            float dist = Vector3.Distance(m.GetComponent<Transform>().localPosition, this.transform.localPosition);
                            if (m.GetComponent<preyAI>().GetInstanceID() != this.GetInstanceID() && dist < minDIst && !m.GetComponent<preyAI>().hasMate && m.GetComponent<preyAI>().mated <= 3 && m.GetComponent<preyAI>().alive && m.GetComponent<preyAI>().isAdult)
                            {
                                mate = m;
                                hasMate = true;

                                m.GetComponent<preyAI>().hasMate = true;
                                m.GetComponent<preyAI>().mate = this.gameObject;
                            }
                        }
                    }

                }
                else
                {
                    if (IsTargetInRange())
                    {
                        mate.GetComponent<preyAI>().mated++;
                        mate.GetComponent<preyAI>().hasMate = false;
                        mate.GetComponent<preyAI>().mate = null;

                        mated++;
                        hasMate = false;
                        mate = null;

                        if (GameObject.FindGameObjectsWithTag("Prey").Length < 40)
                        {
                            Vector3 pos = this.transform.position;
                            pos.z = -1;
                            GameObject temp = Instantiate(pref, pos, Quaternion.identity);
                            int sex = Random.Range(0, 2);
                            if (sex == 0)
                                temp.GetComponent<preyAI>().isMale = true;
                            else if(sex == 1)
                                temp.GetComponent<preyAI>().isMale = false;
                        }

                        mateTimer = timeLeftMax;
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
        if (!alive)
        {
            if (this.mate != null)
            {
                
                this.mate.GetComponent<preyAI>().hasMate = false;
                this.mate.GetComponent<preyAI>().mate = null;
                hasMate = false;
                mate = null;
            }
            Destroy(this.gameObject);
        }
    }
    private bool IsTargetInRange()
    {
        return (Vector2.Distance(this.transform.position, mate.GetComponent<Transform>().localPosition) <= range);
    }

    /*IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;

        }
    }*/
}
