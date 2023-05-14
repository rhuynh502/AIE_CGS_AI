using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIPlayer : MonoBehaviour
{
    float fitnessScore;
    private Network network;

    // These variables will be used to calculate fitness
    // These will change depending on what you need the ai to accomplish
    float timeAlive;
    public bool isAlive = true;

    // Input amount is the amount of inputs of data the network
    // needs to make decisions
    [SerializeField] private int inputAmount;
    // Output amount is the amount of outputs the ai gives out
    // and makes an action depending on these outputs
    [SerializeField] private int outputAmount;
    [SerializeField] private int hiddenLayerAmount;


    // These are the inputs the network takes
    List<float> dependantVariables = new List<float>();
    // These are the outputs the network calculates
    List<float> outputVariables = new List<float>();

    List<float> targets = new List<float>();

    // These variables determine what the ai should do
    [SerializeField] private List<bool> movementCommands;
    private float amountOfCasts = 10;

    Vector3 respawnPos;

    // need to make a class that spawns a bunch of players
    // that class also deals with when crossbreeding occurs.
    // something like a species or population class
    // this class should take care of the ai's functionality

    private void Awake()
    {
        network = new Network(inputAmount, outputAmount, hiddenLayerAmount);
        network.OrderNetwork();
        respawnPos = transform.position;
    }

    private void CalculateFitnessScore()
    {
        fitnessScore = timeAlive;
    }

    public void Think()
    {
        PerformActions();
        timeAlive += Time.deltaTime;
        
    }

    // This is where the AI uses the inputs given by the user
    // and determines the values given into the network
    private void AssessSituation()
    {
        dependantVariables.Clear();
        targets.Clear();
        // This is where modularity comes in. The user should be able to
        // write up their own inputs that the ai will take in
        float largestDist = 0;
        float closestDist = 9999;
        int largestDistPos = 0;

        float viewAngle = 90f;
        float rayAngleDif = (viewAngle * 2) / amountOfCasts;

        // Raycasts forward
        for(int i = 0; i <= amountOfCasts; i++)
        {
            float rayAngle = viewAngle - (i * rayAngleDif);
            // Change to raycast mask to only hit walls
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, rayAngle, 0) * transform.TransformDirection(transform.forward), out RaycastHit hitInfo))
            {
                if(hitInfo.collider.CompareTag("Wall"))
                {
                    dependantVariables.Add(hitInfo.distance);
                    if (hitInfo.distance > largestDist)
                    {
                        largestDist = hitInfo.distance;
                        largestDistPos = i;
                    }
                    if (hitInfo.distance < closestDist)
                    {
                        closestDist = hitInfo.distance;
                    }
                    continue;
                }
                
            }
            dependantVariables.Add(0);
        }

        // Raycast backwards
        if (Physics.Raycast(transform.position, -transform.TransformDirection(transform.forward), out RaycastHit behindHitInfo))
        {
            if (behindHitInfo.collider.CompareTag("Wall"))
            {
                dependantVariables.Add(behindHitInfo.distance);            
            }
        }
        else
            dependantVariables.Add(0);

        for (int i = 0; i < inputAmount; i++)
        {
            dependantVariables[i] = 1 - dependantVariables[i] / largestDist;
        }

        targets.Add(1);
        outputVariables = network.FeedForward(dependantVariables);
        if (outputVariables[1] > 0)
            targets.Add(-0.8f);
        else
            targets.Add(0.8f);

        Debug.Log($"{outputVariables[0]} || {outputVariables[1]}");

    }

    // Actions are performed based off of the outputs given by the network
    private void PerformActions()
    {
        AssessSituation();
        transform.position += transform.TransformDirection(transform.forward) * outputVariables[0] * Time.deltaTime;
        transform.Rotate(0, 5 * outputVariables[1] * Time.deltaTime, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float viewAngle = 90f;
        float rayAngleDif = (viewAngle * 2) / amountOfCasts;
        for (int i = 0; i <= amountOfCasts; i++)
        {
            float rayAngle = viewAngle - (i * rayAngleDif);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, rayAngle, 0) * transform.TransformDirection(transform.forward) * 25);

        }
        Gizmos.DrawRay(transform.position, -transform.TransformDirection(transform.forward) * 25);
    }

    // The functions added past this point are specifically for the racing example
    // Use these as a scaffold for what you want to be making
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Wall"))
        {
            isAlive = false;
            CalculateFitnessScore();
            timeAlive = 0;
            network.BackPropagate(targets);
            transform.position = respawnPos;
            transform.rotation = Quaternion.Euler(0, 45, 0);

            isAlive = true;
        }
    }

    public Network GetNetwork()
    {
        return network;
    }
}
