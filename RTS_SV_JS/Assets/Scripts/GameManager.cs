using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    private GameObject endGamePanel;
    private bool endGamePanelDetected = false;

    [SerializeField]
    GameObject dropPoint;
    public int wood;
    public int animal;
    public int vegetal;
    public int maxPopulation;
    public int currentPopulation;

    public bool wonGame = false;
    public bool lostGame = false;
    public bool lostGameByRessource = false;
    private int forestCount = 0;
    private int birdCount = 0;


    public int ressourceThreshold;
    public int woodRegen;
    public int vegetalRegen;
    public float birdReprRate;
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
        if(SceneManager.GetActiveScene().buildIndex == 1 && !endGamePanelDetected)
        {
            endGamePanel = GameObject.FindGameObjectWithTag("EndGamePanel");
            endGamePanelDetected = true;
            endGamePanel.SetActive(false);

            dropPoint = GameObject.FindGameObjectWithTag("DropPoint");
            maxPopulation = 5;
            currentPopulation = FindObjectsOfType<AITest>().Length;
        }

        if(SceneManager.GetActiveScene().buildIndex == 1)
            UpdateRessources();

        if(wonGame)
        {
            lostGame = false;
            Time.timeScale = 0;
            endGamePanel.SetActive(true);
            endGamePanel.SendMessage("SetWonPanel");
        }
        if(lostGame)
        {
            wonGame = false;
            Time.timeScale = 0;
            endGamePanel.SetActive(true);
            endGamePanel.SendMessage("SetLostPanel");
        }

        forestCount = GameObject.FindGameObjectsWithTag("Ressource").Length;
        birdCount = GameObject.FindGameObjectsWithTag("Prey").Length;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (forestCount <= 0 || birdCount <= 1)
            {
                lostGameByRessource = true;
                wonGame = false;
                lostGame = false;
                Time.timeScale = 0;
                endGamePanel.SetActive(true);
                endGamePanel.SendMessage("SetLostPanelByRessource");
            }
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
