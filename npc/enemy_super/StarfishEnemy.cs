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

    [SerializeField] [Range(1, 100)] protected float m_WalkSpeedLow = 5f;
    [SerializeField] [Range(1, 100)] protected float m_WalkSpeedHigh = 10f;

    protected float m_WalkSpeed;
    protected float m_BreakoutTimer;
    protected bool m_BreakoutFlag;

    const int BREAKOUT_DELAY = 20;

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
        SetEnemyDisabled(!state);

        m_Agent.isStopped = state;
        if(state)
        {
            m_Animator.SetBool("starfish_smash", false);
            m_Animator.SetTrigger("starfish_spin");
        }
        else
        {
            m_Animator.SetBool("starfish_smash", true);
        }
    }

    public override void TeleportToJail()
    {
        AlertBuilder.StarfishEscapeWarningAlert(BREAKOUT_DELAY);

        m_Agent.Warp(GameController.Current.Jail.RandomLocation());
        m_BreakoutFlag = true;
        m_BreakoutTimer = Time.timeSinceLevelLoad + BREAKOUT_DELAY;
    }
}
