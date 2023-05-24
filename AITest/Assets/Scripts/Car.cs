using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private bool isPlayerControlled;

    private int moveSpeed;
    private float turnSpeed;
    private float speedModifier = 0.95f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = playerStats.moveSpeed;
        turnSpeed = playerStats.turnSpeed;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isPlayerControlled)
            return;

        if(Input.GetKey(KeyCode.W))
            ForwardMovement(speedModifier);
        if (Input.GetKey(KeyCode.S))
            ForwardMovement(-speedModifier);
        if (Input.GetKey(KeyCode.A))
            TurnMovement(-turnSpeed);
        if (Input.GetKey(KeyCode.D))
            TurnMovement(turnSpeed);

        if (Input.GetKey(KeyCode.LeftShift))
            speedModifier = 0.5f;
        else
            speedModifier = 0.95f;
    }

    public void ForwardMovement(float directionOfMotion)
    {
        //transform.position += transform.TransformDirection(transform.forward) * moveSpeed * directionOfMotion * Time.deltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(transform.forward) * moveSpeed * directionOfMotion * Time.deltaTime);
    }

    public void TurnMovement(float amountOfTurn)
    {
        //transform.Rotate(0, amountOfTurn * Time.deltaTime, 0);
        rb.MoveRotation(transform.rotation * Quaternion.Euler(0, amountOfTurn * Time.deltaTime, 0));
    }

    public float GetTurnSpeed()
    {
        return turnSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 prevPos = transform.position;

        if (collision.collider.CompareTag("Wall"))
            transform.position = prevPos;
    }
}
