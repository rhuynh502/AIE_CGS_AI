using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICar : Car
{
    [SerializeField] public AIStats stats;

    Vector3 respawnPos;
    Quaternion respawnRot;
    public Vector3 latestCheckpointPos;

    private void Awake()
    {
        respawnPos = transform.position;
        respawnRot = transform.rotation;

        latestCheckpointPos = transform.position;
    }

    private void Update()
    {
        return;
    }

    public float GetAngle()
    {
        return stats.angleSpread;
    }

    public Vector3 GetRespawnPos()
    {
        return respawnPos;
    }

    public Quaternion GetRespawnRot()
    {
        return respawnRot;
    }

    public Vector3 GetLatestCheckPoint()
    {
        return latestCheckpointPos;
    }

    public void SetLatestCheckpointPos(Vector3 checkpoint)
    {
        latestCheckpointPos = checkpoint;
    }

    public void Drive(List<float> outputVariables)
    {
        velocityDirection = outputVariables[0];
        ForwardMovement(outputVariables[0]);
        TurnMovement(outputVariables[1] * GetTurnSpeed());
    }
}
