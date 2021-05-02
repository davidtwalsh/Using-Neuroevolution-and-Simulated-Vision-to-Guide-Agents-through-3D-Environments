using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentRedLight : MonoBehaviour
{

    private bool initilized = false;

    private NeuralNetwork net;
    private Rigidbody rBody;

    private bool alive = true;

    public float movementSpeed = 10f;

    //for camera and rendering
    public Texture2D renderResult = null;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (initilized == true && alive == true)
        {
            if (renderResult != null)
            {
                float[] inputs = new float[16 * 3];

                int index = 0;
                //for taking picture
                Color[] colors = renderResult.GetPixels();
                for (int i = 0; i < colors.Length; i++)
                {
                    inputs[index] = colors[i].r;
                    inputs[index + 1] = colors[i].g;
                    inputs[index + 2] = colors[i].b;
                    index += 3;
                }

                float[] output = net.FeedForward(inputs);

                //for body movement
                if (output[0] < -.25f)
                    output[0] = 0;
                transform.position += transform.forward * movementSpeed * output[0] * Time.deltaTime;

                renderResult = null;
            }
        }
        else if (alive == false)
        {
            gameObject.SetActive(false);
        }
        net.SetFitness(transform.position.z);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GroundDeath")
        {
            alive = false;
        }
    }

    public void Kill()
    {
        alive = false;
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initilized = true;
    }
}
