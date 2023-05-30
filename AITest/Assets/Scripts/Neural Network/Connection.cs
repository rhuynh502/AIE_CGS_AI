using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Neuron fromNeuron;
    public Neuron toNeuron;

    public float weight;

    private float drasticMutateChance = 0.01f;
    private float mutateChance = 0.07f;
    private float rangeOfMutation = 0.03f;
    private float forceMutateRate = 0.1f;

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

        if(chanceToMutate < drasticMutateChance)
            weight = Random.Range(-1.0f, 1.0f);        
        else if (chanceToMutate < mutateChance)
            weight = Random.Range(weight - rangeOfMutation, weight + rangeOfMutation);
    }
    // In this mutation, only small changes to weights will happen. No weights will have drastic
    // changes to their values
    public void ForceMutate()
    {
        float chanceToMutate = Random.value;

        if (chanceToMutate < mutateChance)
            weight = Random.Range(weight - forceMutateRate, weight + forceMutateRate);
    }
}
