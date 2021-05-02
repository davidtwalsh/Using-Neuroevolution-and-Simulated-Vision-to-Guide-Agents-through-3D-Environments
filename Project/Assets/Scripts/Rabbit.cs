using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    private bool initilized = false;

    private NeuralNetwork net;
    private Rigidbody rBody;

    public float hungerMax = 10f;
    private float hunger;
    public float thirstMax = 10f;
    private float thirst;
    public float speed = 10f;

    private bool alive = true;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        hunger = hungerMax;
        thirst = thirstMax;
    }

    void FixedUpdate()
    {
        if (initilized == true && alive == true)
        {
            hunger -= Time.deltaTime;
            thirst -= Time.deltaTime;
            if (hunger <= 0 || thirst <= 0)
            {
                alive = false;
            }

            float[] inputs = new float[2];


            inputs[0] = hunger;
            inputs[1] = thirst;


            float[] output = net.FeedForward(inputs);
            //transform.position += new Vector3(output[0] * speed, transform.position.y, transform.position.z);
            rBody.velocity = new Vector3(output[0] * speed, 0f, 0f);

            net.AddFitness(Time.deltaTime);
        }
        else if (alive == false)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
            thirst = thirstMax;
        else if (other.gameObject.tag == "Carrot")
            hunger = hungerMax;
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initilized = true;
    }


}
