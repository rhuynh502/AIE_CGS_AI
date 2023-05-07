using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    float fitnessScore;
    Network network;

    // These variables will be used to calculate fitness
    // These will change depending on what you need the ai to accomplish
    float timeSpentOnTrack;
    float timeSpentOffTrack;
    int timesHittingWall;
    float timeSpentInCorrectDirection;

    // Input amount is the amount of inputs of data the network
    // needs to make decisions
    int inputAmount;
    // Output amount is the amount of outputs the ai gives out
    // and makes an action depending on these outputs
    int outputAmount;

    // These are the inputs the network takes
    List<float> dependantVariables = new List<float>();
    // These are the outputs the network calculates
    List<float> outputVariables = new List<float>();


    // need to make a class that spawns a bunch of players
    // that class also deals with when crossbreeding occurs.
    // something like a species or population class
    // this class should take care of the ai's functionality

    private AIPlayer()
    {
        network = new Network(inputAmount, outputAmount);
    }

    private void CalculateFitnessScore()
    {

    }

    private void Update()
    {
        
    }

    // This is where the AI uses the inputs given by the user
    // and determines the values given into the network
    private void AssessSituation()
    {
        network.TakeInInputs(dependantVariables);
    }

    // Actions are performed based off of the outputs given by the network
    // These will be toggleable bools or float values that can alter
    // specific variables as needed. eg. velocity of an object can change.
    private void PerformActions()
    {

    }
}
