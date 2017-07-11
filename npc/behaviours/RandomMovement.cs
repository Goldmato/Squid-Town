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

    // Default constructor
    public RandomMovement(BaseEnemy enemy, float moveDistanceLow = 15f, float moveDistanceHigh = 30f,
        float doorTriggerDistance = 10f, float doorDistanceMultiplier = 2f) : base(enemy, doorTriggerDistance)
    {
        Enemy = enemy;
        m_DoorDistanceMultiplier = doorDistanceMultiplier;
        m_MoveDistance = Random.Range(moveDistanceLow, moveDistanceHigh);
    }

    public override bool MoveNext()
    {
        Enemy.transform.Rotate(Enemy.transform.up, Random.Range(0, 360));
        Vector3 targetPos = Enemy.transform.position +
            (Enemy.transform.forward * m_MoveDistance * (m_PrevDoor != null ? m_DoorDistanceMultiplier : 1));

        if(m_PrevDoor != null)
            m_PrevDoor.Leave();
        m_PrevDoor = null;

        if(CheckForDoor(targetPos, m_DoorTriggerDistance))
            return false;

        Enemy.Agent.destination = targetPos;
        return true;
    }
}
