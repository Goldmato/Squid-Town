using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Class controlling door behaviour/data
///</summary>
[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour
{
    public Vector3 Edge { get { return m_DoorEdge; } }
    public bool Occupied { get { return m_IsOccupied; } }

    private Vector3 m_DoorEdge;
    private bool m_IsOccupied;

    void Awake()
    {
        m_DoorEdge = transform.position + (-transform.up * GetComponent<Collider>().bounds.extents.y);
    }

    public bool Occupy()
    {
        if(m_IsOccupied)
            return false;
        m_IsOccupied = true;
        return true;
    }

    public void Leave()
    {
        m_IsOccupied = false;
    }
}
