using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour 
{
    [HideInInspector] public bool Continue = true;

	[SerializeField] NavMeshAgent m_Agent;
    [SerializeField] GameObject m_Plane;

    [SerializeField] [Range(5f, 10f)] float m_PathFindDelay = 5f;

    Vector3 m_OriginalPos;
    Vector3 m_PlaneSize;
    Vector3 m_PlanePos;
    Vector2 m_PlaneOffset;

    void Start()
    {
        m_OriginalPos = transform.position;
        m_PlanePos = m_Plane.transform.position;
        m_PlaneSize = m_Plane.GetComponent<Collider>().bounds.size;
        m_PlaneOffset = new Vector2(m_PlanePos.x + (m_PlaneSize.x / 2), m_PlanePos.z + (m_PlaneSize.z / 2));
        StartCoroutine(GoToAllDoors());
    }

    IEnumerator GoToAllDoors()
    {
        var doors = Registry.Current.Doors;

        Debug.Log(gameObject.name + " enemy beginning door pathfinding routine...");

        for (int i = 0; i < doors.Count; i++)
        {
            Continue = false;

            m_Agent.destination = doors[i].transform.position;
            do 
            {
                yield return null;
            } while (!Continue);

            yield return new WaitForSeconds(1);
            Debug.Log("Door[" + i + "] succesfully reached, moving on to next target...");
        }

        m_Agent.destination = m_OriginalPos;
        while (m_Agent.remainingDistance > 0.1f)
        {
            yield return null;
        }
        GetComponent<Animator>().enabled = false;
        Debug.Log(gameObject.name + " enemy door pathfinding routine finished!");
    }
}
