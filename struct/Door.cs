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
    private bool m_IsOccupied;

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
