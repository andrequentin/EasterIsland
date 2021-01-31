﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private const string SELECTABLE_TAG = "Selectable";
    private const string RESSOURCE_TAG = "Ressource";
    private const string GROUND_TAG = "Ground";
    private const string DROPPOINT_TAG = "DropPoint";
    private const string ENEMY_TAG = "Enemy";
    private const string PREY_TAG = "Prey";
    private const string BUILDABLE_TAG = "Buildable";
    private const string ENEMYBUILDING_TAG = "EnemyBuilding";
    private const string ENEMYNEXUS_TAG = "EnemyDropPoint";
    private bool isMultiSelecting = false;
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
    private bool buildSelected = false;
    private bool upgradeSelected = false;
    public bool forestSelected = false;
    GameObject buildingToPut = null;
    bool buildingPending = false;
    int buildingCost;
    public void SetBuildingToPut(GameObject b)
    {
        this.buildingToPut = b;
        buildingPending = true;
    }

    public void SetBuildingToPutCost(int c)
    {
        this.buildingCost = c;
    }

    public void PutBuilding()
    {
        if (buildingToPut.tag == "forestFoundation")
        {
            if (GameManager._instance.vegetal >= buildingCost)
            {
                Vector2 putPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Instantiate(buildingToPut, putPosition, Quaternion.identity);
                this.buildingToPut = null;
                buildingPending = false;
                GameObject.FindGameObjectWithTag("DropPoint").SendMessage("DecrementVegetal", buildingCost);
                buildingCost = 0;
            }
            else
            {
                MessageDisplayer._instance.DisplayMessage("Not enough vegetal");
                this.buildingToPut = null;
                buildingPending = false;
                buildingCost = 0;
            }
        }
        else
        {
            if (GameManager._instance.wood >= buildingCost)
            {
                Vector2 putPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Instantiate(buildingToPut, putPosition, Quaternion.identity);
                this.buildingToPut = null;
                buildingPending = false;
                GameObject.FindGameObjectWithTag("DropPoint").SendMessage("DecrementWood", buildingCost);
                buildingCost = 0;
            }
            else
            {
                MessageDisplayer._instance.DisplayMessage("Not enough wood");
                this.buildingToPut = null;
                buildingPending = false;
                buildingCost = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        { 
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInfoPanel();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                upgradeSelected = true;
                GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleUpgradePanel");
            }

            if(Input.GetKeyDown(KeyCode.M))
            {
                Time.timeScale++;
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                Time.timeScale--;
            }

            if (Input.GetMouseButtonDown(0) && !buildingPending)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                    LeftClick();
            }
            else if (Input.GetMouseButtonDown(0) && buildingPending)
            {
                PutBuilding();
            }

            if (Input.GetMouseButton(0) && boxStartingPosition == Vector2.zero)
            {
                isMultiSelecting = true;
                boxStartingPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && boxStartingPosition != Vector2.zero)
            {
                boxEndingPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0) && !buildingPending)
            {
                units = GameObject.FindGameObjectsWithTag(SELECTABLE_TAG);
                MultiSelect();
                isMultiSelecting = false;
            }

            if (selectedObjects.Count > 0 && Input.GetMouseButtonDown(1))
            {
                RightClick();
            }
            selectBox = new Rect(boxStartingPosition.x,Screen.height - boxStartingPosition.y, boxEndingPosition.x - boxStartingPosition.x, -1 * ((Screen.height - boxStartingPosition.y) - (Screen.height - boxEndingPosition.y)));
        }
    }

    

    public void MultiSelect()
    {

        foreach(GameObject u in units)
        {
            Vector2 unitPosition = Camera.main.WorldToScreenPoint(u.transform.position);
            if(selectBox.Contains(unitPosition,true))
            {
                selectedObjects.Add(u);
                u.SendMessage("Select");
            }
        }

        

        boxStartingPosition = Vector2.zero;
        boxEndingPosition = Vector2.zero;
    }

    void LeftClick()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        /*if(!GameObject.FindGameObjectWithTag("Ground").GetComponent<SpriteRenderer>().sprite.bounds.Contains(origin) )
        {
            Debug.Log("Out of boounds");
            return;
        }*/
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up);
        if (forestSelected)
        {
            forestSelected = false;
            GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleForestPanel");
        }

        if (hit.collider != null && hit.transform.tag == SELECTABLE_TAG /*&& selectedObject==null*/)
        {
            if(buildSelected)
            {
                buildSelected = false;
                GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleBuildPanel");
            }
            //selectedObject = hit.collider.gameObject;
            if (!selectedObjects.Contains(hit.collider.gameObject))
            {
                hit.collider.gameObject.SendMessage("Select");
                selectedObjects.Add(hit.collider.gameObject);
            }
            else
            {
                hit.collider.gameObject.SendMessage("UnSelect");
                selectedObjects.Remove(hit.collider.gameObject);
            }
            /*if (!hit.collider.gameObject.GetComponent<AITest>().IsSelected())
                selectedObjects.Add(hit.collider.gameObject);
            else
                selectedObjects.Remove(hit.collider.gameObject);*/
            //selectedObject.SendMessage("ToggleSelected");
        }
        // else if(hit.collider != null && hit.transform.tag == SELECTABLE_TAG)
        // {
        //     selectedObject.SendMessage("ToggleSelected");
        //     selectedObject = hit.collider.gameObject;
        //     selectedObject.SendMessage("ToggleSelected");
        // }

        else if(hit.transform.tag == GROUND_TAG &&  selectedObjects.Count > 0)
        {
            //selectedObject.SendMessage("ToggleSelected");
            //selectedObject = null;
            foreach(GameObject u in selectedObjects)
            {
                u.SendMessage("UnSelect");
            }
            selectedObjects.Clear();
        }

        else if(hit.transform.tag == DROPPOINT_TAG)
        {
            foreach (GameObject u in selectedObjects)
            {
                u.SendMessage("UnSelect");
            }
            selectedObjects.Clear();
            buildSelected = true;
            GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleBuildPanel");
        }

        else if(hit.transform.tag == GROUND_TAG && buildSelected)
        {
            buildSelected = false;
            GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleBuildPanel");
        }
    }

    void RightClick()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(origin, -Vector2.up);
        //Debug.Log(origin);
        //Debug.Log(hit.collider.gameObject);
        if(hit.collider != null && !hit.collider.CompareTag(GROUND_TAG) && (hit.transform.tag == RESSOURCE_TAG || hit.transform.tag == BUILDABLE_TAG || hit.transform.tag == ENEMYBUILDING_TAG || hit.transform.tag == DROPPOINT_TAG || hit.transform.tag == ENEMYNEXUS_TAG || hit.transform.tag == ENEMY_TAG || hit.transform.tag == PREY_TAG))
        {
            //selectedObject.SendMessage("SetTarget", hit.transform);
            if(hit.transform.tag == RESSOURCE_TAG && selectedObjects.Count > 0)
            {
                GameObject.FindGameObjectWithTag("UI").SendMessage("ToggleForestPanel");
                forestSelected = true;
            }
            foreach (GameObject u in selectedObjects)
            {
               
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
        if(selectedObjects.Count > 0)
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
