using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private const string cameraTag = "MainCamera";
    public float cameraMoveSpeed = 0.3f;
    private float panDetect = 15.0f;
    public float cameraRotateSpeed = 0.3f;
    public float rotateAmount;

    public float cameraZoomSpeed = 6f;

    private float minZoom = 5;
    private float maxZoom = 10;
    Vector2 moveInput;
    private Quaternion cameraRot;
    
    
    void Start()
    {
       cameraRot = Camera.main.transform.rotation;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
		moveInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }

    void ZoomCamera()
    {
        float zoom = Camera.main.orthographicSize;

        zoom -= Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        Camera.main.orthographicSize = zoom;
    }

    void MoveCamera()
    {
        float posX = Camera.main.transform.position.x;
        float posY = Camera.main.transform.position.y;
       
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;

        //Keyboard movement
        posX += moveInput.x * cameraMoveSpeed;
        posY += moveInput.y * cameraMoveSpeed;

        //Mouse movement
        if(mouseX > 0 && mouseX < panDetect)
        {
            Debug.Log("Going left");
            posX -= cameraMoveSpeed;
        }
        else if(mouseX < Screen.width && mouseX > (Screen.width - panDetect))
        {
            Debug.Log("Going right");
            posX += cameraMoveSpeed;
        }
        else if(mouseY > 0 && mouseY < panDetect)
        {
            Debug.Log("Going down");
            posY -= cameraMoveSpeed;
        }
        else if(mouseY < Screen.height && mouseY > (Screen.height - panDetect))
        {
            Debug.Log("Going up");
            posY += cameraMoveSpeed;
        }

        Vector3 newPos = new Vector3(posX, posY, -10f);
        Camera.main.transform.position = newPos;
    }

    void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 dest = origin;

        if(Input.GetKey(KeyCode.E))
        {
            dest.z += rotateAmount;
        }

        if(Input.GetKey(KeyCode.A))
        {
            dest.z -= rotateAmount;
        }

        if(dest != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, dest, cameraRotateSpeed);
        }

        
    }
}
