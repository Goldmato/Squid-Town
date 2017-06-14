using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StarfishEnemy : SuperEnemy 
{
    // TODO: Zigzag running behaviour
    // TODO: Breaking enemies out of jail periodically
    void Start()
    {
        m_MoveSpeed = 10;
        m_RunSpeed = 20;
        SetRunState(false);
    }

    protected override void MoveInitialize()
    {
        if(MoveMode == null)
            MoveMode = new StarfishMoveBehaviour(this);

        m_Agent.isStopped = false;
    }
}
