using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StarfishEnemy : SuperEnemy
{
    // TODO: Zigzag running behaviour
    // TODO: Breaking enemies out of jail periodically
    public float WalkSpeed
    {
        get
        {
            if(m_WalkSpeed == 0)
                m_WalkSpeed = Random.Range(m_WalkSpeedLow, m_WalkSpeedHigh);
            return m_WalkSpeed;
        }
    }

    [SerializeField] protected float m_WalkSpeedLow = 5f;
    [SerializeField] protected float m_WalkSpeedHigh = 10f;

    protected float m_WalkSpeed;
    protected float m_BreakoutTimer;
    protected bool m_BreakoutFlag;

    const float BREAKOUT_DELAY = 20f;

    void Start()
    {
        m_MoveMode = new StarfishMoveBehaviour(this);
        SetRunState(false);
    }

    void Update()
    {
        if(m_BreakoutFlag)
        {
            if(Time.timeSinceLevelLoad > m_BreakoutTimer)
            {
                m_BreakoutFlag = false;
                GameController.Current.EC.ReleaseEnemy(this);
            }
        }
    }

    public override void Enabled(bool state)
    {
        m_EnemyDisabled = !state;

        m_Agent.isStopped = state;
        if(state)
            m_Animator.SetTrigger("starfish_spin");
        else
            m_Animator.SetTrigger("starfish_smash");
    }

    public override void TeleportToJail()
    {
        m_Agent.Warp(GameController.Current.Jail.RandomLocation());
        m_BreakoutFlag = true;
        m_BreakoutTimer = Time.timeSinceLevelLoad + BREAKOUT_DELAY;
    }
}
