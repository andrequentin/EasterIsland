using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [SerializeField]
    GameObject dropPoint;
    public int wood;
    public int animal;
    public int vegetal;
    public int maxPopulation;
    public int currentPopulation;

    public bool wonGame = false;
    public bool lostGame = false;
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
        dropPoint = GameObject.FindGameObjectWithTag("DropPoint");
        maxPopulation = 5;
        currentPopulation = FindObjectsOfType<AITest>().Length;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRessources();

        if(wonGame)
        {
            lostGame = false;
            Time.timeScale = 0;
            GameObject.FindGameObjectWithTag("EndGamePanel").SetActive(true);
            GameObject.FindGameObjectWithTag("EndGamePanel").SendMessage("SetWonPanel");
        }
        if(lostGame)
        {
            wonGame = false;
            Time.timeScale = 0;
            GameObject.FindGameObjectWithTag("EndGamePanel").SetActive(true);
            GameObject.FindGameObjectWithTag("EndGamePanel").SendMessage("SetLostPanel");
        }
    }

    private void UpdateRessources()
    {
        this.wood = dropPoint.GetComponent<DropPoint>().GetWood();
        this.animal = dropPoint.GetComponent<DropPoint>().GetAnimal();
        this.vegetal = dropPoint.GetComponent<DropPoint>().GetVegetal();
    }

    public void IncreasePopulation(int pop)
    {
        this.maxPopulation += pop;
    }

    public void IncreaseCurrentPopulation()
    {
        this.currentPopulation++;
    }

    public GameObject GetDropPoint()
    {
        return this.dropPoint;
    }

  
}
