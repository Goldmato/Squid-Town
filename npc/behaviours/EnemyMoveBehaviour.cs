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
    public bool SkipUpdates { get { return m_SkipUpdates; } }

    protected Door m_PrevDoor;

    protected float m_DoorTriggerDistance;
    protected bool m_SkipUpdates;

    public EnemyMoveBehaviour(BaseEnemy enemy, float doorTriggerDistance = 10f)
    {
        m_DoorTriggerDistance = doorTriggerDistance;
    }

    protected virtual BaseEnemy Enemy { get; set; }

    public abstract bool MoveNext();

    public virtual void RunFromPlayer(float distance = 25f)
    {
        var player = GameController.Current.Player;

        // TODO: Add slight variation to direction;
        Vector3 direction = (Enemy.transform.position - player.transform.position).normalized;
        direction.x += Random.Range(-0.2f, 0.2f);
        direction.z += Random.Range(-0.2f, 0.2f);
        Vector3 targetPos = Enemy.transform.position + (direction * distance);

        if(CheckForDoor(targetPos, m_DoorTriggerDistance * 2))
            return;
        Enemy.Agent.destination = targetPos;
    }

    public virtual void GoRightOrLeft(bool rightOrLeft, float distance = 15f)
    {
        Vector3 direction = rightOrLeft ? Enemy.transform.right : -Enemy.transform.right;
        Enemy.Agent.destination = Enemy.transform.position + (direction * distance);
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
                    Enemy.Agent.destination = door.Edge;
                    m_PrevDoor = door;
                    return true;
                }
            }
        }
        return false;
    }
}
