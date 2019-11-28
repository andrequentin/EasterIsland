using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treesRessources : MonoBehaviour
{
    public Ressource vegetal;
    public Ressource wood;
    // Start is called before the first frame update
    void Start()
    {
        vegetal = new Ressource();
        wood = new Ressource();
        wood.ressourceType = RessourceTypes.WOOD;
        vegetal.ressourceType = RessourceTypes.VEGETAL;
        wood.maxYield = 40;
        wood.currentYield = 40;

        vegetal.maxYield = 20;
        vegetal.currentYield = 20;
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.Clamp(vegetal.currentYield, 0, vegetal.maxYield);
        Mathf.Clamp(wood.currentYield, 0, wood.currentYield);

        CheckYield();
    }
    public int getYield()
    {
        if (vegetal.currentYield > 0)
        {
            return vegetal.currentYield;
        }
        else
        {
            return wood.currentYield;
        }
    }

    public void Consume(int value)
    {
        if (vegetal.currentYield > 0)
        {
            vegetal.currentYield -= value;
        }
        else
        {
            wood.currentYield -= value;
        }
        //print(wood.currentYield);
        //print(vegetal.currentYield);
    }



    public RessourceTypes GetRessourceType()
    {
        if(vegetal.currentYield > 0)
        {
            return vegetal.GetRessourceType();
        }
        return wood.GetRessourceType() ;
    }
    private void CheckYield()
    {
        if (wood.currentYield <= 0 && vegetal.currentYield <= 0)
        {

            Destroy(this.gameObject);
        }
    }
}
