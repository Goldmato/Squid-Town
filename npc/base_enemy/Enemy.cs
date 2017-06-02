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
public class Enemy : MonoBehaviour
{
    public NavMeshAgent Agent { get { return m_Agent; } }
    public bool Disabled { get { return m_EnemyDisabled; } }
    public bool Stopped { get { return m_EnemyStopped; } }
    public bool RunState
    {
        get { return m_EnemyRunState; }
        set
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

            // Debug.Log("Speed increased by: " + (m_Agent.speed - m_MoveSpeed));
        }
    }

    [SerializeField] [Range(0, 10f)] protected float m_SpeedLow = 3f;
    [SerializeField] [Range(0, 10f)] protected float m_SpeedHigh = 5f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunSpeedLow = 15f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunSpeedHigh = 18f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunDistanceLow = 10f;
    [SerializeField] [Range(10f, 20f)] protected float m_RunDistanceHigh = 20f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayLow = 1f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayHigh = 5f;

    protected EnemyMoveBehaviour MoveMode;

    protected Renderer[] m_Renderer;
    protected NavMeshAgent m_Agent;
    protected Animator m_Animator;
    protected Collider m_Collider;

    private static InJail m_InJail;

    private float m_DisableDelay;
    private float m_MoveSpeed;
    private float m_RunSpeed;
    private bool m_EnemyStopped;
    private bool m_EnemyDisabled;
    private bool m_EnemyRunState;
    private int m_DoorIgnoreIndex;

    const float DISABLE_INTERVAL = 5f;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponentsInChildren<Renderer>();

        m_MoveSpeed = Random.Range(m_SpeedLow, m_SpeedHigh);
        m_RunSpeed = Random.Range(m_RunSpeedLow, m_RunSpeedHigh);

        RunState = false;

        GameController.Current.EC.Register(this);

        if(m_InJail == null)
            m_InJail = GameObject.FindGameObjectWithTag("InJail").GetComponent<InJail>();
    }
    
    void Update()
    {
        m_EnemyStopped = m_Agent.velocity.sqrMagnitude <= 0f & m_EnemyRunState;
    }

    void OnTriggerEnter(Collider other)
    {
        if(Time.timeSinceLevelLoad > m_DisableDelay && other.CompareTag("Door"))
        {
            Debug.Log("Enemy entered doorway!");

            // Cooldown delay for door trigger events
            m_DisableDelay = Time.timeSinceLevelLoad + DISABLE_INTERVAL;
            StartCoroutine(DisableEnemy());
        }
    }

    protected IEnumerator DisableEnemy()
    {
        m_EnemyDisabled = true;
        m_Agent.isStopped = true;
        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = false; }

        var hideDelay = Random.Range(m_HideDelayLow, m_HideDelayHigh);
        yield return new WaitForSeconds(hideDelay);

        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = true; }

        m_EnemyDisabled = false;

        MoveUpdate();
    }

    protected void MoveInitialize()
    {
        if(MoveMode == null)
            MoveMode = new RandomMovement(this);

        m_Agent.isStopped = false;
    }

    public void RunFromPlayer()
    {
        if(m_EnemyDisabled)
            return;
        MoveInitialize();

        if(!m_EnemyStopped)
            MoveMode.RunFromPlayer(Random.Range(m_RunDistanceLow, m_RunDistanceHigh));
        else
            GoRightOrLeft();
    }

    public void MoveUpdate()
    {
        if(m_EnemyDisabled)
            return;
        MoveInitialize();

        bool moveSuccesful = MoveMode.MoveNext();
        // Debug.Log("Enemy move successful: " + moveSuccesful);
    }

    public void GoRightOrLeft(bool ?rightOrLeft = null)
    {
        bool direction;

        if(rightOrLeft != null)
            direction = (bool)rightOrLeft;
        else
            direction = Random.value > 0.5 ? true : false;

        if(m_EnemyDisabled)
            return;
        MoveInitialize();

        MoveMode.GoRightOrLeft(direction, Random.Range(m_RunDistanceLow, m_RunDistanceHigh));
    }

    public void SetMoveMode(BehaviourType ?moveType)
    {
        switch(moveType)
        {
            case BehaviourType.RandomMovement:
                MoveMode = new RandomMovement(this);
                break;
            case BehaviourType.SeekDoors:
                MoveMode = new SeekDoors(this);
                break;
            default:
                goto case BehaviourType.RandomMovement;
        }
    }

    public void TeleportToJail()
    {
        m_EnemyDisabled = true;
        var teleportLocation = m_InJail.RandomLocation();
        gameObject.transform.position = teleportLocation;
    }

    public void DisableNavMeshAgent() { m_Agent.enabled = false; }

    public void DisableAnimator() { m_Animator.enabled = false; }
}
