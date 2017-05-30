﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Base enemy behaviour
///</summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    public NavMeshAgent Agent { get { return m_Agent; } }

	public GameObject m_jail;

    [SerializeField] [Range(0, 10f)] protected float m_SpeedLow = 2f;
    [SerializeField] [Range(0, 10f)] protected float m_SpeedHigh = 8f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayLow = 1f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayHigh = 5f;

    protected MoveBehaviour MoveMode;

    protected Renderer[] m_Renderer;
    protected NavMeshAgent m_Agent;
    protected Collider m_Collider;

    private float m_DisableDelay;
    private int m_DoorIgnoreIndex;

    const float DISABLE_INTERVAL = 1.5f;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponentsInChildren<Renderer>();
        m_Agent.speed = Random.Range(m_SpeedLow, m_SpeedHigh);
		m_jail = GameObject.FindGameObjectWithTag ("Jail");

        GameController.Current.EC.Register(this);
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

    protected IEnumerator DisableEnemy(bool moveAfter = true)
    {
        m_Agent.isStopped = true;
        m_Agent.Warp(m_Agent.destination);
        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = false; }

        var hideDelay = Random.Range(m_HideDelayLow, m_HideDelayHigh);
        yield return new WaitForSeconds(hideDelay);

        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = true; }

        if(moveAfter)
            Move();
    }

    public void Move()
    {
        if(MoveMode == null)
            MoveMode = new SeekDoors();

        m_Agent.isStopped = false;

        MoveMode.MoveNext(m_Agent, transform.position);
    }

	public void TeleportToJail ()
	{
		var teleportLocation = GameObject.FindGameObjectWithTag ("InJail").GetComponent<InJail> ().RandomLocation ();
		gameObject.transform.position = teleportLocation;
	}

	public void DisableNavMeshAgent ()
	{
		gameObject.GetComponent<NavMeshAgent> ().enabled = false;
	}

	public void DisableAnimator ()
	{
		gameObject.GetComponent<Animator> ().enabled = false;
	}
}
