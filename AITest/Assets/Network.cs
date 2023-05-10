using System.Collections;
using System.Collections.Generic;
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
                connections.Add(new Connection(neurons[i], neurons[biasNeuronLocation + j + 1], Random.Range(-1, 1)));
            }
        }

        for(int i = 0; i < outputs; i++)
        {
            for(int j = 0; j < hiddenLayerAmountOfNodes; j++)
            {
                connections.Add(new Connection(neurons[biasNeuronLocation + j + 1], neurons[inputs + i], Random.Range(-1, 1)));
            }
        }

        // Connect Bias Node
        for(int i = 0; i < hiddenLayerAmountOfNodes; i++)
        {
            connections.Add(new Connection(neurons[biasNeuronLocation], neurons[biasNeuronLocation + i + 1], Random.Range(-1, 1)));
        }

        for(int i = 0; i < outputs; i++)
        {
            connections.Add(new Connection(neurons[biasNeuronLocation], neurons[inputs + i], 1));
        }
    }

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

    public void BackPropagate(List<float> targets)
    {
        for(int i = 0; i < outputs; i++)
        {
            neurons[inputs + i].GradientDescent(targets[i]);
        }

        for(int i = 0; i < hiddenLayerAmountOfNodes; i++)
        {
            neurons[biasNeuronLocation + 1 + i].GradientDescent();
            neurons[biasNeuronLocation + 1 + i].ChangeWeights();
        }

        for (int i = 0; i < outputs; i++)
        {
            neurons[inputs + i].ChangeWeights();
        }
        
        for(int i = 0; i < connections.Count; i++)
        {
            connections[i].Mutate();
        }
    }

    // This function will give all required data to the neurons
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
    // for the network to go through the correct nodes from start to finish
    public void OrderNetwork()
    {
        ConnectNetwork();
        orderedNetwork = new List<Neuron>();

        for(int i = 0; i < layers; i++)
        {
            for(int j = 0; j < neurons.Count; j++)
            {
                if (neurons[j].layer == i)
                    orderedNetwork.Add(neurons[j]);
            }
        }

    }

    public List<Neuron> GetNeurons()
    {
        return neurons;
    }
    public List<Connection> GetConnections()
    {
        return connections;
    }

}
