using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLightGreenLight : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material greenMat;
    public Material yellowMat;
    public Material redMat;
    public ManagerRedLight managerRedLight;
    
    enum state
    {
        green,
        yellow,
        red
    }
    state currentState = state.green;
    float timer = 0f;
    float timeTillNextStateChange = 0f;

    void Start()
    {
        timeTillNextStateChange = Random.Range(2f, 4f);
    }
    public void Reset()
    {
        currentState = state.green;
        timeTillNextStateChange = Random.Range(2f, 4f);
        timer = 0f;
        managerRedLight.isRedLight = false;
        meshRenderer.material = greenMat;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (currentState == state.green)
        {
            if (timer >= timeTillNextStateChange)
            {
                //switch to yellow state
                //switch to red state
                meshRenderer.material = redMat;
                currentState = state.red;
                timer = 0f;
                timeTillNextStateChange = Random.Range(2f, 4f);
                managerRedLight.redLightBegan = true;
                managerRedLight.isRedLight = true;
            }
        }
        else if (currentState == state.yellow)
        {
            if (timer >= timeTillNextStateChange)
            {
                //switch to red state
                meshRenderer.material = redMat;
                currentState = state.red;
                timer = 0f;
                timeTillNextStateChange = Random.Range(2f, 4f);
                managerRedLight.redLightBegan = true;
                managerRedLight.isRedLight = true;
            }
        }
        else if (currentState == state.red)
        {
            if (timer >= timeTillNextStateChange)
            {
                //switch to green state
                meshRenderer.material = greenMat;
                currentState = state.green;
                timer = 0f;
                timeTillNextStateChange = Random.Range(2f, 4f);
                managerRedLight.isRedLight = false;
            }
        }

    }
}
