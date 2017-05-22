using System.Collections;
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

    [SerializeField] [Range(0, 10f)] protected float m_SpeedLow = 2f;
    [SerializeField] [Range(0, 10f)] protected float m_SpeedHigh = 8f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayLow = 1f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayHigh = 5f;

    protected int[] m_DoorsToIgnore = { -1, -1 };
    protected Renderer[] m_Renderer;
    protected NavMeshAgent m_Agent;
    protected Collider m_Collider;

    private int m_OldDoorIndex = int.MaxValue;
    private int m_DoorIgnoreIndex;

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Collider = GetComponent<Collider>();
        m_Renderer = GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        m_Agent.speed = Random.Range(m_SpeedLow, m_SpeedHigh) * 2.5f;
        GoToNearestDoor();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            Debug.Log("Enemy entered doorway!");
            StartCoroutine(DisableEnemy());
        }
    }

    protected virtual IEnumerator DisableEnemy()
    {
        m_Collider.enabled = false;
        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = false; }

        var hideDelay = Random.Range(m_HideDelayLow, m_HideDelayHigh);
        yield return new WaitForSeconds(hideDelay);

        m_Collider.enabled = true;
        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = true; }
        GoToNearestDoor();
    }

    protected virtual void GoToNearestDoor()
    {
        var doors = Registry.Current.Doors;

        float minDist = float.MaxValue;
        int selectedDoor = -1;

        for(int i = 0; i < doors.Count; i++)
        {
            if(IsDoorIgnored(i))
                continue;

            float dist = Vector3.Distance(transform.position, doors[i].transform.position);

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
            m_Agent.destination = doors[selectedDoor].transform.position;
        else
            throw new UnityException("ERROR No door selected");
    }

    bool IsDoorIgnored(int index)
    {
        for (int i = 0; i < m_DoorsToIgnore.Length; i++)
        {
            if(m_DoorsToIgnore[i] == index)
                return true;
        }
        return false;
    }
}
