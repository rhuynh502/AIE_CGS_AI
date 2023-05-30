using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private Vector3 intendedDirection;
    [SerializeField] private Transform winScreen;
    [SerializeField] private Transform loseScreen;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Car>(out Car car))
        {
            if (Vector3.Dot(intendedDirection, car.transform.TransformDirection(car.transform.forward) * car.velocityDirection) > 0.25f)
            {
                car.IncrementLaps();
                CheckForWin(car);
            }
            else
            {
                car.DecrementLaps();

            }
        }
    }

    private void CheckForWin(Car car)
    {
        if (car.amountOfLaps >= 4)
        {
            Time.timeScale = 0;
            if (car.GetIsPlayerControlled())
                winScreen.gameObject.SetActive(true);
            else
                loseScreen.gameObject.SetActive(true);
        }
    }
}
