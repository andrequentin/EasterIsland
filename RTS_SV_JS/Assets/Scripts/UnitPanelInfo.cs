using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitPanelInfo : MonoBehaviour
{
    Unit unitInfo;
    AITest unitAI;

    [SerializeField]
    private TextMeshProUGUI unitType;
    [SerializeField]
    private TextMeshProUGUI unitHp;
    [SerializeField]
    private TextMeshProUGUI unitDmg;
    [SerializeField]
    private TextMeshProUGUI wood;
    [SerializeField]
    private TextMeshProUGUI animal;
    [SerializeField]
    private TextMeshProUGUI vegetal;

    // Start is called before the first frame update
    void Start()
    {
        unitAI = GetComponentInParent<AITest>();
        unitInfo = unitAI.GetUnitInfo();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePanel();
    }

    void UpdatePanel()
    {
        this.unitType.text = "Unit type: " + this.unitInfo.unitType;
        this.unitHp.text = "Unit health: " + unitAI.GetHealth();
        this.unitDmg.text = "Unit damage: " + this.unitInfo.damage;

        this.wood.text = "Wood: " + unitAI.GetWood();
        this.animal.text = "Animal: " + unitAI.GetAnimal();
        this.vegetal.text = "Vegetal: " + unitAI.GetVegetal();
    }
}
