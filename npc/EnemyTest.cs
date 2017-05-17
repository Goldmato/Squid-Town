using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour 
{
	[SerializeField] NavMeshAgent m_Agent;
    [SerializeField] GameObject m_Plane;

    [SerializeField] [Range(5f, 10f)] float m_PathFindDelay = 5f;

    Vector3 m_PlaneSize;
    Vector3 m_PlanePos;
    Vector2 m_PlaneOffset;

    void Start()
    {
        m_PlanePos = m_Plane.transform.position;
        m_PlaneSize = m_Plane.GetComponent<Collider>().bounds.size;
        m_PlaneOffset = new Vector2(m_PlanePos.x + (m_PlaneSize.x / 2), m_PlanePos.z + (m_PlaneSize.z / 2));
        StartCoroutine(FindRandomTarget());
    }

    IEnumerator FindRandomTarget()
    {
        while (true)
        {
            m_Agent.destination = new Vector3 (Random.Range(-m_PlaneOffset.x, m_PlaneOffset.x), m_PlanePos.y, 
                                            Random.Range(-m_PlaneOffset.y, m_PlaneOffset.y));
            yield return new WaitForSeconds(m_PathFindDelay);
        }
    }
}
