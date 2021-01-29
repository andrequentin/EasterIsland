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
    public TextMeshProUGUI score;
    public GameObject buildMenuPanel;
    public GameObject upgradeMenuPanel;
    public GameObject ForestMenuPanel;
    private bool isBuildPanelOpen = false;
    private bool isUpgradePanelOpen = false;
    private bool isForestPanelOpen = false;

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
        woodQuantity.text = ressourceManager.GetWood().ToString() + "/" + ressourceManager.GetMaxWood().ToString();
        animalQuantity.text = ressourceManager.GetAnimal().ToString() + "/" + ressourceManager.GetMaxAnimal().ToString();
        vegetalQuantity.text = ressourceManager.GetVegetal().ToString() + "/" + ressourceManager.GetMaxVegetal().ToString();
        populationQuantity.text = GameManager._instance.currentPopulation.ToString() + "/" + GameManager._instance.maxPopulation.ToString();
        score.text = GameManager._instance.GetDropPoint().GetComponent<DropPoint>().score.ToString();
    }

    public void ToggleBuildPanel()
    {
        isBuildPanelOpen = !isBuildPanelOpen;
        buildMenuPanel.GetComponent<Animator>().SetBool("isOpen", isBuildPanelOpen);
    }

    public void ToggleUpgradePanel()
    {
        isUpgradePanelOpen = !isUpgradePanelOpen;
        upgradeMenuPanel.GetComponent<Animator>().SetBool("isOpen", isUpgradePanelOpen);
    }
    public void ToggleForestPanel()
    {
        isForestPanelOpen = !isForestPanelOpen;
        ForestMenuPanel.GetComponent<Animator>().SetBool("isOpen", isForestPanelOpen);
    }

}
