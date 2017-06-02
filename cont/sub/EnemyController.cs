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
    
    public void StartEnemyUpdateCycle()
    {
        StartCoroutine(UpdateEnemies());
    }

    public void Register(Enemy enemy)
    {
        m_Enemies.Add(enemy);
        m_SkippedUpdates.Add(0);
    }

    public void MoveEnemy(int index)
    {
        if(!m_Enemies[index].Disabled && (!m_Enemies[index].Agent.hasPath || m_SkippedUpdates[index] >= MAX_SKIPPED_UPDATES || m_Enemies[index].RunState))
        {
            // Debug.Log("Enemy [" + i + "] moved after [" + m_SkippedUpdates[i] + "] skipped update cycles");
            if(m_Enemies[index].RunState)
                m_Enemies[index].RunFromPlayer();
            else
                m_Enemies[index].MoveUpdate();

            m_SkippedUpdates[index] = 0;
        }
        else
        {
            m_SkippedUpdates[index]++;
        }
    }

    IEnumerator UpdateEnemies()
    {
        m_UpdateEnemies = true;
        while(m_UpdateEnemies)
        {
            for (int i = 0; i < EnemyCount; i++)
            {
                MoveEnemy(i);

                for(int j = 0; j < m_IntervalFrames / EnemyCount; j++)
                {
                    yield return null;
                }
            }
        }
    }
}
