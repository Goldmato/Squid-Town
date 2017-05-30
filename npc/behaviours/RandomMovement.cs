using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RandomMovement : MoveBehaviour
{
    private Door m_PrevDoor;

    private float m_DetectionRange;
    private float m_MoveDistance;
    private float m_DoorTriggerDistance;

    // Default constructor
    public RandomMovement(float moveDistanceLow = 15f, float moveDistanceHigh = 30f,
        float detectionRange = 25f, float doorTriggerDistance = 25f)
    {
        m_DetectionRange = detectionRange;
        m_DoorTriggerDistance = doorTriggerDistance;
        m_MoveDistance = Random.Range(moveDistanceLow, moveDistanceHigh);
    }

    public void MoveNext(NavMeshAgent agent, Vector3 currentPos)
    {
        // TODO: Either figure out whether the player is in range within
        // this function or within the EnemyController, and find a place to run to

        if(m_PrevDoor != null)
            m_PrevDoor.Leave();

        agent.transform.Rotate(agent.transform.up, Random.Range(0, 360));
        Vector3 targetPos = currentPos + (agent.transform.forward * m_MoveDistance);

        foreach(var col in Physics.OverlapSphere(targetPos, m_DoorTriggerDistance, Physics.AllLayers))
        {
            if(col.CompareTag("Door"))
            {
                if(col.GetComponent<Door>().Occupy())
                {
                    m_PrevDoor = col.GetComponent<Door>();
                    agent.destination = col.transform.position;
                    return;
                }
            }
        }

        NavMeshHit hit;
        NavMesh.SamplePosition(targetPos, out hit, m_MoveDistance, NavMesh.AllAreas);
        agent.destination = hit.position;
    }
}
