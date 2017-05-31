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

    [SerializeField] [Range(0, 10f)] protected float m_SpeedLow = 2f;
    [SerializeField] [Range(0, 10f)] protected float m_SpeedHigh = 8f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayLow = 1f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayHigh = 5f;

    protected EnemyMoveBehaviour MoveMode;

    protected Renderer[] m_Renderer;
    protected NavMeshAgent m_Agent;
    protected Animator m_Animator;
    protected Collider m_Collider;

    private static InJail m_InJail;

    private float m_DisableDelay;
    private bool m_EnemyDisabled;
    private int m_DoorIgnoreIndex;

    const float DISABLE_INTERVAL = 5f;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponentsInChildren<Renderer>();
        m_Agent.speed = Random.Range(m_SpeedLow, m_SpeedHigh);

        GameController.Current.EC.Register(this);

        if(m_InJail == null)
            m_InJail = GameObject.FindGameObjectWithTag("InJail").GetComponent<InJail>();
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

        Move();
    }

    public void Move()
    {
        if(m_EnemyDisabled)
            return;
        if(MoveMode == null)
            MoveMode = new RandomMovement();

        m_Agent.isStopped = false;

        MoveMode.MoveNext(m_Agent, transform.position);
    }

    public void TeleportToJail()
    {
        m_EnemyDisabled = true;
        var teleportLocation = m_InJail.RandomLocation();
        gameObject.transform.position = teleportLocation;
    }

    public void DisableNavMeshAgent()
    {
        m_Agent.enabled = false;
    }

    public void DisableAnimator()
    {
        m_Animator.enabled = false;
    }
}
