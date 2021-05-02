using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMazeRunner : MonoBehaviour
{
    private bool initilized = false;

    private NeuralNetwork net;
    private Rigidbody rBody;

    private bool alive = true;

    public float movementSpeed = 10f;
    public float rotSpeed = 100f; //body rotation speed

    //for head movement
    public Transform head;
    public float headRotSpeed = 100f;
    float xRotation;

    public GameObject flag;
    bool hitFlag = false;

    //for camera and rendering
    public Texture2D renderResult;
    public HeadCameraRender headCameraRender;
    bool savePNGTest = false;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        renderResult = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            savePNGTest = true;
        }
    }
    void FixedUpdate()
    {
        if (initilized == true && alive == true)
        {
            if (hitFlag == true)
            {
                return;
            }

            TakeScreenshot(256, 128);
            if (renderResult == null)
            {
                return;
            }
            if (savePNGTest == true)
            {
                savePNGTest = false;
                byte[] _bytes = renderResult.EncodeToPNG();
                int num = Random.Range(0, 10000);
                System.IO.File.WriteAllBytes(Application.dataPath + "/CameraScreenshots/screenshot" + num + ".png", _bytes);
            }
            float[] inputs = new float[384 + 1];

            inputs[0] = head.eulerAngles.x;
            //for taking picture
            int index = 1;
            Color[] colors = renderResult.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                inputs[index] = colors[i].r;
                inputs[index + 1] = colors[i].g;
                inputs[index + 2] = colors[i].b;
            }


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

            net.SetFitness(1 / Vector3.Distance(transform.position, flag.transform.position));
        }
        else if (alive == false)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Flag")
        {
            hitFlag = true;
        }
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        initilized = true;
    }

    private void TakeScreenshot(int width, int height)
    {
        headCameraRender.myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        headCameraRender.takeScreenshotOnNextFrame = true;
    }

}
