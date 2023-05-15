using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIPlayer : MonoBehaviour
{
    public float fitnessScore;
    private Network network;

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float moveSpeed = 4;
    // These variables will be used to calculate fitness
    // These will change depending on what you need the ai to accomplish
    float timeAlive;
    public bool isAlive = true;
    private float lifetime = 480;

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
    public float velocityDirection;

    List<float> targets = new List<float>();

    // These variables determine what the ai should do
    [SerializeField] private List<bool> movementCommands;
    private float amountOfCasts = 10;
    private Collider collider;

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
        collider = GetComponent<Collider>();
    }

    public void CalculateFitnessScore()
    {
        fitnessScore += Vector3.Distance(respawnPos, transform.position) - (Vector3.Distance(respawnPos, transform.position) / timeAlive);
    }

    public void Think()
    {
        
        timeAlive += Time.deltaTime;
        PerformActions();
        
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

        float viewAngle = 90f;
        float rayAngleDif = (viewAngle * 2) / amountOfCasts;

        // Raycasts forward
        for(int i = 0; i <= amountOfCasts; i++)
        {
            float rayAngle = viewAngle - (i * rayAngleDif);
            // Change to raycast mask to only hit walls
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, rayAngle, 0) * transform.TransformDirection(transform.forward), out RaycastHit hitInfo, Mathf.Infinity, layerMask))
            {
                if(hitInfo.collider.CompareTag("Wall"))
                {
                    dependantVariables.Add(hitInfo.distance);
                    if (hitInfo.distance > largestDist)
                    {
                        largestDist = hitInfo.distance;
                    }
                    continue;
                }
                
            }
            dependantVariables.Add(0);
        }

        // Raycast backwards
        if (Physics.Raycast(transform.position, -transform.TransformDirection(transform.forward), out RaycastHit behindHitInfo, Mathf.Infinity, layerMask))
        {
            if (behindHitInfo.collider.CompareTag("Wall"))
            {
                dependantVariables.Add(behindHitInfo.distance);            
            }
            else
                dependantVariables.Add(0);
        }
        else
            dependantVariables.Add(0);

        for (int i = 0; i < inputAmount; i++)
        {
            dependantVariables[i] = 1 - dependantVariables[i] / largestDist;
        }

        outputVariables = network.FeedForward(dependantVariables);
        targets.Add(1);
        if (outputVariables[1] > 0)
            targets.Add(-0.9f);
        else
            targets.Add(0.9f);

        if (timeAlive > lifetime)
            isAlive = false;
        
    }

    // Actions are performed based off of the outputs given by the network
    private void PerformActions()
    {
        Vector3 initialPos = transform.position;
        AssessSituation();
        velocityDirection = outputVariables[0];
        transform.position += transform.TransformDirection(transform.forward) * outputVariables[0] * Time.deltaTime;
        transform.Rotate(0, 5 * outputVariables[1] * Time.deltaTime, 0);

    }

    private void OnDrawGizmos()
    {
        if (!isAlive)
            return;
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
        }
        else
        {
            Physics.IgnoreCollision(collision.collider, collider);
        }
    }

    public void Mutate()
    {
        //BackPropagate();
        network.Mutate();
    }

    public void BackPropagate()
    {
        network.BackPropagate(targets);
    }

    public Network GetNetwork()
    {
        return network;
    }

    public void SetNetwork(Network _network)
    {
        network = _network;
    }

    public void ResetTime()
    {
        timeAlive = 0;

        transform.position = respawnPos;
        transform.rotation = Quaternion.Euler(0, 45, 0);
    }
}
