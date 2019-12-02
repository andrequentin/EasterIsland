using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField]
    private bool isEnemy;

    [SerializeField]
    private int maxHealth;

    [SerializeField]
    private bool isNexus;

    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        this.currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Collapse();
        }
    }

    public void TakeDamage(int dmg)
    {
        this.currentHealth -= dmg;
        if(currentHealth <= 0)
        {
            Collapse();
        }
    }

    public int GetCurrentHealth()
    {
        return this.currentHealth;
    }

    private void Collapse()
    {
        if(GetComponent<House>())
        {
            if (isEnemy)
                EasyAI._instance.IncreasePopulation(-2);
            else
                GameManager._instance.IncreasePopulation(-2);
        }
        else if(GetComponent<Sawmill>())
        {
            if (isEnemy)
                EasyAI._instance.ChangeWoodMaxCapacity(-50);
            else
                GameManager._instance.GetDropPoint().GetComponent<DropPoint>().ChangeWoodMaxCapacity(-50);
        }
        else if(GetComponent<Granary>())
        {
            if(isEnemy)
            {
                EasyAI._instance.ChangeAnimalMaxCapacity(-50);
                EasyAI._instance.ChangeVegetalMaxCapacity(-50);
            }
            else
            {
                GameManager._instance.GetDropPoint().GetComponent<DropPoint>().ChangeAnimalMaxCapacity(-50);
                GameManager._instance.GetDropPoint().GetComponent<DropPoint>().ChangeVegetalMaxCapacity(-50);
            }
        }
        //Else if for the other buildings
        else if(isNexus)
        {
            if (isEnemy)
                GameManager._instance.wonGame = true;
            else
                GameManager._instance.lostGame = true;
        }
        if(!isNexus)
            Destroy(this.gameObject);
    }

    
}
