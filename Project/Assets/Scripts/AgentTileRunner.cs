using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentTileRunner : MonoBehaviour
{
    private bool initilized = false;

    private NeuralNetwork net;
    private Rigidbody rBody;

    private bool alive = true;

    public float movementSpeed = 10f;
    public float rotSpeed = 100f; //body rotation speed

    //for raycasting
    public int numRays = 30; // must be odd
    public float degBetweenRays = 5f;
    //for head movement
    public Transform head;
    public float headRotSpeed = 100f;
    float xRotation;
    public LayerMask rayCastLayers;

    public CourseCreator courseCreator;
    GameObject flag;
    bool hitFlag = false;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        courseCreator = FindObjectOfType<CourseCreator>();
        flag = courseCreator.flag;
    }

    void FixedUpdate()
    {
        if (initilized == true && alive == true)
        {
            if (hitFlag == true)
            {
                return;
            }
            //2 float inputs for each ray + 1 for head.eularangles.x
            float[] inputs = new float[numRays * 2 + 1];

            inputs[0] = head.eulerAngles.x;

            //for raycasting
            float leftRayOffset = -degBetweenRays;
            float rightRayOffset = degBetweenRays;
            int inputIndex = 1;
            for (int i = 0; i < numRays; i++)
            {
                Ray ray = new Ray();
                if (i == 0) // ray right in front of agent
                {
                    ray = new Ray(head.position, Quaternion.Euler(0, 0, 0) * head.forward);

                }
                else if (i % 2 == 1) // is odd
                {
                    ray = new Ray(head.position, Quaternion.Euler(0, leftRayOffset, 0) * head.forward);

                    leftRayOffset -= degBetweenRays;
                }
                else if (i % 2 == 0) //is even
                {
                    ray = new Ray(head.position, Quaternion.Euler(0, rightRayOffset, 0) * head.forward);

                    rightRayOffset += degBetweenRays;
                }
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 200, rayCastLayers))
                {
                    Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
                    inputs[inputIndex] = hitInfo.distance;
                    if (hitInfo.collider.gameObject.tag == "Ground")
                        inputs[inputIndex + 1] = 100f;
                    else if (hitInfo.collider.gameObject.tag == "Flag")
                        inputs[inputIndex + 1] = 200f;
                    else
                        inputs[inputIndex + 1] = 300f;
                    inputIndex += 2;
                   
                }
                else
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 200, Color.green);
                    inputs[inputIndex] = -1;
                    inputs[inputIndex + 1] = -1;
                    inputIndex += 2;
                }
    
            }
            //end raycasing


            float[] output = net.FeedForward(inputs);
            //for head rotation
            xRotation = head.eulerAngles.x;
            if (xRotation <= 50f || xRotation >= 310f)
            {
                xRotation += output[0] * headRotSpeed * Time.deltaTime;
            }
            else if (xRotation > 50f && xRotation < 180f)
                xRotation = 49.9f;
            else if (xRotation > 180f && xRotation < 310f)
                xRotation = 310.1f;
            head.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
            //for body rotation
            transform.Rotate(new Vector3(0f, rotSpeed * output[1], 0f) * Time.deltaTime);
            //for body movement
            transform.position += transform.forward * movementSpeed * output[2] * Time.deltaTime;

            net.SetFitness(1/Vector3.Distance(transform.position, flag.transform.position));
        }
        else if (alive == false)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GroundDeath")
        {
            alive = false;
        }
        else if (other.gameObject.tag == "Flag")
        {
            hitFlag = true;
        }
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initilized = true;
    }
}
