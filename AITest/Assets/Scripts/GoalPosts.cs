using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPosts : MonoBehaviour
{
    [SerializeField] private Vector3 intendedDirection;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<AIPlayer>(out AIPlayer player))
        {
            if (Vector3.Dot(intendedDirection, player.transform.TransformDirection(player.transform.forward) * player.GetCar().velocityDirection) >= 0.25f)
            {
                player.CalculateFitnessScore();
                player.AddTime();
            }
            else
            {
                player.SubtractTime();
            }

            player.GetCar().SetLatestCheckpointPos(transform.position);
 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<AIPlayer>(out AIPlayer player))
        {
            if (Vector3.Dot(intendedDirection, player.transform.TransformDirection(player.transform.forward) * player.GetCar().velocityDirection) >= 0)
            {
                player.AddTime();
            }
            else
            {
                player.SubtractTime();
            }

        }
    }

}
