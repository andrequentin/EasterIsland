using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyAI : MonoBehaviour
{



    private int health;
    int time ;
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
        rb2d.drag = 20;
        
        rb2d.gravityScale = 0;
        this.health = 2;
        this.time = 0;

    }

    // Update is called once per frame
    void Update()
    {

        UpdateGFX();
    }
    void UpdateGFX()
    {

        //Flip updating
        if (rb2d.velocity.x >= 0.01f)
        {
            gfx.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb2d.velocity.x <= -0.01f)
        {
            gfx.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
    void FixedUpdate()
    {
        if (alive)
        {
            if (time == 30)
            {
                Vector2 mv = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                rb2d.velocity = mv;

                time = 0;
            }
            else
            {
                time++;
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
}
