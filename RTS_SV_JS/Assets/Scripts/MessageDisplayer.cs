using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageDisplayer : MonoBehaviour
{ 

    public static MessageDisplayer _instance;
    public TextMeshProUGUI messageZone;


    [SerializeField]
    private float fadeOutTime;
    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }


    public void DisplayMessage(string message)
    {
        this.messageZone.text = message;
        messageZone.CrossFadeAlpha(1f, 0.1f, false);
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(2f);
        
        messageZone.CrossFadeAlpha(0f, fadeOutTime, false);
    }
}
