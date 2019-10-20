using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const string SELECTABLE_TAG = "Selectable";
    private const string RESSOURCE_TAG = "Ressource";
    private const string GROUND_TAG = "Ground";
    private const string DROPPOINT_TAG = "DropPoint";

    public GameObject selectedObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }

        if(selectedObject != null && Input.GetMouseButtonDown(1))
        {
            RightClick();
        }
    }

    void LeftClick()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up);

        if(hit.collider != null && hit.transform.tag == SELECTABLE_TAG)
        {
            selectedObject = hit.collider.gameObject;
            selectedObject.SendMessage("ToggleSelected");
        }

        else if(hit.transform.tag == GROUND_TAG)
        {
            selectedObject.SendMessage("ToggleSelected");
            selectedObject = null;
        }
    }

    void RightClick()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up);

        if(hit.collider != null && (hit.transform.tag == RESSOURCE_TAG || hit.transform.tag == DROPPOINT_TAG))
        {
            selectedObject.SendMessage("SetTarget", hit.transform);
        }

        //Later for moving units, we create an empty gameobject as checkpoint and set it as target
    }
}
