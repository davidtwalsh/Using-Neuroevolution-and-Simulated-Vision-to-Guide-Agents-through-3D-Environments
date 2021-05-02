using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentDebug : MonoBehaviour
{
    public float movementSpeed = 0f;
    public float rotSpeed = 0f; //body rotation speed

    //for raycasting
    public int numRays = 5; // must be odd
    public float degBetweenRays = 10f;
    public int numRayGroups = 3;
    public float degBetweenRayGroups = 10f;
    //for head movement
    public Transform head;
    public float headRotSpeed = 100f;
    float xRotation;
    public LayerMask rayCastLayers;

    float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            //Debug.Log(head.eulerAngles.x);
            timer = 0;
        }

        //for moving and rotating body
        if (Input.GetKey("d"))
        {
            transform.Rotate(new Vector3(0f, rotSpeed, 0f) * Time.deltaTime);
        }
        else if (Input.GetKey("a"))
        {
            transform.Rotate(new Vector3(0f, -rotSpeed, 0f) * Time.deltaTime);
        }
        if (Input.GetKey("q"))
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }
        else if (Input.GetKey("e"))
        {
            transform.position -= transform.forward * movementSpeed * Time.deltaTime;
        }


        //for head rotation
        xRotation = head.eulerAngles.x;
        if (xRotation <= 50f || xRotation >= 310f)
        {
            if (Input.GetKey("w"))
            {
                xRotation -= headRotSpeed * Time.deltaTime;
            }
            if (Input.GetKey("s"))
            {
                xRotation += headRotSpeed * Time.deltaTime;
            }
        }
        else if (xRotation > 50f && xRotation < 180f)
            xRotation = 49.9f;
        else if (xRotation > 180f && xRotation < 310f)
            xRotation = 310.1f;

        head.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);

        //for raycasting
        float leftRayOffset = -degBetweenRays;
        float rightRayOffset = degBetweenRays;
        float xOffset = 0;
        float upOffset = degBetweenRayGroups;
        float downOffset = -degBetweenRayGroups;
        for (int j = 0; j < numRayGroups; j++)
        {
            if (j == 0)
                xOffset = 0;
            else if (j % 2 == 1) //odd
            {
                xOffset = upOffset;
                upOffset += degBetweenRayGroups;
            }
            else if (j % 2 == 0) //is even
            {
                xOffset = downOffset;
                downOffset -= degBetweenRayGroups;
            }
            for (int i = 0; i < numRays; i++)
            {
                Ray ray = new Ray();
                if (i == 0) // ray right in front of agent
                {
                    ray = new Ray(head.position, Quaternion.Euler(xOffset, 0, 0) * head.forward);
                    
                }
                else if (i % 2 == 1) // is odd
                {
                    ray = new Ray(head.position, Quaternion.Euler(xOffset, leftRayOffset, 0) * head.forward);
                    
                    leftRayOffset -= degBetweenRays;
                }
                else if (i % 2 == 0) //is even
                {
                    ray = new Ray(head.position, Quaternion.Euler(xOffset, rightRayOffset, 0) * head.forward);
                    
                    rightRayOffset += degBetweenRays;
                }
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 200, rayCastLayers))
                {
                    Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
                }
                else
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 200, Color.green);
                }
                if (hitInfo.collider != null)
                {
                    if (hitInfo.collider.gameObject.tag == "Flag")
                    {
                        //Debug.Log("Flag hit");
                        //Debug.Log(hitInfo.distance);
                    }
                }
            }
            leftRayOffset = -degBetweenRays;
            rightRayOffset = degBetweenRays;
        }


    }//end of Update()
}
