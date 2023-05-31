using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AIPlayerTemplate : MonoBehaviour
{
    public float fitnessScore;
    private Network network;

    // Input amount is the amount of inputs of data the network
    // needs to make decisions
    public int inputAmount;
    // Output amount is the amount of outputs the ai gives out
    // and makes an action depending on these outputs
    public int outputAmount;
    // Hidden layer holds the nodes that lie in between the inputs and outputs.
    // You can play around with how many you want in here.
    public int hiddenLayerAmount;

    // These are the inputs the network takes
    public List<float> dependantVariables = new List<float>();
    // These are the outputs the network calculates
    public List<float> outputVariables = new List<float>();

    // IMPORTANT
    // Initialise the input, output and hidden layer amounts in your script in an Awake call
    // and set your network there using those values.

    public abstract void CalculateFitnessScore();

    public virtual void Think()
    {
        PerformActions();
    }

    // This is where the AI uses the inputs given by the user
    // and determines the values it will give to the network
    public abstract void AssessSituation();


    // Actions are performed based off of the outputs given by the network
    public virtual void PerformActions()
    {
        AssessSituation();
    }

    public virtual void Mutate()
    {
        network.Mutate();
    }

    public void ForceMutate()
    {
        network.ForceMutate();
    }

    public Network GetNetwork()
    {
        return network;
    }

    public void SetNetwork(Network _network)
    {
        network = _network;
    }
}
