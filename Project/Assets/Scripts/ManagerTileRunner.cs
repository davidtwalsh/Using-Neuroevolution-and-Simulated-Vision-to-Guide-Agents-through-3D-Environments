using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ManagerTileRunner : MonoBehaviour
{

    public GameObject agentTileRunnerPrefab;

    public bool isTraining = false;
    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 61, 50, 50, 50, 3 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private List<AgentTileRunner> agentList = null;

    public float timeBetweenGens = 10f;
    float generationTimer = 0f;
    public Text generationNumberText;
    public Button nextGenButton;

    bool isLoadingNN = false;
    public CourseCreator courseCreator;
    public GameObject confirmSaveObj;

    void Update()
    {
        generationNumberText.text = "Generation Number: " + generationNumber;
        generationTimer -= Time.deltaTime;
        if (generationTimer <= 0f)
            isTraining = false;

        if (isTraining == false)
        {
            //courseCreator.ClearObstacles();
            //courseCreator.CreateTileObstacle();

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

        //1st check if files DNE. if so, create files
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/TileRunner/nn" + i.ToString();
            if (!File.Exists(filePath)) //nn files dont exist so make them
            {
                Debug.Log("File doesnt exist");
                File.Create(filePath);
            }
            i++;
        }
        //Next Write Weights of each NN to file
        i = 0;
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/TileRunner/nn" + i.ToString();
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
        //copy weights from file to NN
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/TileRunner/nn" + i.ToString();
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

        agentList = new List<AgentTileRunner>();

        for (int i = 0; i < populationSize; i++)
        {
            AgentTileRunner r = Instantiate(agentTileRunnerPrefab, new Vector3(0f, 1f, 0f), agentTileRunnerPrefab.transform.rotation).GetComponent<AgentTileRunner>();
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
