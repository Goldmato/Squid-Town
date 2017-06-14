﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Static master class controlling all overhead enemy behaviour
///</summary>
public class EnemyController : MonoBehaviour
{
    public static EnemyController Current { get { return m_Instance; } }
    
    public bool UpdateRunning { get { return m_UpdateEnemies; } set { m_UpdateEnemies = value; } }
    public int EnemyCount { get { return m_Enemies.Count; }  }

    private List<BaseEnemy> m_Enemies = new List<BaseEnemy>();
    private List<BaseEnemy> m_JailEnemies = new List<BaseEnemy>();
    private List<byte> m_SkippedUpdates = new List<byte>();

    private static EnemyController m_Instance;

    private bool m_UpdateEnemies;
    private int m_IntervalFrames = 30;

    const byte MAX_SKIPPED_UPDATES = 25;
    
    public void StartEnemyUpdateCycle()
    {
        StartCoroutine(UpdateEnemies());
    }

    public void Register(BaseEnemy enemy)
    {
        m_Enemies.Add(enemy);
        m_SkippedUpdates.Add(0);
    }

    public void SendEnemyToJail(BaseEnemy enemy)
    {
        m_JailEnemies.Add(enemy);

        enemy.Enabled(false);
        enemy.TeleportToJail();
    }

    public void ReleaseRandomEnemy()
    {
        if(m_JailEnemies.Count <= 0)
            throw new UnityException("No enemies to release from jail");

        int randEnemy = Random.Range(0, m_JailEnemies.Count);

        m_JailEnemies[randEnemy].Enabled(true);
        m_JailEnemies.RemoveAt(randEnemy);

        GameController.Current.Score--;
    }

    void MoveEnemy(int index)
    {
        if(m_Enemies[index].Disabled || m_Enemies[index].SkipUpdates)
            return;
        if(!m_Enemies[index].Agent.hasPath || m_SkippedUpdates[index] >= MAX_SKIPPED_UPDATES || m_Enemies[index].GetRunState())
        {
            // Debug.Log("Enemy [" + i + "] moved after [" + m_SkippedUpdates[i] + "] skipped update cycles");
            if(m_Enemies[index].GetRunState())
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
