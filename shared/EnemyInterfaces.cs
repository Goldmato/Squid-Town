using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public interface MoveBehaviour
{
    void Move(NavMeshAgent agent, Vector3 currentPos);
}
