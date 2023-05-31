using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class AIPlayer : AIPlayerTemplate
{
    // These variables will be used to calculate fitness
    // These will change depending on what you need the ai to accomplish
    public bool isAlive = true;
    private float timeAlive;
    private float lifetime;

    private AICar carController;
    private float angleInDegrees;

    private LayerMask layerMask;

    private void Awake()
    {
        carController = GetComponent<AICar>();

        angleInDegrees = carController.GetAngle();
        lifetime = carController.stats.lifetime;

        inputAmount = carController.stats.amountOfInputs;
        outputAmount = carController.stats.amountOfOutputs;
        hiddenLayerAmount = carController.stats.amountOfHiddenNodes;

        layerMask = carController.stats.layersToScan;

        SetNetwork(new Network(inputAmount, outputAmount, hiddenLayerAmount));
        GetNetwork().OrderNetwork();
    }

    public override void CalculateFitnessScore()
    {
        fitnessScore += Vector3.Distance(carController.GetLatestCheckPoint(), transform.position);
    }

    public override void Think()
    {
        timeAlive += Time.deltaTime;

        PerformActions();
    }

    // This is where the AI uses the inputs given by the user
    // and determines the values it will give to the network
    public override void AssessSituation()
    {
        // Reset the dependant vairables for next feed forward
        dependantVariables.Clear();

        // The user should be able to
        // write up their own inputs that the ai will take in. In this case, the
        // AI needs to know where the walls are in respect to its own position. These
        // Raycasts give that distance to the player.
        float largestDist = 0;


        float rayAngleDif = (angleInDegrees * 2) / inputAmount;


        // Raycasts forward
        for (int i = 0; i < inputAmount; i++)
        {
            float rayAngle = angleInDegrees - (i * rayAngleDif);
            // Change to raycast mask to only hit walls
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, rayAngle, 0) * 
                transform.TransformDirection(transform.forward), out RaycastHit hitInfo, 
                Mathf.Infinity, layerMask))
            {
                if (hitInfo.collider.CompareTag("Wall"))
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

        for (int i = 0; i < inputAmount; i++)
        {
            dependantVariables[i] = 1 - dependantVariables[i] / largestDist;
        }

        // The amount of dependant variables must be the same as the amount of inputs to the network.
        // If it is not, it can cause some errors.
        outputVariables = GetNetwork().FeedForward(dependantVariables);

        if (carController.stats.canLearn && timeAlive > lifetime)
            isAlive = false;
        
    }

    // Actions are performed based off of the outputs given by the network
    public override void PerformActions()
    {
        if (fitnessScore < 0)
            isAlive = false;

        Vector3 initialPos = transform.position;

        AssessSituation();

        // outputs are passed to the script that drives the car.
        carController.Drive(outputVariables);
    }

    private void OnDrawGizmos()
    {
        if (!isAlive)
            return;
        Gizmos.color = Color.red;

        float rayAngleDif = (angleInDegrees * 2) / inputAmount;
        for (int i = 0; i < inputAmount; i++)
        {
            float rayAngle = angleInDegrees - (i * rayAngleDif);
            Gizmos.DrawRay(transform.position, Quaternion.Euler(0, rayAngle, 0) * transform.TransformDirection(transform.forward) * 25);

        }
        //Gizmos.DrawRay(transform.position, -transform.TransformDirection(transform.forward) * 25);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(carController.stats.canLearn && collision.collider.CompareTag("Wall"))
        {
            isAlive = false;
        }

    }

    public override void Mutate()
    {
        if(carController.stats.canLearn)
            GetNetwork().Mutate();
    }

    public void ResetTime()
    {
        timeAlive = 0;
    }

    public void Respawn()
    {
        transform.position = carController.GetRespawnPos();
        transform.rotation = carController.GetRespawnRot();

        isAlive = true;
        fitnessScore = 0;

        ResetTime();
    }
    public float GetTimeLeft()
    {
        return lifetime / timeAlive;
    }

    public void AddTime()
    {
        timeAlive -= carController.stats.timeAdd;
    }

    public void SubtractTime()
    {
        timeAlive += carController.stats.timeRemove;
    }

    public AICar GetCar()
    {
        return carController;
    }
}
