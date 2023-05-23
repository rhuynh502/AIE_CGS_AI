using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// The network should handle all the connections between nodes
// It also keeps track of the structure of the system
public class Network
{
    // These are determined in the agent class
    private int inputs;
    private int outputs;
    private int hiddenLayerAmountOfNodes;

    private List<Neuron> neurons = new List<Neuron>();
    private List<Connection> connections = new List<Connection>();
    int currNeuron = 0;
    int biasNeuronLocation;
    // A basic network starts with 2 layers. This can increase
    int layers = 3;

    private List<Neuron> orderedNetwork = new List<Neuron>();

    public Network(int _inputs, int _outputs, int _hiddenLayerAmountOfNodes)
    {
        // Generate a network with these amounts of inputs and outputs
        inputs = _inputs;
        outputs = _outputs;
        hiddenLayerAmountOfNodes = _hiddenLayerAmountOfNodes;

        // Generate the input nodes/neurons
        for(int i = 0; i < inputs; i++)
        {
            neurons.Add(new Neuron(currNeuron));
            currNeuron++;
            // Sets layer to 0 because input layers are always first
            neurons[i].layer = 0;
        }

        // Generate the output nodes/neurons
        // i + inputs is used to continue the list from where it ended
        for(int i = 0; i < outputs; i++)
        {
            neurons.Add(new Neuron(currNeuron));
            neurons[i + inputs].layer = 2;
            currNeuron++;
        }

        // Generate the bias node/neuron
        neurons.Add(new Neuron(currNeuron));
        // This keeps track of where the bias neuron is.
        // The bias neuron is needed in every feedforward operation.
        biasNeuronLocation = currNeuron;
        // Bias neuron will always be on first layer
        neurons[biasNeuronLocation].layer = 0;
        currNeuron++;

        for(int i = 0; i < hiddenLayerAmountOfNodes; i++)
        {
            neurons.Add(new Neuron(currNeuron));
            neurons[currNeuron].layer = 1;
            currNeuron++;
        }

        // Create connections between the inputs, hidden layer, and outputs
        for(int i = 0; i < inputs; i++)
        {
            for(int j = 0; j < hiddenLayerAmountOfNodes; j++)
            {
                connections.Add(new Connection(neurons[i], neurons[biasNeuronLocation + j + 1], Random.Range(-1.0f, 1.0f)));
            }
        }

        for(int i = 0; i < outputs; i++)
        {
            for(int j = 0; j < hiddenLayerAmountOfNodes; j++)
            {
                connections.Add(new Connection(neurons[biasNeuronLocation + j + 1], neurons[inputs + i], Random.Range(-1.0f, 1.0f)));
            }
        }

        // Connect Bias Node
        for(int i = 0; i < hiddenLayerAmountOfNodes; i++)
        {
            connections.Add(new Connection(neurons[biasNeuronLocation], neurons[biasNeuronLocation + i + 1], Random.Range(-1.0f, 1.0f)));
        }

        for(int i = 0; i < outputs; i++)
        {
            connections.Add(new Connection(neurons[biasNeuronLocation], neurons[inputs + i], 1));
        }
    }

    // This function will give all required data to the neurons
    public List<float> FeedForward(List<float> inputsFromPlayer)
    {
        // Set the outputs for the input nodes. We know where they are in the
        // network because they are the first nodes we add in the constructor.
        for (int i = 0; i < inputs; i++)
        {
            neurons[i].outputValue = inputsFromPlayer[i];
        }
        
        neurons[biasNeuronLocation].outputValue = -1;

        for (int i = 0; i < orderedNetwork.Count; i++)
        {
            orderedNetwork[i].SendOutput();
        }

        List<float> outputsForPlayer = new List<float>();
        // Get outputs from network. we know where the output nodes are because
        // they are initialized after the inputs in the constructor
        for(int i = 0; i < outputs; i++)
        {
            outputsForPlayer.Add(neurons[inputs + i].outputValue);
        }

        // reset network for next feedforward
        for(int i = 0; i < neurons.Count; i++)
        {
            neurons[i].SetInputSum(0);
        }

        return outputsForPlayer;
    }

    // This function connects the neurons to each other
    private void ConnectNetwork()
    {
        // reset input sums for next feedforward
        for(int i = 0; i < neurons.Count; i++)
        {
            neurons[i].SetInputSum(0);
        }

        // Add the connection to the node. The node isn't aware of the connection until here
        for(int i = 0; i < connections.Count; i++)
        {
            connections[i].fromNeuron.connections.Add(connections[i]);
        }
    }

    // Currently the order of nodes in the network start from 0 -> 1 -> random amount of layers
    // for the network to go through the correct nodes from start to finish. This will sort the network
    // for the feedforward process where it starts from 0 and couns up per layer.
    public void OrderNetwork()
    {
        ConnectNetwork();
        orderedNetwork = new List<Neuron>();

        for(int i = 0; i < layers; i++)
        {
            for(int j = 0; j < neurons.Count; j++)
            {
                if (neurons[j].layer == i)
                {
                    orderedNetwork.Add(neurons[j]);

                }
            }
        }

    }
    public void Mutate()
    {
        for (int i = 0; i < connections.Count; i++)
            connections[i].Mutate();
    }
    public List<Neuron> GetNeurons()
    {
        return neurons;
    }
    public List<Connection> GetConnections()
    {
        return connections;
    }

    public int GetInputsAmount() { return inputs; }
    public int GetOutputsAmount() { return outputs; }
    public int GetHiddenLayerAmount() { return hiddenLayerAmountOfNodes; }

    public Network CloneNetwork()
    {
        Network clone = new Network(inputs, outputs, hiddenLayerAmountOfNodes);

        for(int i = 0; i < connections.Count; i++)
        {
            clone.connections[i].weight = connections[i].weight;
        }

        clone.ResetNeurons();
        clone.OrderNetwork();

        return clone;
    }

    public void ResetNeurons()
    {
        foreach(Neuron neuron in neurons)
        {
            neuron.SetInputSum(0);
        }
    }
}
