using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    [SerializeField]
    private int maxBuild;
    private int currentBuildValue = 0;

    [SerializeField]
    GameObject finalResult;
    // Start is called before the first frame update
    private float timer = 1;
    private void Update()
    {
        if (this.tag == "forestFoundation")
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                BuildMe(1);
                timer = 1;
            }
        }
        if (currentBuildValue >= maxBuild)
        {
            Instantiate(finalResult, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            
        }
        
    }

    public void BuildMe(int b)
    {
        currentBuildValue += b;
    }

    public int GetBuildValue()
    {
        return this.currentBuildValue;
    }

    public int GetMaxBuild()
    {
        return this.maxBuild;
    }
}
