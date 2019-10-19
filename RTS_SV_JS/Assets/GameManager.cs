using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager _instance;

    private int woodRessource = 0;
    private int animalFoodRessource = 0;
    private int vegatalFoodRessource = 0;
    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getWoodRessource()
    {
        return this.woodRessource;
    }
}
