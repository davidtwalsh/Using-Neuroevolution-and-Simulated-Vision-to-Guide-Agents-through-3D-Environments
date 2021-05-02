using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ManagerRedLight : MonoBehaviour
{

    public GameObject agentRedLightPrefab;

    public bool isTraining = false;
    public int populationSize = 10;
    private int generationNumber = 0;
    private int[] layers = new int[] { 16*3, 50, 50, 50, 1 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private List<AgentRedLight> agentList = null;

    public float timeBetweenGens = 20f;
    float generationTimer = 0f;
    public Text generationNumberText;
    public Button nextGenButton;

    bool isLoadingNN = false;
    public GameObject confirmSaveObj;
    public List<Transform> spawns;

    public bool redLightBegan = false;
    public bool isRedLight = false;
    List<float> zPositions;

    public RedLightGreenLight redLightGreenLight;

    void Update()
    {
        generationNumberText.text = "Generation Number: " + generationNumber;
        generationTimer -= Time.deltaTime;
        if (generationTimer <= 0f)
            isTraining = false;

        if (isTraining == false)
        {
            redLightGreenLight.Reset();

            if (generationNumber == 0)
            {
                InitAgentNeuralNetworks();
            }
            else if (isLoadingNN == true)
            {
                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
                isLoadingNN = false;
            }
            else
            {
                nets.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }


            generationNumber++;

            isTraining = true;
            generationTimer = timeBetweenGens;
            CreateAgentBodies();
        }

        //only happens once when switched to red light
        if (redLightBegan == true)
        {
            zPositions = new List<float>();
            foreach (AgentRedLight agent in agentList)
            {
                Transform t = agent.GetComponent<Transform>();
                zPositions.Add(t.position.z);
            }
            redLightBegan = false;
        }
        //kill agents if they move to far from pos when red light began
        if (isRedLight == true)
        {
            for (int i = 0; i < agentList.Count; i++)
            {
                float agentZ = agentList[i].GetComponent<Transform>().position.z;
                float startZ = zPositions[i];
                if (Mathf.Abs(agentZ - startZ) > 0.5f)
                {
                    agentList[i].Kill();
                }
            }
        }
    }

    public void NextGenButtonPressed()
    {
        isTraining = false;
    }

    public void ShowConfirmSaveObj()
    {
        confirmSaveObj.SetActive(true);
    }
    public void HideSaveObj()
    {
        confirmSaveObj.SetActive(false);
    }

    public void SaveNeuralNets()
    {
        Debug.Log("Saving");
        string filePath = "";
        int i = 0;

        /*
        //check if gen file DNE, if so create genfile
        filePath = Application.dataPath + "/NeuralNets/RedLight/genNumber";
        if (!File.Exists(filePath)) //nn files dont exist so make them
        {
            Debug.Log("File doesnt exist");
            File.Create(filePath);
        }
        */

        //1st check if files DNE. if so, create files
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/RedLight/nn" + i.ToString();
            if (!File.Exists(filePath)) //nn files dont exist so make them
            {
                Debug.Log("File doesnt exist");
                File.Create(filePath);
            }
            i++;
        }

        /*
        //write gen number to gen file
        filePath = Application.dataPath + "/NeuralNets/RedLight/genNumber";
        string genNumberStr = generationNumber.ToString();
        File.WriteAllText(filePath, genNumberStr);
        */
        //Next Write Weights of each NN to file
        i = 0;
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/RedLight/nn" + i.ToString();
            //nn.WriteWeights(filePath);
            string weightsString = nn.GetWeightsString();
            File.WriteAllText(filePath, weightsString);
            i++;
        }
        confirmSaveObj.SetActive(false);
    }

    public void LoadNeuralNets()
    {
        Debug.Log("Loading");
        string filePath = "";
        int i = 0;

        /*
        //copy genNumber from genNumber file
        filePath = Application.dataPath + "/NeuralNets/RedLight/genNumber";
        StreamReader sr = new StreamReader(filePath);
        generationNumber = int.Parse(sr.ReadLine());
        */
        //copy weights from file to NN
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/RedLight/nn" + i.ToString();
            nn.CopyWeightsFromFile(filePath);
            i++;
        }
        isLoadingNN = true;
        isTraining = false;
    }

    private void CreateAgentBodies()
    {
        if (agentList != null)
        {
            for (int i = 0; i < agentList.Count; i++)
            {
                GameObject.Destroy(agentList[i].gameObject);
            }

        }

        agentList = new List<AgentRedLight>();

        for (int i = 0; i < populationSize; i++)
        {
            AgentRedLight r = Instantiate(agentRedLightPrefab, spawns[i].position, agentRedLightPrefab.transform.rotation).GetComponent<AgentRedLight>();
            r.Init(nets[i]);
            agentList.Add(r);


        }

    }

    void InitAgentNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
