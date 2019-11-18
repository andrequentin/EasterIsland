using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The ressource manager
public class DropPoint : MonoBehaviour
{

    private int woodQuantity;
    private int animalQuantity;
    private int vegetalQuantity;

    public int woodMaxCapacity;
    public int animalMaxCapacity;
    public int vegetalMaxCapacity;

    public int maxCapacity;
    private int totalCapacity;

    private bool isFull = false;
    
    // Start is called before the first frame update
    void Start()
    {
        maxCapacity = woodMaxCapacity + animalMaxCapacity + vegetalMaxCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        //CheckCapacity();
    }

    private void CheckCapacity()
    {
        totalCapacity = 0;
        totalCapacity = woodQuantity + animalQuantity + vegetalQuantity;
        
        if(totalCapacity >= maxCapacity)
        {
            isFull = true;
        }
        else 
        {
            isFull = false;
        }
    }

    public void SetRessource(int[] ressources)
    {
        this.woodQuantity += ressources[0];
        this.animalQuantity += ressources[1];
        this.vegetalQuantity += ressources[2];
        CheckCapacity();
    }

    public bool IsFull()
    {
        return isFull;
    }

    public int GetTotalCapacity()
    {
        totalCapacity = woodQuantity + animalQuantity + vegetalQuantity;
        return totalCapacity;
    }

    public int GetMaxCapacity()
    {
        maxCapacity = woodMaxCapacity + animalMaxCapacity + vegetalMaxCapacity;
        return this.maxCapacity;
    }

    public int GetWood()
    {
        return this.woodQuantity;
    }

    public int GetAnimal()
    {
        return this.animalQuantity;
    }

    public int GetVegetal()
    {
        return this.vegetalQuantity;
    }

    public void ChangeWoodMaxCapacity(int d)
    {
        this.woodMaxCapacity += d;
        maxCapacity = woodMaxCapacity + animalMaxCapacity + vegetalMaxCapacity;
    }

    public void DecrementWood(int d)
    {
        this.woodQuantity -= d;
    }

    public void DecrementAnimal(int d)
    {
        this.animalQuantity -= d;
    }

    public void DecrementVegetal(int d)
    {
        this.vegetalQuantity -= d;
    }
}
