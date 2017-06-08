using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Enemy movement behaviour that targets the nearest
/// door while ignoring the previous 2 doors visited
/// while using this behaviour
///</summary>
public class SeekDoors : EnemyMoveBehaviour
{
    private int[] m_DoorsToIgnore = { -1, -1 };
    private int m_DoorIgnoreIndex;

    public SeekDoors(BaseEnemy enemy) : base (enemy) 
    {
    }

    public override bool MoveNext()
    {
        var doors = GameController.Current.Doors;

        float minDist = float.MaxValue;
        int selectedDoor = -1;

        for(int i = 0; i < doors.Count; i++)
        {
            if(IsDoorIgnored(i))
                continue;

            float dist = Vector3.Distance(m_Enemy.transform.position, doors[i].transform.position);

            if(dist < minDist)
            {
                selectedDoor = i;
                minDist = dist;
            }
        }

        m_DoorsToIgnore[m_DoorIgnoreIndex++] = selectedDoor;
        if(m_DoorIgnoreIndex == m_DoorsToIgnore.Length)
            m_DoorIgnoreIndex = 0;

        if(selectedDoor >= 0)
            m_Enemy.Agent.destination = doors[selectedDoor].Edge;
        else
            return false;
        return true;
    }

    bool IsDoorIgnored(int index)
    {
        for(int i = 0; i < m_DoorsToIgnore.Length; i++)
        {
            if(m_DoorsToIgnore[i] == index)
                return true;
        }
        return false;
    }
}
