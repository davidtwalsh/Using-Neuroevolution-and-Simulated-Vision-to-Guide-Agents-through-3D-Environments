using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ManagerTwoD : MonoBehaviour
{

    public GameObject rabbitPrefab;

    private bool isTraining = false;
    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 2, 10, 10, 1 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private List<Rabbit> rabbitList = null;

    public float timeBetweenGens = 10f;
    float generationTimer = 0f;
    public Text generationNumberText;
    public Button nextGenButton;

    bool isLoadingNN = false;

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
                InitRabbitNeuralNetworks();
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
            CreateRabbitBodies();
        }

    }

    public void NextGenButtonPressed()
    {
        isTraining = false;
    }

    public void SaveNeuralNets()
    {
        Debug.Log("Saving");
        string filePath = "";
        int i = 0;

        //1st check if files DNE. if so, create files
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/Rabbits/nn" + i.ToString();
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
            filePath = Application.dataPath + "/NeuralNets/Rabbits/nn" + i.ToString();
            string weightsString = nn.GetWeightsString();
            File.WriteAllText(filePath, weightsString);
            i++;
        }
    }

    public void LoadNeuralNets()
    {
        Debug.Log("Loading");
        string filePath = "";
        int i = 0;
        //copy weights from file to NN
        foreach (NeuralNetwork nn in nets)
        {
            filePath = Application.dataPath + "/NeuralNets/Rabbits/nn" + i.ToString();
            nn.CopyWeightsFromFile(filePath);
            i++;
        }
        isLoadingNN = true;
        isTraining = false;
    }

    private void CreateRabbitBodies()
    {
        if (rabbitList != null)
        {
            for (int i = 0; i < rabbitList.Count; i++)
            {
                GameObject.Destroy(rabbitList[i].gameObject);
            }

        }

        rabbitList = new List<Rabbit>();

        for (int i = 0; i < populationSize; i++)
        {
            Rabbit r = Instantiate(rabbitPrefab, new Vector3(0f,1f,0f), rabbitPrefab.transform.rotation).GetComponent<Rabbit>();
            r.Init(nets[i]);
            rabbitList.Add(r);
        }

    }

    void InitRabbitNeuralNetworks()
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
