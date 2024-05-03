using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class RotateBigCube : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    Vector3 previousMousePosition;
    Vector3 mouseDelta;
    Vector3 startingPosition = new Vector3(-18, -56, 25);
    //public GameObject target;
    bool doOnce = true;

    // float speed = 200f;
    private float speed = 150f;
    private Quaternion targetQuaternion;
    private bool autoRotateCube = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Drag();
        if(Input.GetKeyDown(KeyCode.Return) && CubeState.started && !CubeState.keyMove && !CubeState.autoRotateDrag && !CubeState.drag) 
        {
            transform.rotation = Quaternion.Euler(startingPosition);
        }
     
    }

    private void LateUpdate()
    {
        if (doOnce)
        {
            transform.rotation = Quaternion.Euler(startingPosition);
            doOnce = false;
        }
        if(autoRotateCube)
        {
            AutoRotateCube();
        }
      
    }

    private void AutoRotateCube()
    {
        var step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetQuaternion, step);
        if (Quaternion.Angle(transform.localRotation, targetQuaternion) == 0)
        {
            transform.localRotation = targetQuaternion;
            // unparent the little cubes        
            autoRotateCube = false;
            CubeState.autoCubeRotate = false;
            CubeState.keyMove = false;
        }
    }

    public void RotateCube(string face)
    {
        if(face == "B")
        {
            // targetQuaternion = Quaternion.Euler(13, 115, -21);
            targetQuaternion = Quaternion.Euler(0, 90, 0);
        }
        else if(face == "F")
        {
            //targetQuaternion = Quaternion.Euler(-18, -56, 25);
            targetQuaternion = Quaternion.Euler(-18,-56,25);
        }
        autoRotateCube = true;
    }
    void Drag()
    {
        if (Input.GetMouseButton(1))
        {
            // while the mouse is held doen the cube can be moved around its central axis to provide visual feedback
            mouseDelta = Input.mousePosition - previousMousePosition;
            mouseDelta *= .5f; // reduction of rotation speed
            float currentY = transform.rotation.y%90;
            float currentZ = transform.rotation.z % 90;
            float currentX = transform.rotation.x % 90;

            if (currentX <= 20)
            {
                transform.rotation = Quaternion.Euler(0, -mouseDelta.x, 0) * transform.rotation;
            }
            else if (currentX >= 70)
            {
                transform.rotation = Quaternion.Euler(0,0,-mouseDelta.x) * transform.rotation;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, -mouseDelta.x, -mouseDelta.x) * transform.rotation;
            }
            
            if(currentY <= 20)
            {
                transform.rotation = Quaternion.Euler(mouseDelta.y,0, 0) * transform.rotation;
            }
            else if(currentY >= 70)
            {
                transform.rotation = Quaternion.Euler(0, 0, mouseDelta.y) * transform.rotation;
            }
            else
            {
                transform.rotation = Quaternion.Euler(mouseDelta.y, 0, mouseDelta.y) * transform.rotation;
            }
        }
     
        previousMousePosition = Input.mousePosition;
    }


    

  

}
