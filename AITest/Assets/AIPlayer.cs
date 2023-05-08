using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIPlayer : MonoBehaviour
{
    float fitnessScore;
    Network network;

    // These variables will be used to calculate fitness
    // These will change depending on what you need the ai to accomplish
    float timeAlive;
    bool isAlive = true;

    // Input amount is the amount of inputs of data the network
    // needs to make decisions
    [SerializeField] private int inputAmount;
    // Output amount is the amount of outputs the ai gives out
    // and makes an action depending on these outputs
    [SerializeField] private int outputAmount;


    // These are the inputs the network takes
    List<float> dependantVariables = new List<float>();
    // These are the outputs the network calculates
    List<float> outputVariables = new List<float>();

    // These variables determine what the ai should do
    [SerializeField] private List<bool> movementCommands;
    // These bools are in their respective positions in movement commands
    /*bool goForward;
    bool goBackward;
    bool goLeft;
    bool goRight;*/
    Vector3 targetPos;
    Vector3 respawnPos = new Vector3( -30, 1.15f, 30 );

    // need to make a class that spawns a bunch of players
    // that class also deals with when crossbreeding occurs.
    // something like a species or population class
    // this class should take care of the ai's functionality

    private void Awake()
    {
        network = new Network(inputAmount, outputAmount);
        network.OrderNetwork();
    }

    private void CalculateFitnessScore()
    {
        fitnessScore = timeAlive;
    }

    private void Update()
    {
        if(isAlive)
        {
            PerformActions();
            timeAlive += Time.deltaTime;
        }
    }

    // This is where the AI uses the inputs given by the user
    // and determines the values given into the network
    private void AssessSituation()
    {
        dependantVariables.Clear();
        // This is where modularity comes in. The user should be able to
        // write up their own inputs that the ai will take in
        for(int i = -Mathf.FloorToInt(inputAmount / 2); i <= Mathf.FloorToInt(inputAmount / 2); i++)
        {
            if(Physics.Raycast(transform.position, Quaternion.Euler(0, -15 * i, 0) * transform.TransformDirection(transform.forward), out RaycastHit hitInfo))
            {
                if(hitInfo.collider.CompareTag("Wall"))
                {
                    dependantVariables.Add(hitInfo.distance);
                }
                else
                {
                    dependantVariables.Add(0);
                }
            }
        }

        outputVariables = network.FeedForward(dependantVariables);
        for (int i = 0; i < outputAmount; i++)
        {
            if (outputVariables[i] > 0.5f)
                movementCommands[i] = true;
            else
                movementCommands[i] = false;
        }
    }

    // Actions are performed based off of the outputs given by the network
    // These will be toggleable bools or float values that can alter
    // specific variables as needed. eg. velocity of an object can change.
    private void PerformActions()
    {
        AssessSituation();
        
        if (movementCommands[0])
            transform.position += transform.TransformDirection(transform.forward) * Time.deltaTime * 2;
        if (movementCommands[1])
            transform.position -= transform.TransformDirection(transform.forward) * Time.deltaTime;
        if (movementCommands[2])
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + 0.5f);
        if (movementCommands[3])
            Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0.5f, 0, 0), 180);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = -Mathf.FloorToInt(inputAmount / 2); i <= Mathf.FloorToInt(inputAmount / 2); i++)
        {
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -15 * i, 0) * transform.TransformDirection(transform.forward) * 25);

        }
    }

    // The functions added past this point are specifically for the racing example
    // Use these as a scaffold for what you want to be making
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Wall"))
        {
            isAlive = false;
            CalculateFitnessScore();
            network.Mutate();
            transform.position = respawnPos;
            transform.rotation = Quaternion.Euler(90, 0, 90);
            isAlive = true;
        }
    }
}
