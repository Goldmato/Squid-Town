using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour 
{
    [HideInInspector] public bool Continue = true;

	[SerializeField] NavMeshAgent m_Agent;
/*    [SerializeField] GameObject m_Plane;

    [SerializeField] [Range(5f, 10f)] float m_PathFindDelay = 5f;*/

    Vector3 m_OriginalPos;
/*    Vector3 m_PlaneSize;
    Vector3 m_PlanePos;
    Vector2 m_PlaneOffset;*/

    void Start()
    {
        m_OriginalPos = transform.position;
/*        m_PlanePos = m_Plane.transform.position;
        m_PlaneSize = m_Plane.GetComponent<Collider>().bounds.size;
        m_PlaneOffset = new Vector2(m_PlanePos.x + (m_PlaneSize.x / 2), m_PlanePos.z + (m_PlaneSize.z / 2));*/
        StartCoroutine(GoToAllDoors());
    }

    ///<summary>
    /// Goes to every door starting with the nearest
    ///</summary>
    IEnumerator GoToAllDoors()
    {
        var doors = GameController.Current.Doors;

        Debug.Log(gameObject.name + " enemy beginning door pathfinding routine...");

        while(doors.Count > 0)
        {
            float minDist = float.MaxValue;
            int selectedDoor = 0;

            for (int i = 0; i < doors.Count; i++)
            {
                if(Vector3.Distance(transform.position, doors[i].transform.position) < minDist)
                {
                    selectedDoor = i;
                }
            }

            // Set selected doors as target destination and
            // remove from list
            m_Agent.destination = doors[selectedDoor].transform.position;
            doors.RemoveAt(selectedDoor);

            // Pause execution
            Continue = false;
            while(!Continue) { yield return null; }
        }
        Debug.Log("All doors successfully reached! Returning to starting position...");

        m_Agent.destination = m_OriginalPos;
        while (m_Agent.remainingDistance > 0.1f)
        {
            yield return null;
        }
        GetComponent<Animator>().enabled = false;
        Debug.Log(gameObject.name + " enemy door pathfinding routine finished!");
    }
}
