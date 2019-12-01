using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RessourceTypes
    {
        WOOD,
        ANIMAL,
        VEGETAL
    }


public class Ressource : MonoBehaviour
{
    private int woodRegen;
    private int vegetalRegen;
    public RessourceTypes ressourceType;
    public int maxYield;
    public int currentYield;
    public float time = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        this.currentYield = this.maxYield;
        this.woodRegen = GameManager._instance.woodRegen;
        this.vegetalRegen = GameManager._instance.vegetalRegen;
    }

    // Update is called once per frame
    void Update()
    {
        if (ressourceType == RessourceTypes.WOOD || ressourceType == RessourceTypes.VEGETAL)
        {
            time += Time.deltaTime;
        }

        if (time >= 1.0f && currentYield<maxYield && ressourceType == RessourceTypes.WOOD)
        {
            time = 0.0f;
            currentYield += woodRegen;
        }

        if (time >= 1.0f && currentYield < maxYield && ressourceType == RessourceTypes.WOOD)
        {
            time = 0.0f;
            currentYield += vegetalRegen;
        }

        Mathf.Clamp(currentYield, 0, maxYield);
        CheckYield();
    }   
    public void Grow(int g)
    {

        if (ressourceType == RessourceTypes.WOOD)
        {
            maxYield += g;
        }
    }
    public void Consume(object[] a)
    {
        // Ressource[] rs = (this.GetComponents<Ressource>());
        if ((RessourceTypes)a[1] == ressourceType && currentYield>0)
        {
            currentYield -= (int)a[0];
        }
    }

    public int getYield()
    {
        return this.currentYield;
    }

    public int GetAnimalYield()
    {
        return this.maxYield;
    }

    public RessourceTypes GetRessourceType()
    {
        return this.ressourceType;
    }

    private void CheckYield()
    {
        Ressource[] s = gameObject.GetComponents<Ressource>();
        bool destroy = true;
        foreach (Ressource r in s)
        {
            if(r.currentYield > 0)
            {
                destroy = false;
            }
        }
        if (destroy)
        {
        
            Destroy(this.gameObject);
        }
    }

    
}
