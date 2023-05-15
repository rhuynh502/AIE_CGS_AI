using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Population : MonoBehaviour
{
    [SerializeField] private List<AIPlayer> population;
    [SerializeField] private List<GoalPosts> goals;
    private Network bestPlayer;
    public float bestScore;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] goalGameObjects = GameObject.FindGameObjectsWithTag("Goals");

        foreach(GameObject goal in goalGameObjects)
        {
            goals.Add(goal.GetComponent<GoalPosts>());
        }

        GameObject[] robotGameObjects = GameObject.FindGameObjectsWithTag("Robot");

        foreach (GameObject robot in robotGameObjects)
        {
            population.Add(robot.GetComponent<AIPlayer>());
        }

        try
        {
            using(StreamReader reading = new StreamReader("BestPlayer.txt"))
            {
                bestPlayer = new Network(int.Parse(reading.ReadLine(), System.Globalization.NumberStyles.Integer),
                    int.Parse(reading.ReadLine(), System.Globalization.NumberStyles.Integer),
                    int.Parse(reading.ReadLine(), System.Globalization.NumberStyles.Integer));

                bestPlayer.OrderNetwork();

                List<Connection> connectionsLoad = bestPlayer.GetConnections();

                for(int i = 0; i < connectionsLoad.Count; i++)
                {
                    connectionsLoad[i].weight = float.Parse(reading.ReadLine(), System.Globalization.NumberStyles.Float);
                }
            }

        }
        catch
        {

        }
    }

    private void Start()
    {
        if(bestPlayer != null)
            foreach (AIPlayer player in population)
                player.SetNetwork(bestPlayer);
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
            RegeneratePopulation();
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

    private void RegeneratePopulation()
    {
        for(int i = 0; i < population.Count; i++)
        {
            //population[i].CalculateFitnessScore();
            population[i].ResetTime();
            if (population[i].fitnessScore > bestScore)
            {
                bestScore = population[i].fitnessScore;
                bestPlayer = population[i].GetNetwork();
            }
        }

        foreach(GoalPosts goal in goals)
        {
            goal.ResetPlayerList();
        }

        for (int i = 0; i < population.Count; i++)
        {
            if(bestScore != 0)
                population[i].SetNetwork(bestPlayer.CloneNetwork());

            population[i].Mutate();
            population[i].fitnessScore = 0;
            population[i].isAlive = true;
        }

    }

    private void OnDestroy()
    {
        SaveBestPlayer();
    }

    private void SaveBestPlayer()
    {
        using(StreamWriter writeText = new StreamWriter("BestPlayer.txt"))
        {
            writeText.WriteLine(bestPlayer.GetInputsAmount());
            writeText.WriteLine(bestPlayer.GetOutputsAmount());
            writeText.WriteLine(bestPlayer.GetHiddenLayerAmount());

            List<Connection> saveData = bestPlayer.GetConnections();
            for(int i = 0; i < saveData.Count; i++)
                writeText.WriteLine(saveData[i].weight);

        }
    }

}
