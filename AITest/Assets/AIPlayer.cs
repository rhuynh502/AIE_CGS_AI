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
    [SerializeField] private int hiddenLayerAmount;


    // These are the inputs the network takes
    List<float> dependantVariables = new List<float>();
    // These are the outputs the network calculates
    List<float> outputVariables = new List<float>();

    List<float> targets = new List<float>();

    // These variables determine what the ai should do
    [SerializeField] private List<bool> movementCommands;
    // These bools are in their respective positions in movement commands
    /*bool goForward;
    bool goBackward;
    bool goLeft;
    bool goRight;*/
    Vector3 targetPos;
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
        targets.Clear();
        // This is where modularity comes in. The user should be able to
        // write up their own inputs that the ai will take in
        float largestDist = 0;
        float closestDist = 9999;
        int largestDistPos = 0;
        int closestDistPos = 0;

        float viewAngle = 45f;
        float rayAngleDif = (viewAngle * 2) / inputAmount;

        for(int i = 0; i < inputAmount; i++)
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
                        closestDistPos = i;
                    }
                    continue;
                }
                
            }
            dependantVariables.Add(0);
        }

        for (int i = 0; i < inputAmount; i++)
        {
            dependantVariables[i] /= largestDist;
        }

        targets.Add(largestDist / 10);
        targets.Add(closestDist / 50);

        float forwardRadians = Mathf.Atan2(transform.forward.z, transform.forward.x);

        float targetRadians = forwardRadians - 0.37f * (closestDistPos - (inputAmount / 2) + (largestDistPos - (inputAmount / 2)));

        targets.Add(Vector3.Dot(transform.forward, new Vector3(Mathf.Cos(targetRadians), 0, Mathf.Sin(targetRadians))));

        outputVariables = network.FeedForward(dependantVariables);
        Debug.Log($"{outputVariables[0]} || {outputVariables[1]}");

    }

    // Actions are performed based off of the outputs given by the network
    private void PerformActions()
    {
        AssessSituation();

        transform.position += transform.TransformDirection(transform.forward) * Time.deltaTime * 2 * (outputVariables[0] * outputVariables[1]);

        float forwardRadians = Mathf.Atan2(transform.forward.z, transform.forward.x);
        float targetRadians = forwardRadians - 0.37f * outputVariables[2] * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(new Vector3(Mathf.Cos(targetRadians), 0, Mathf.Sin(targetRadians)));
        transform.forward = new Vector3(Mathf.Cos(targetRadians), 0, Mathf.Sin(targetRadians));
        /*if (movementCommands[0])
        {
            float forwardRadians = Mathf.Atan2(transform.forward.z, transform.forward.x);
            float targetRadians = forwardRadians + 0.1f * Time.deltaTime;

            float temp = transform.rotation.y;
            transform.rotation = Quaternion.LookRotation(new Vector3(Mathf.Cos(targetRadians), 0, Mathf.Sin(targetRadians)));
        }
        if (movementCommands[1])
        {
            float forwardRadians = Mathf.Atan2(transform.forward.z, transform.forward.x);
            float targetRadians = forwardRadians - 0.1f * Time.deltaTime;

            float temp = transform.rotation.y;
            transform.rotation = Quaternion.LookRotation(new Vector3(Mathf.Cos(targetRadians), 0, Mathf.Sin(targetRadians)));
        }*/

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
            timeAlive = 0;
            network.BackPropagate(targets);
            transform.position = respawnPos;
            transform.rotation = Quaternion.Euler(0, 45, 0);

            isAlive = true;
        }
    }
}
