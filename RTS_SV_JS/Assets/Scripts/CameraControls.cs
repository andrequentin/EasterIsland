using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Vector2 panLimit;
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
        float posX = Input.mousePosition.x;
        float posY = Input.mousePosition.y;

        float mouseX = Input.GetAxis("Horizontal");
        float mouseY = Input.GetAxis("Vertical");

        //Keyboard movement
        posX += moveInput.x * cameraMoveSpeed;
        posY += moveInput.y * cameraMoveSpeed;

        //Mouse movement
        if (posX > 0 && posX < panDetect)
        {
            mouseX -= cameraMoveSpeed;
        }
        else if (posX < Screen.width && posX > (Screen.width - panDetect))
        {
            mouseX += cameraMoveSpeed;
        }
        else if (posY > 0 && posY < panDetect)
        {
            mouseY -= cameraMoveSpeed;
        }
        else if (posY < Screen.height && posY > (Screen.height - panDetect))
        {
            mouseY += cameraMoveSpeed;
        }

        Vector3 np = new Vector3(mouseX, mouseY, 0);

        Camera.main.transform.Translate(np);


    }

    void RotateCamera()
    {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 dest = origin;

        if (Input.GetKey(KeyCode.E))
        {
            dest.z += rotateAmount;
        }

        if (Input.GetKey(KeyCode.A))
        {
            dest.z -= rotateAmount;
        }

        if (dest != origin)
        {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, dest, cameraRotateSpeed);
        }


    }
}
