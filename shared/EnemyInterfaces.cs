using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Abstract interface for all Enemy movement behaviour classes
///</summary>
public abstract class EnemyMoveBehaviour
{
    protected Enemy m_Enemy;

    public EnemyMoveBehaviour(Enemy enemy)
    {
        m_Enemy = enemy;
    }

    public abstract void MoveNext();

    public void RunFromPlayer(float distance = 25f)
    {
        // TODO: Find angle between player and enemy and send enemy
        // a random distance in that direction
        Debug.Log("Running from player!");

        var player = GameController.Current.Player;

        Vector3 direction = (m_Enemy.transform.position - player.transform.position).normalized;

        m_Enemy.Agent.destination = m_Enemy.transform.position + (direction * distance);
    }
}
