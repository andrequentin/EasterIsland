using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestCanvas : MonoBehaviour
{

    public TextMeshProUGUI ressourceYield;
    public GameObject ressource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ressourceYield.text = "Ressource: " + ressource.GetComponent<Ressource>().getYield();
    }
}
