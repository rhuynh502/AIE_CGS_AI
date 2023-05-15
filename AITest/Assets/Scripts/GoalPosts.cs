using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPosts : MonoBehaviour
{
    public List<AIPlayer> m_players = new List<AIPlayer>();
    [SerializeField] private Vector3 intendedDirection;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<AIPlayer>(out AIPlayer player))
        {
            if(!m_players.Contains(player))
            {
                if(Vector3.Dot(intendedDirection, player.transform.TransformDirection(player.transform.forward) * player.velocityDirection) > 0)
                    player.fitnessScore += 50;
                else
                    player.fitnessScore -= 50;

                m_players.Add(player);
            }
        }
    }

    public void ResetPlayerList()
    {
        m_players.Clear();
    }
}
