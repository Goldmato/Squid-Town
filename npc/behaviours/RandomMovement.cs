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
    private float m_MoveDistance;
    private float m_DoorDistanceMultiplier;

    private BaseEnemy m_Enemy;

    // Default constructor
    public RandomMovement(BaseEnemy enemy, float moveDistanceLow = 15f, float moveDistanceHigh = 30f,
        float doorTriggerDistance = 10f, float doorDistanceMultiplier = 2f) : base(enemy, doorTriggerDistance)
    {
        m_Enemy = enemy;
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

        if(CheckForDoor(targetPos, m_DoorTriggerDistance))
            return false;

        m_Enemy.Agent.destination = targetPos;
        return true;
    }
}
