using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron : MonoBehaviour
{
    // number is used to find where it is in the network
    int number;
    public int layer;
    public float inputSum;

    private float outputValue;

    // Need to know which nodes it is connected to so it can
    // send the output value to it
    private List<Connection> connections = new List<Connection>();

    // This will send the output value to the connections
    private void SendOutput()
    {
        // If this neuron is not in the inputs layer, send output value
        // loop through connections and add output value to their input sum
    }

    private void Sigmoid()
    {
        // calculate the output value using a sigmoid function
    }
}
