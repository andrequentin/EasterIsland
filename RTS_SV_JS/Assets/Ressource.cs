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
    
    public RessourceTypes ressourceType;
    public int maxYield;
    public int currentYield;
    // Start is called before the first frame update
    void Start()
    {
        this.currentYield = this.maxYield;
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(currentYield,0,maxYield);
        CheckYield();
    }

    public void Consume(int value)
    {
        currentYield -= value;
    }

    public int getYield()
    {
        return this.currentYield;
    }

    public RessourceTypes GetRessourceType()
    {
        return this.ressourceType;
    }

    private void CheckYield()
    {
        if(currentYield <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    
}
