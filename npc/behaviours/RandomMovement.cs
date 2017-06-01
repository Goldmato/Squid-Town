using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

///<summary>
/// Rotates the enemy to a random angle and
/// moves it forward a random distance, enters
/// doors that are within range
///</summary>
public class RandomMovement : EnemyMoveBehaviour
{
    private Door m_PrevDoor;

    private float m_MoveDistance;
    private float m_DoorTriggerDistance;
    private float m_DoorDistanceMultiplier;

    // Default constructor
    public RandomMovement(Enemy enemy, float moveDistanceLow = 15f, float moveDistanceHigh = 30f,
        float doorTriggerDistance = 10f, float doorDistanceMultiplier = 2f) : base(enemy)
    {
        m_DoorTriggerDistance = doorTriggerDistance;
        m_DoorDistanceMultiplier = doorDistanceMultiplier;
        m_MoveDistance = Random.Range(moveDistanceLow, moveDistanceHigh);
    }

    public override bool MoveNext()
    {
        m_Enemy.transform.Rotate(m_Enemy.transform.up, Random.Range(0, 360));
        Vector3 targetPos = m_Enemy.transform.position + 
            (m_Enemy.transform.forward * m_MoveDistance * (m_PrevDoor != null ? m_DoorDistanceMultiplier : 1));

        if(m_PrevDoor != null)
            m_PrevDoor.Leave();
        m_PrevDoor = null;

        foreach(var col in Physics.OverlapSphere(targetPos, m_DoorTriggerDistance, Physics.AllLayers))
        {
            if(col.CompareTag("Door"))
            {
                if(col.GetComponent<Door>().Occupy())
                {
                    m_PrevDoor = col.GetComponent<Door>();
                    m_Enemy.Agent.destination = m_PrevDoor.Edge;
                    return false;
                }
            }
        }

        m_Enemy.Agent.destination = targetPos;
        return true;
    }
}
