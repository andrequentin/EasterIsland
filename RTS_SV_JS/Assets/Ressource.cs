using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ressource : MonoBehaviour
{

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

    private void CheckYield()
    {
        if(currentYield <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
