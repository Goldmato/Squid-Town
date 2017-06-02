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
    protected Door m_PrevDoor;

    protected float m_DoorTriggerDistance;

    public EnemyMoveBehaviour(Enemy enemy, float doorTriggerDistance = 10f)
    {
        m_Enemy = enemy;
        m_DoorTriggerDistance = doorTriggerDistance;
    }

    public abstract bool MoveNext();

    public virtual void RunFromPlayer(float distance = 25f)
    {
        var player = GameController.Current.Player;
        
        // TODO: Add slight variation to direction;
        Vector3 direction = (m_Enemy.transform.position - player.transform.position).normalized;
        direction.x += Random.Range(-0.2f, 0.2f);
        direction.z += Random.Range(-0.2f, 0.2f);
        Vector3 targetPos = m_Enemy.transform.position + (direction * distance);

        if(CheckForDoor(targetPos, m_DoorTriggerDistance * 2))
            return;
        m_Enemy.Agent.destination = targetPos;
    }

    public virtual void GoRightOrLeft(bool rightOrLeft, float distance = 15f)
    {
        Vector3 direction = rightOrLeft ? m_Enemy.transform.right : -m_Enemy.transform.right;
        m_Enemy.Agent.destination = m_Enemy.transform.position + (direction * distance);
    }

    protected bool CheckForDoor(Vector3 targetPos, float triggerDistance)
    {
        foreach(var col in Physics.OverlapSphere(targetPos, triggerDistance, Physics.AllLayers))
        {
            if(col.CompareTag("Door"))
            {
                var door = col.GetComponent<Door>();
                if(door != null && door.Occupy())
                {
                    m_Enemy.Agent.destination = door.Edge;
                    m_PrevDoor = door;
                    return true;
                }
            }
        }
        return false;
    }
}
