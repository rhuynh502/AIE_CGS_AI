using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
    void FixedUpdate()
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
        Parallel.For(0, population.Count, i =>
        {
            if (population[i].fitnessScore > bestScore)
            {
                bestScore = population[i].fitnessScore;
                bestPlayer = population[i].GetNetwork();
            }
        });

        population.Sort((a, b) => (CompareFitness(a.fitnessScore, b.fitnessScore)));

        Network bestOfThisGen = population[0].GetNetwork();
        Network baby;

        if (population[0].fitnessScore > bestScore)
            baby = CrossBreed(bestOfThisGen, bestPlayer);
        else
            baby = CrossBreed(bestPlayer, bestOfThisGen);

        for (int i = 0; i < population.Count; i++)
        {
            if (i < population.Count / 2)
                population[i].SetNetwork(bestPlayer.CloneNetwork());
            else
                population[i].SetNetwork(baby.CloneNetwork());

            if (i != 0)
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
        Network baby = _parent1.CloneNetwork();

        List<Connection> babiesConnections = baby.GetConnections();

        List<Connection> parent2Connections = _parent2.GetConnections();

        for(int i = 0; i < babiesConnections.Count; i++)
        {
            if(Random.value < 0.05f)
            {
                babiesConnections[i].weight = parent2Connections[i].weight;
            }        
        }

        baby.ResetNeurons();
        baby.OrderNetwork();

        return baby;
    }

    public void SaveButton()
    {
        population.Sort((a, b) => (CompareFitness(a.fitnessScore, b.fitnessScore)));
        SaveBestPlayer();
        bestPlayer = population[0].GetNetwork();
        bestScore = population[0].fitnessScore;
    }

    public void KillAll()
    {
        foreach (AIPlayer player in population)
            player.isAlive = false;
    }

    public void ResetBestFitness()
    {
        bestScore = 0;
        prevBestScore = 0;
    }

    private int CompareFitness(float a, float b)
    {
        if(a == b) return 0;

        return a < b ? 1 : -1;
    }

}
