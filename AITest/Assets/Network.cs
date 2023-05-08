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
    private List<float> inputValues;
    
    private List<Neuron> neurons = new List<Neuron>();
    private List<Connection> connections = new List<Connection>();
    int currNeuron = 0;
    int biasNeuronLocation;
    // A basic network starts with 2 layers. This can increase
    int layers = 2;

    private List<Neuron> orderedNetwork = new List<Neuron>();

    public Network(int _inputs, int _outputs)
    {
        // Generate a network with these amounts of inputs and outputs
        inputs = _inputs;
        outputs = _outputs;

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
            // Set to layer 1 because there are only 2 layers in the network
            // so far. This will change as more are added as outputs need to
            // be at the end
            neurons[i + inputs].layer = 1;
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

    }

    public List<float> FeedForward(List<float> inputsFromPlayer)
    {
        // Set the outputs for the input nodes. We know where they are in the
        // network because they are the first nodes we add in the constructor.
        for (int i = 0; i < inputs; i++)
        {
            neurons[i].outputValue = inputsFromPlayer[i];
        }
        // bias neurons output value is always a set 1
        neurons[biasNeuronLocation].outputValue = 1;

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

    // Add a random node to the existing network
    private void AddRandomNode()
    {
        // Get a random connection that is not the bias node
        int randomConnection = Random.Range(0, connections.Count);
        if(connections.Count != 0)
        {
            while (connections[randomConnection].fromNeuron == neurons[biasNeuronLocation])
                randomConnection = Random.Range(0, connections.Count);
        }

        // Add a new neuron and increment to keep track of which neuron this is
        neurons.Add(new Neuron(currNeuron));
        currNeuron++;

        // Create a connection in between the random connection and disable that connection
        if(connections.Count != 0)
        {
            connections.Add(new Connection(connections[randomConnection].fromNeuron, neurons[currNeuron], Random.value * Random.Range(0, 2) == 0 ? -1 : 1));
            connections.Add(new Connection(neurons[currNeuron], connections[randomConnection].toNeuron, Random.value * Random.Range(0, 2) == 0 ? -1 : 1));
            connections.RemoveAt(randomConnection);
            neurons[currNeuron].layer = connections[randomConnection].fromNeuron.layer + 1;
        }
        else
        {
            connections.Add(new Connection(neurons[Random.Range(0, inputs)], neurons[currNeuron], Random.value * Random.Range(0, 2) == 0 ? -1 : 1));
            connections.Add(new Connection(neurons[currNeuron], neurons[Random.Range(inputs + 1, inputs + outputs)], Random.value * Random.Range(0, 2) == 0 ? -1 : 1));
            neurons[currNeuron].layer = 1;
        }


        // Move layers up by one if new node conflicts
        if (neurons[currNeuron - 1].layer == connections[randomConnection].toNeuron.layer)
        {
            for (int i = 0; i < neurons.Count - 1; i++)
            {
                if (neurons[i].layer >= neurons[currNeuron - 1].layer)
                {
                    neurons[i].layer++;
                }
            }
            layers++;
        }
    }

    // Adds a random connection between two existing nodes in the network
    private void AddRandomConnection()
    {

        int node1 = Random.Range(0, neurons.Count);
        int node2 = Random.Range(0, neurons.Count);

        // Cannot add connection if neurons have the same layer
        // and are already connected
        while (neurons[node1].layer == neurons[node2].layer 
                && CheckForExistingConnection(neurons[node1], neurons[node2]))
        {
            node2 = Random.Range(0, neurons.Count);
        }

        // Check which node is ahead of the other. swap them if node1 is on a further layer
        if (neurons[node1].layer > neurons[node2].layer)
        {
            int temp = node1;
            node1 = node2;
            node2 = temp;
        }

        connections.Add(new Connection(neurons[node1], neurons[node2], Random.value * Random.Range(0, 2) == 0 ? -1 : 1));
    }

    public void Mutate()
    {
        if(Random.value < 0.75f)
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Mutate();
            }

        if(Random.value < 0.2f)
            AddRandomNode();
        if(Random.value < 0.1f)
            AddRandomConnection();
    }

    // This mixes the genes of two parents ie. mixes the connections they have together
    // This should not exceed the current amount of genes
    private Network CrossBreed(Network parent)
    {
        Network child = new Network(inputs, outputs);

        // from this parents connections, take random ones from each
        for (int i = 0; i < connections.Count; i++)
        {
            int pickAParent = Random.Range(0, 2);

            int randomConnection = Random.Range(0, connections.Count);

            while (CheckForExistingConnection(child, connections[i]) || CheckForExistingConnection(child, parent.connections[i]))
            {
                randomConnection = Random.Range(0, connections.Count);
            }

            if(pickAParent > 0.5f)
            {
                AddConnectionAndNeurons(child, this, randomConnection);
            }
            else
            {
                AddConnectionAndNeurons(child, parent, randomConnection);
            }
        }

        return child;
    }

    private bool CheckForExistingConnection(Neuron from, Neuron to)
    {
        for(int i = 0; i < from.connections.Count; i++)
        {
            if (from.connections[i].toNeuron == to)
                return false;
        }

        return true;
    }
    
    private bool CheckForExistingConnection(Network network, Connection connection)
    {
        if (network.connections.Contains(connection))
            return false;

        return true;
    }
    
    private void AddConnectionAndNeurons(Network child, Network parent, int numberConnection)
    {
        child.connections.Add(parent.connections[numberConnection]);
        if (!child.neurons.Contains(parent.connections[numberConnection].toNeuron))
        {
            child.neurons.Add(parent.connections[numberConnection].toNeuron);
        }
        if (!child.neurons.Contains(parent.connections[numberConnection].fromNeuron))
        {
            child.neurons.Add(parent.connections[numberConnection].fromNeuron);
        }
    }

}
