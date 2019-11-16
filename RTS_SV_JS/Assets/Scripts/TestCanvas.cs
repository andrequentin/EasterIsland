using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestCanvas : MonoBehaviour
{

    public DropPoint ressourceManager;
    public TextMeshProUGUI woodQuantity;
    public TextMeshProUGUI animalQuantity;
    public TextMeshProUGUI vegetalQuantity;
    public TextMeshProUGUI populationQuantity;
    public GameObject buildMenuPanel;
    private bool isBuildPanelOpen = false;
    
    // Start is called before the first frame update
    void Start()
    {
        woodQuantity.text = 0.ToString();;
        animalQuantity.text = 0.ToString();
        vegetalQuantity.text = 0.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        woodQuantity.text = ressourceManager.GetWood().ToString();
        animalQuantity.text = ressourceManager.GetAnimal().ToString();
        vegetalQuantity.text = ressourceManager.GetVegetal().ToString();
        populationQuantity.text = GameManager._instance.currentPopulation.ToString() + "/" + GameManager._instance.maxPopulation.ToString();
    }

    public void ToggleBuildPanel()
    {
        isBuildPanelOpen = !isBuildPanelOpen;
        buildMenuPanel.GetComponent<Animator>().SetBool("isOpen", isBuildPanelOpen);
    }




}
