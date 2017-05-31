using System.Collections;
using System.Collections.Generic;

using UnityEngine;

///<summary>
/// Static master class controlling all overhead enemy behaviour
///</summary>
public class EnemyController : MonoBehaviour
{
    public static EnemyController Current { get { return m_Instance; } }
    
    public bool UpdateRunning { get { return m_UpdateEnemies; } set { m_UpdateEnemies = value; } }
    public int EnemyCount { get { return m_Enemies.Count; } }

    private List<Enemy> m_Enemies = new List<Enemy>();
    private List<byte> m_SkippedUpdates = new List<byte>();

    private static EnemyController m_Instance;

    private bool m_UpdateEnemies;
    private int m_IntervalFrames = 30;

    const byte MAX_SKIPPED_UPDATES = 25;

    void Update()
    {
        // DEBUGGING/TESTING
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            MoveAllEnemies();
        }
    }

    public void StartEnemyUpdateCycle()
    {
        StartCoroutine(UpdateEnemies());
    }

    public void Register(Enemy enemy)
    {
        m_Enemies.Add(enemy);
        m_SkippedUpdates.Add(0);
    }

    public void MoveAllEnemies()
    {
        int numEnemiesMoved = 0;

        for(int i = 0; i < m_Enemies.Count; i++)
        {
            if(!m_Enemies[i].Disabled && (!m_Enemies[i].Agent.hasPath || m_SkippedUpdates[i] >= MAX_SKIPPED_UPDATES || m_Enemies[i].RunState))
            {
                // Debug.Log("Enemy [" + i + "] moved after [" + m_SkippedUpdates[i] + "] skipped update cycles");

                if(m_Enemies[i].RunState)
                    m_Enemies[i].RunFromPlayer();
                else
                    m_Enemies[i].MoveUpdate();

                m_SkippedUpdates[i] = 0;
                numEnemiesMoved++;
            }
            else
            {
                m_SkippedUpdates[i]++;
            }
        }

        // Debug.Log(numEnemiesMoved + " Enemies moved during update");
    }

    IEnumerator UpdateEnemies()
    {
        m_UpdateEnemies = true;
        while(m_UpdateEnemies)
        {
            MoveAllEnemies();

            for(int i = 0; i < m_IntervalFrames; i++)
            {
                yield return null;
            }
        }
    }
}
