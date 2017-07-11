using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Base enemy behaviour
///</summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class BaseEnemy : MonoBehaviour
{
    [SerializeField] [Range(0, 100f)] protected float m_SpeedLow = 3f;
    [SerializeField] [Range(0, 100f)] protected float m_SpeedHigh = 5f;
    [SerializeField] [Range(10f, 100f)] protected float m_RunDistanceLow = 10f;
    [SerializeField] [Range(10f, 100f)] protected float m_RunDistanceHigh = 20f;
    [SerializeField] [Range(10f, 200f)] protected float m_RunSpeedLow = 15f;
    [SerializeField] [Range(10f, 200f)] protected float m_RunSpeedHigh = 18f;

    protected EnemyMoveBehaviour m_MoveMode;

    protected Renderer[] m_Renderer;
    protected NavMeshAgent m_Agent;
    protected Animator m_Animator;
    protected Collider m_Collider;

    protected float m_DisableDelay;
    protected float m_MoveSpeed;
    protected float m_RunSpeed;
    protected bool m_EnemyStopped;
    protected bool m_EnemyDisabled;
    protected bool m_EnemyRunState;
    protected int m_DoorIgnoreIndex;

    public NavMeshAgent Agent { get { return m_Agent; } }
    public Animator Animator { get { return m_Animator; } }
    public bool SkipUpdates
    {
        get
        {
            if(MoveMode != null)
                return MoveMode.SkipUpdates & !m_EnemyStopped & !m_EnemyRunState;
            else
                return false;
        }
    }
    public bool Disabled { get { return m_EnemyDisabled; } }
    public bool Stopped { get { return m_EnemyStopped; } }

    public float MoveSpeed
    {
        get
        {
            if(m_MoveSpeed == 0)
                m_MoveSpeed = Random.Range(m_SpeedLow, m_SpeedHigh);
            return m_MoveSpeed;
        }
    }

    public float RunSpeed
    {
        get
        {
            if(m_RunSpeed == 0)
                m_RunSpeed = Random.Range(m_RunSpeedLow, m_RunSpeedHigh);
            return m_RunSpeed;
        }
    }

    public virtual bool GetRunState()
    {
        return m_EnemyRunState;
    }

    public virtual void SetRunState(bool value)
    {
        m_EnemyRunState = value;
        if(value)
            m_Agent.speed = RunSpeed;
        else
            m_Agent.speed = MoveSpeed;
    }

    protected virtual EnemyMoveBehaviour MoveMode
    {
        get
        {
            if(m_MoveMode == null)
                m_MoveMode = new RandomMovement(this);
            return m_MoveMode;
        }
    }

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponentsInChildren<Renderer>();

        GameController.Current.EC.Register(this);
    }

    void Update()
    {
        if(!m_Agent.pathPending && !m_EnemyDisabled)
        {
            if(m_Agent.remainingDistance <= m_Agent.stoppingDistance)
            {
                if(!m_Agent.hasPath || m_Agent.velocity.sqrMagnitude == 0f)
                {
                    m_EnemyStopped = !m_EnemyRunState;
                }
            }
        }
    }

    public virtual void RunFromPlayer()
    {
        if(m_EnemyDisabled)
            return;
        m_Agent.isStopped = false;

        if(!m_EnemyStopped)
            MoveMode.RunFromPlayer(Random.Range(m_RunDistanceLow, m_RunDistanceHigh));
        else
            GoRightOrLeft();
    }

    public virtual void MoveUpdate()
    {
        if(m_EnemyDisabled)
            return;
        m_Agent.isStopped = false;

        MoveMode.MoveNext();
    }

    public virtual void GoRightOrLeft(bool? rightOrLeft = null)
    {
        if(m_EnemyDisabled)
            return;
        m_Agent.isStopped = false;

        if(rightOrLeft == null)
            rightOrLeft = Random.value > 0.5 ? true : false;

        MoveMode.GoRightOrLeft((bool)rightOrLeft, Random.Range(m_RunDistanceLow, m_RunDistanceHigh));
    }

    public virtual void SetMoveMode(BehaviourType? moveType)
    {
        switch(moveType)
        {
            case BehaviourType.RandomMovement:
                m_MoveMode = new RandomMovement(this);
                break;
            case BehaviourType.SeekDoors:
                m_MoveMode = new SeekDoors(this);
                break;
            default:
                goto case BehaviourType.RandomMovement;
        }
    }

    public virtual void TeleportToJail()
    {
        var teleportLocation = GameController.Current.Jail.RandomLocation();
        gameObject.transform.position = teleportLocation;
    }

    public virtual void Enabled(bool state)
    {
        SetEnemyDisabled(!state);
        m_Agent.enabled = state;
        m_Animator.enabled = state;
    }

    protected virtual void SetEnemyDisabled(bool state) { m_EnemyDisabled = state; }
}
