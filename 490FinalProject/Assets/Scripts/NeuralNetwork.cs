
using System.Collections.Generic;
using System;
using System.IO;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] layers; 
    private float[][] neurons; //neuron matix
    private float[][][] weights; //weight matrix
    private float fitness; //fitness of the network


//Neural network constructor, inits neurons and weights matrices
    public NeuralNetwork(int[] layers)
    {
        //copy layers over
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitNeurons();
        InitWeights();
    }


    //method to copy over neural network
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }

    //copies weights from 1 NN to another
    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }
    //returns a string to manager composing all the weights of the NN
    public string GetWeightsString()
    {
        string weightsString = "";
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weightsString += weights[i][j][k].ToString() + "\n";
                }
            }
        }
        return weightsString;
    }

    public void WriteWeights(string path)
    {
        StreamWriter sw = new StreamWriter(path);
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    //weightsString += weights[i][j][k].ToString() + "\n";
                    sw.WriteLine(weights[i][j][k].ToString());
                }
            }
        }
    }

    //The weights of the NN are set by reading file. file path given by manager 
    public void CopyWeightsFromFile(string path)
    {
        StreamReader sr = new StreamReader(path);
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    string nextLine = sr.ReadLine();
                    weights[i][j][k] = float.Parse(nextLine);
                }
            }
        }
    }

    //set up neurons jagged array
    private void InitNeurons()
    {
        //Neuron Initilization
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++) //run through all layers
        {
            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
        }
        neurons = neuronsList.ToArray(); //convert list to array
    }

    //init weights jagged array with random numbers between -.5 and .5
    private void InitWeights()
    {

        List<float[][]> weightsList = new List<float[][]>(); //weights list which will later will converted into a weights 3D array

        //iterate over all neurons that have a weight connection
        // 1 less layer length then neurons array
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>(); //layer weight list for this current layer (will be converted to 2D array)

            int neuronsInPreviousLayer = layers[i - 1];

            //iterate over all neurons in this current layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                //iterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    //give random weights to neuron weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
            }

            weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
        }

        weights = weightsList.ToArray(); //convert to 3D array
    }

//feed forward data from inputs thru NN and return output layer
    public float[] FeedForward(float[] inputs)
    {
        //Add inputs to the neuron matrix
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //iterate over all neurons and compute feedforward values 
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f; //bias value

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum off all weights connections of this neuron weight their values in previous layer
                }

                neurons[i][j] = (float)Math.Tanh(value); //Hyperbolic tangent activation
            }
        }

        return neurons[neurons.Length - 1]; //return output layer
    }

//Mutates NN to lead to new behaviors over generations
    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //mutate weight value, will happen 8% off time
                    float randomNumber = UnityEngine.Random.Range(0f, 100f);

                    if (randomNumber <= 2f)
                    { //flip sign of weight
                        weight *= -1f;
                    }
                    else if (randomNumber <= 4f)
                    { 
                      //pick random weight between -.5 and .5
                        weight = UnityEngine.Random.Range(-0.5f, .5f);
                    }
                    else if (randomNumber <= 6f)
                    { 
                      //randomly increase by 0% to 100%, clamp between -.5 and .5
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                        if (weight > .5f)
                            weight = .5f;
                    }
                    else if (randomNumber <= 8f)
                    { 
                      //randomly decrease by 0% to 100% , clamp beween -.5 and .5
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                        if (weight < -.5f)
                            weight = -.5f;
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float f)
    {
        fitness += f;
    }

    public void SetFitness(float f)
    {
        fitness = f;
    }

    public float GetFitness()
    {
        return fitness;
    }

    // allows ability to compare to another NN (other) by their fitness score
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }
}