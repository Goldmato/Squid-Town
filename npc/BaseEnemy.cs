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
    public NavMeshAgent Agent { get { return m_Agent; } }
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

    public virtual bool GetRunState()
    {
        return m_EnemyRunState;
    }

    public virtual void SetRunState(bool value)
    {
        m_EnemyRunState = value;
        if(value)
        {
            m_Agent.acceleration = m_RunSpeed * 2;
            m_Agent.speed = m_RunSpeed;
        }
        else
        {
            m_Agent.acceleration = m_MoveSpeed * 2;
            m_Agent.speed = m_MoveSpeed;
        }
    }

    [SerializeField] [Range(0, 10f)] protected float m_SpeedLow = 3f;
    [SerializeField] [Range(0, 10f)] protected float m_SpeedHigh = 5f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunSpeedLow = 15f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunSpeedHigh = 18f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunDistanceLow = 10f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunDistanceHigh = 20f;

    protected EnemyMoveBehaviour MoveMode;

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

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponentsInChildren<Renderer>();

        m_MoveSpeed = Random.Range(m_SpeedLow, m_SpeedHigh);
        m_RunSpeed = Random.Range(m_RunSpeedLow, m_RunSpeedHigh);

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
        else
        {
            m_EnemyStopped = false;
        }
    }

    protected virtual void MoveInitialize()
    {
        if(MoveMode == null)
            MoveMode = new RandomMovement(this);

        m_Agent.isStopped = false;
    }

    public virtual void RunFromPlayer()
    {
        if(m_EnemyDisabled)
            return;
        MoveInitialize();

        if(!m_EnemyStopped)
            MoveMode.RunFromPlayer(Random.Range(m_RunDistanceLow, m_RunDistanceHigh));
        else
            GoRightOrLeft();
    }

    public virtual void MoveUpdate()
    {
        if(m_EnemyDisabled)
            return;
        MoveInitialize();

        bool moveSuccesful = MoveMode.MoveNext();
        // Debug.Log("Enemy move successful: " + moveSuccesful);
    }

    public virtual void GoRightOrLeft(bool? rightOrLeft = null)
    {
        if(m_EnemyDisabled)
            return;
        MoveInitialize();

        if(rightOrLeft == null)
            rightOrLeft = Random.value > 0.5 ? true : false;

        MoveMode.GoRightOrLeft((bool)rightOrLeft, Random.Range(m_RunDistanceLow, m_RunDistanceHigh));
    }

    public virtual void SetMoveMode(BehaviourType? moveType)
    {
        switch(moveType)
        {
            case BehaviourType.RandomMovement:
                MoveMode = new RandomMovement(this);
                break;
            case BehaviourType.SeekDoors:
                MoveMode = new SeekDoors(this);
                break;
            case BehaviourType.Starfish:
                MoveMode = new StarfishMoveBehaviour(this);
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
        m_EnemyDisabled = !state;
        m_Agent.enabled = state;
        m_Animator.enabled = state;
    }
}
