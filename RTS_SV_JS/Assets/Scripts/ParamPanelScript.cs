using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ParamPanelScript : MonoBehaviour
{
    [SerializeField]
    Slider ressourceFarmSlider;

    [SerializeField]
    Slider woodSlider;

    [SerializeField]
    Slider vegetalSlider;

    [SerializeField]
    Slider birdSlider;

    [SerializeField]
    TextMeshProUGUI ressourceFarmText;

    [SerializeField]
    TextMeshProUGUI woodText;

    [SerializeField]
    TextMeshProUGUI vegetalText;

    [SerializeField]
    TextMeshProUGUI birdText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ressourceFarmText.text = ressourceFarmSlider.value.ToString();
        woodText.text = woodSlider.value.ToString();
        vegetalText.text = vegetalSlider.value.ToString();
        birdText.text = birdSlider.value.ToString();
    }

    public void SetRessourceFarm()
    {
        GameManager._instance.ressourceThreshold = (int) ressourceFarmSlider.value;
    }

    public void SetWoodRegen()
    {
        GameManager._instance.woodRegen = (int) woodSlider.value;
    }

    public void SetVegetalRegen()
    {
        GameManager._instance.vegetalRegen = (int)vegetalSlider.value;
    }

    public void SetBirdReprRate()
    {
        GameManager._instance.birdReprRate = birdSlider.value;
    }
}
