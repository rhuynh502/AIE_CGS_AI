using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    [SerializeField] private List<AIPlayer> population;
    private Network bestPlayer;
    float bestScore;

    [SerializeField] private int amountInPop = 10;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckForAlive())
        {
            for(int i = 0; i < population.Count; i++) 
            {
                if (population[i].isAlive)
                {
                    population[i].Think();
                }
            }
        }
        else
        {
            // stop simulation, reset players and mutate.
        }
    }

    private bool CheckForAlive()
    {
        for(int i = 0; i < population.Count; i++)
        {
            if (population[i].isAlive)
                return false;
        }

        return true;
    }

    private void GeneratePopulation()
    {
        
    }
}
