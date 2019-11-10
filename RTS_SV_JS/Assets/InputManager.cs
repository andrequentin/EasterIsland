using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const string SELECTABLE_TAG = "Selectable";
    private const string RESSOURCE_TAG = "Ressource";
    private const string GROUND_TAG = "Ground";
    private const string DROPPOINT_TAG = "DropPoint";
    private const string ENEMY_TAG = "Enemy";

    public GameObject selectedObject;
    public List<GameObject> selectedObjects;
    private GameObject[] units;

    [SerializeField]
    private GameObject goToObject;
    private Rect selectBox;
    private Vector2 boxStartingPosition;
    private Vector2 boxEndingPosition;
    [SerializeField]
    private Texture boxTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            ToggleInfoPanel();
        }

        if(Input.GetMouseButtonDown(0))
        {
            LeftClick();
        }

        if(Input.GetMouseButton(0) && boxStartingPosition == Vector2.zero)
        {
            boxStartingPosition = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0) && boxStartingPosition != Vector2.zero)
        {
            boxEndingPosition = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            units = GameObject.FindGameObjectsWithTag(SELECTABLE_TAG);
            MultiSelect();
            
        }

        if(selectedObjects.Capacity > 0  && Input.GetMouseButtonDown(1))
        {
            RightClick();
        }

        selectBox = new Rect(boxStartingPosition.x,Screen.height - boxStartingPosition.y, boxEndingPosition.x - boxStartingPosition.x, -1 * ((Screen.height - boxStartingPosition.y) - (Screen.height - boxEndingPosition.y)));
    }

    public void MultiSelect()
    {
        foreach(GameObject u in units)
        {
            Vector2 unitPosition = Camera.main.WorldToScreenPoint(u.transform.position);
            if(selectBox.Contains(unitPosition,true))
            {
                selectedObjects.Add(u);
                u.SendMessage("ToggleSelected");
            }
        }

        boxStartingPosition = Vector2.zero;
        boxEndingPosition = Vector2.zero;
    }

    void LeftClick()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up);

        if(hit.collider != null && hit.transform.tag == SELECTABLE_TAG /*&& selectedObject==null*/)
        {
            
            //selectedObject = hit.collider.gameObject;
            hit.collider.gameObject.SendMessage("ToggleSelected");
            selectedObjects.Add(hit.collider.gameObject);
            //selectedObject.SendMessage("ToggleSelected");
        }
        // else if(hit.collider != null && hit.transform.tag == SELECTABLE_TAG)
        // {
        //     selectedObject.SendMessage("ToggleSelected");
        //     selectedObject = hit.collider.gameObject;
        //     selectedObject.SendMessage("ToggleSelected");
        // }

        else if(hit.transform.tag == GROUND_TAG && /*selectedObject != null*/ selectedObjects.Capacity > 0)
        {
            //selectedObject.SendMessage("ToggleSelected");
            //selectedObject = null;
            foreach(GameObject u in selectedObjects)
            {
                u.SendMessage("ToggleSelected");
            }
            selectedObjects.Clear();
        }
    }

    void RightClick()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up);
        Debug.Log(hit.collider.gameObject);
        if(hit.collider != null && (hit.transform.tag == RESSOURCE_TAG || hit.transform.tag == DROPPOINT_TAG || hit.transform.tag == ENEMY_TAG))
        {
            //selectedObject.SendMessage("SetTarget", hit.transform);
            foreach(GameObject u in selectedObjects)
            {
                Debug.Log("test");
                u.SendMessage("SetTarget", hit.transform);
            }
        }
        else if(hit.collider != null && hit.transform.tag == GROUND_TAG)
        {
            //selectedObject.SendMessage("SetDestination", origin);
            foreach(GameObject u in selectedObjects)
            {
                u.SendMessage("SetDestination", origin);
            }
        }

       
    }

    void ToggleInfoPanel()
    {
        if(selectedObjects.Capacity > 0)
        {
            foreach(GameObject u in selectedObjects)
            {
                u.SendMessage("ToggleInfoPanel");
            }
        }
    }

    void OnGUI()
    {
        if(boxStartingPosition != Vector2.zero && boxEndingPosition != Vector2.zero)
        {
            //(boxStartingPosition.x,Screen.height - boxStartingPosition.y, boxEndingPosition.x - boxStartingPosition.x, -1 * ((Screen.height - boxStartingPosition.y) - (Screen.height - boxEndingPosition.y)))
            GUI.DrawTexture(selectBox, boxTexture);
        }
    }
}
