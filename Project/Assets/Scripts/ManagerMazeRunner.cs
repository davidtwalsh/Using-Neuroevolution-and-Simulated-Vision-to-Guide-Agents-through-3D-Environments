using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ManagerMazeRunner : MonoBehaviour
{

    public GameObject agentTileRunnerPrefab;

    public bool isTraining = false;
    public int populationSize = 4;
    private int generationNumber = 0;
    private int[] layers = new int[] { 385, 50, 50, 50, 3 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private List<AgentMazeRunner> agentList = null;

    public float timeBetweenGens = 10f;
    float generationTimer = 0f;
    public Text generationNumberText;
    public Button nextGenButton;

    bool isLoadingNN = false;
    public GameObject confirmSaveObj;
    public List<GameObject> flags;
    public List<Transform> spawns;

    public int cameraDisplayIndex = 1;

    void Update()
    {
        generationNumberText.text = "Generation Number: " + generationNumber;
        generationTimer -= Time.deltaTime;
        if (generationTimer <= 0f)
            isTraining = false;

        if (isTraining == false)
        {

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

        //check if gen file DNE, if so create genfile
        filePath = Application.dataPath + "/NeuralNets/MazeRunner/genNumber";
        if (!File.Exists(filePath)) //nn files dont exist so make them
        {
            Debug.Log("File doesnt exist");
            File.Create(filePath);
        }

        //1st check if files DNE. if so, create files
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/MazeRunner/nn" + i.ToString();
            if (!File.Exists(filePath)) //nn files dont exist so make them
            {
                Debug.Log("File doesnt exist");
                File.Create(filePath);
            }
            i++;
        }

        //write gen number to gen file
        filePath = Application.dataPath + "/NeuralNets/MazeRunner/genNumber";
        string genNumberStr = generationNumber.ToString();
        File.WriteAllText(filePath, genNumberStr);

        //Next Write Weights of each NN to file
        i = 0;
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/MazeRunner/nn" + i.ToString();
            nn.WriteWeights(filePath);
            i++;
        }
        confirmSaveObj.SetActive(false);
    }

    public void LoadNeuralNets()
    {
        Debug.Log("Loading");
        string filePath = "";
        int i = 0;

        //copy genNumber from genNumber file
        filePath = Application.dataPath + "/NeuralNets/MazeRunner/genNumber";
        StreamReader sr = new StreamReader(filePath);
        generationNumber = int.Parse(sr.ReadLine());

        //copy weights from file to NN
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/MazeRunner/nn" + i.ToString();
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

        agentList = new List<AgentMazeRunner>();

        for (int i = 0; i < populationSize; i++)
        {
            AgentMazeRunner r = Instantiate(agentTileRunnerPrefab, spawns[i].position, agentTileRunnerPrefab.transform.rotation).GetComponent<AgentMazeRunner>();
            r.flag = flags[i];
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
