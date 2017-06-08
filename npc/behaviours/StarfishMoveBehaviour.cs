using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StarfishMoveBehaviour : EnemyMoveBehaviour
{
    private bool m_TurnState;

    const bool LEFT = false, RIGHT = true;

    public StarfishMoveBehaviour(BaseEnemy enemy) : base(enemy)
    {
    }

    public override bool MoveNext()
    {
        // TODO: Change to actual starfish behaviour instead of placeholder

        m_Enemy.transform.Rotate(m_Enemy.transform.up, Random.Range(0, 360));
        Vector3 targetPos = m_Enemy.transform.position + 
            (m_Enemy.transform.forward * 15);

        m_Enemy.Agent.destination = targetPos;
        return true;
    }

    public override void RunFromPlayer(float distance = 10)
    {
        var player = GameController.Current.Player;
        m_TurnState = !m_TurnState;
        float zigZagAngle = 45 * (m_TurnState ? 1 : -1);

        // TODO: Add slight variation to direction;
        Vector3 direction = (m_Enemy.transform.position - player.transform.position).normalized;
        direction = Quaternion.Euler(0, zigZagAngle, 0) * direction;
        Vector3 targetPos = m_Enemy.transform.position + (direction * distance);

        m_Enemy.Agent.destination = targetPos;
    }
}
