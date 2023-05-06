using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// The network should handle all the connections between nodes
// It also keeps track of the structure of the system
public class Network : MonoBehaviour
{
    // These are determined in the agent class
    private int inputs;
    private int outputs;
    private float[] inputValues;
    
    private List<Neuron> neurons = new List<Neuron>();
    private List<Connection> connections = new List<Connection>();
    int currNeuron = 0;
    int biasNeuronLocation;

    private Network(int _inputs, int _outputs)
    {
        // Generate a network with these amounts of inputs and outputs
        inputs = _inputs;
        outputs = _outputs;

        // Generate the input nodes/neurons
        for(int i = 0; i < inputs; i++)
        {
            neurons.Add(new Neuron());
            currNeuron++;
            // Sets layer to 0 because input layers are always first
            neurons[i].layer = 0;
        }

        // Generate the output nodes/neurons
        // i + inputs is used to continue the list from where it ended
        for(int i = 0; i < outputs; i++)
        {
            neurons.Add(new Neuron());
            // Set to layer 1 because there are only 2 layers in the network
            // so far. This will change as more are added as outputs need to
            // be at the end
            neurons[i + inputs].layer = 1;
        }

        // Generate the bias node/neuron
        neurons.Add(new Neuron());
        // This keeps track of where the bias neuron is.
        // The bias neuron is needed in every feedforward operation.
        biasNeuronLocation = currNeuron;
        // Bias neuron will always be on first layer
        neurons[biasNeuronLocation].layer = 0;
    }

    // Add a node to the existing network
    private void AddNode()
    {

    }

    // Adds a connection between two existing nodes in the network
    private void AddConnection()
    {

    }

    // This mixes the genes of two parents ie. mixes the connections they have together
    // This should not exceed the current amount of genes
    private void CrossBreed()
    {

    }

    // Each network has a chance to mutate which alters the network
    // This could be adding a new node, a new connection or alterting an existing weight
    private void Mutate()
    {

    }
}
