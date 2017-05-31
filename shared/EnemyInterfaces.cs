using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public interface EnemyMoveBehaviour
{
    void MoveNext(NavMeshAgent agent, Vector3 currentPos);
}
