using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Abstract interface for all Enemy movement behaviour classes
/// which includes base RunFromPlayer behaviour
///</summary>
public abstract class EnemyMoveBehaviour
{
    protected Enemy m_Enemy;

    public EnemyMoveBehaviour(Enemy enemy)
    {
        m_Enemy = enemy;
    }

    public abstract bool MoveNext();

    public void RunFromPlayer(float distance = 25f)
    {
        var player = GameController.Current.Player;

        Vector3 direction = (m_Enemy.transform.position - player.transform.position).normalized;

        m_Enemy.Agent.destination = m_Enemy.transform.position + (direction * distance);
    }
}
