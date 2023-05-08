using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Neuron
{
    // number is used to find where it is in the network
    int number;
    public int layer;
    private float inputSum = 0;

    public float outputValue;

    // Need to know which nodes it is connected to so it can
    // send the output value to it
    public List<Connection> connections = new List<Connection>();

    public Neuron(int _number)
    {
        number = _number;
    }

    // This will send the output value to the connections
    public void SendOutput()
    {
        // If this neuron is not in the inputs layer, send output value
        // using the sigmoid value
        if (layer != 0)
            outputValue = Sigmoid(inputSum);

        for(int i = 0; i < connections.Count; i++)
        {
            connections[i].toNeuron.AddToInputSum(outputValue * connections[i].weight);
        }
    }

    private float Sigmoid(float inputValue)
    {
        // calculate the output value using a sigmoid function
        return 1 / (1 + Mathf.Exp(-inputValue));
    }

    public void AddToInputSum(float inputValue)
    {
        inputSum += inputValue;
    }

    public void SetInputSum(float inputValue)
    {
        inputSum = inputValue;
    }
}
