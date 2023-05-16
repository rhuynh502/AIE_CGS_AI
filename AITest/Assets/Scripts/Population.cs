using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Population : MonoBehaviour
{
    [SerializeField] private List<AIPlayer> population;
    private Network bestPlayer;
    public float bestScore;
    private float prevBestScore;

    List<Network> bestOfThisGen = new List<Network>();

    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] robotGameObjects = GameObject.FindGameObjectsWithTag("Robot");

        foreach (GameObject robot in robotGameObjects)
        {
            population.Add(robot.GetComponent<AIPlayer>());
        }

        try
        {
            using(StreamReader reading = new StreamReader("BestPlayer.txt"))
            {
                bestScore = float.Parse(reading.ReadLine(), System.Globalization.NumberStyles.Float);

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
        prevBestScore = bestScore;
    }

    private void Start()
    {
        if(bestPlayer != null)
            foreach (AIPlayer player in population)
            {
                player.SetNetwork(bestPlayer.CloneNetwork());
                player.Mutate();
            }
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
            population[i].CalculateFitnessScore();
            if (population[i].fitnessScore > bestScore)
            {
                bestScore = population[i].fitnessScore;
                bestPlayer = population[i].GetNetwork();
            }
        }

        population.Sort((a, b) => (CompareFitness(a.fitnessScore, b.fitnessScore)));

        //Network baby = CrossBreed(population[Random.Range(0, 5)].GetNetwork(), population[Random.Range(5, 10)].GetNetwork());

        Network leader1 = population[0].GetNetwork();
        Network leader2 = population[Random.Range(1, 3)].GetNetwork();

        for (int i = 0; i < population.Count; i++)
        {
            if (i < population.Count / 2)
                population[i].SetNetwork(leader1.CloneNetwork());
            else
                population[i].SetNetwork(leader2.CloneNetwork());

            // Dont mutate the first one to keep consistency
            population[i].Mutate();
            population[i].Respawn();
        }

    }

    private void OnDestroy()
    {
        if(bestScore > prevBestScore)
            SaveBestPlayer();
    }

    private void SaveBestPlayer()
    {
        using(StreamWriter writeText = new StreamWriter("BestPlayer.txt"))
        {
            writeText.WriteLine(bestScore);
            writeText.WriteLine(bestPlayer.GetInputsAmount());
            writeText.WriteLine(bestPlayer.GetOutputsAmount());
            writeText.WriteLine(bestPlayer.GetHiddenLayerAmount());

            List<Connection> saveData = bestPlayer.GetConnections();
            for(int i = 0; i < saveData.Count; i++)
                writeText.WriteLine(saveData[i].weight);
            
        }
    }

    // This function returns a cross breed of two selected parents. _parent 1 should have the better fitness score
    private Network CrossBreed(Network _parent1, Network _parent2)
    {
        Network baby = new Network(_parent1.GetInputsAmount(), _parent1.GetOutputsAmount(), _parent1.GetHiddenLayerAmount());

        List<Connection> babiesConnections = baby.GetConnections();
        List<Connection> parent1Connections = _parent1.GetConnections();
        List<Connection> parent2Connections = _parent2.GetConnections();

        for(int i = 0; i < babiesConnections.Count; i++)
        {
            if(Random.value > 0.25)
                babiesConnections[i].weight = parent1Connections[i].weight;
            else
                babiesConnections[i].weight = parent2Connections[i].weight;
        }

        baby.OrderNetwork();

        return baby;
    }

    private int CompareFitness(float a, float b)
    {
        return a < b ? 1 : -1;
    }

}
