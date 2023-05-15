using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Neuron fromNeuron;
    public Neuron toNeuron;

    public float weight;

    public Connection(Neuron _fromNeuron, Neuron _toNeuron, float _weight)
    {
        fromNeuron = _fromNeuron;
        toNeuron = _toNeuron;
        weight = _weight;
    }


    // There is a chance for the weight of a connection to mutate
    public void Mutate()
    {
        float chanceToMutate = Random.value;

        if(chanceToMutate < 0.01f)
            weight = Random.Range(-1.0f, 1.0f);        
        else if (chanceToMutate < 0.1f)
            weight = Random.Range(weight - 0.05f, weight + 0.05f);
    }
}
