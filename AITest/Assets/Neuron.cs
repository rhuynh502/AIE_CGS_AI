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
    public float gradientDescent;

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
            outputValue = Tanh(inputSum);

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

    private float Tanh(float inputValue)
    {
        return (Mathf.Exp(inputValue) - Mathf.Exp(-inputValue)) / (Mathf.Exp(inputValue) + Mathf.Exp(-inputValue));
    }

    private float SigmoidDerivative(float inputValue)
    {
        return inputValue * (1 - inputValue);
    }
    
    private float TanhDerivative(float inputValue)
    {
        return 1 - Mathf.Pow((float)System.Math.Tanh(inputValue), 2);
    }

    public float GradientDescent(float? target = null)
    {
        if (!target.HasValue)
        {
            float sum = 0;

            for (int i = 0; i < connections.Count; i++)
                sum += connections[i].toNeuron.gradientDescent * connections[i].weight;

            return gradientDescent = sum * TanhDerivative(outputValue);
        }

        return gradientDescent = CalculateError(target.Value) * TanhDerivative(outputValue);
    }

    public void ChangeWeights()
    {
        for(int i = 0; i < connections.Count; i++)
        {
            float tempWeight = connections[i].weight;
            connections[i].weight += 0.4f * gradientDescent * connections[i].fromNeuron.outputValue * 0.9f;
            //Debug.Log($"{tempWeight} || {connections[i].weight}");
        }
    }

    private float CalculateError(float target)
    {
        return (outputValue - target) * (outputValue - target);
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
