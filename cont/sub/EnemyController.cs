using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Static master class controlling all overhead enemy behaviour
///</summary>
public class EnemyController : MonoBehaviour
{
    public bool UpdateRunning { get { return m_UpdateEnemies; } set { m_UpdateEnemies = value; } }
    public int FreeEnemyCount { get { return m_FreeEnemies.Count; } }
    public int JailEnemyCount { get { return m_JailEnemies.Count; } }

    private List<BaseEnemy> m_FreeEnemies = new List<BaseEnemy>();
    private List<BaseEnemy> m_JailEnemies = new List<BaseEnemy>();
    private List<byte> m_SkippedUpdates = new List<byte>();

    private bool m_UpdateEnemies;
    private int m_IntervalFrames = 30;

    const byte MAX_SKIPPED_UPDATES = 25;

    // DEBUGGING/TESTING
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SendEnemyToJail(m_FreeEnemies.Find(x => x.GetType() == typeof(StarfishEnemy)));
        }
    }

    public void StartEnemyUpdateCycle()
    {
        StartCoroutine(UpdateEnemies());
    }

    public void Register(BaseEnemy enemy, bool startInJail = false)
    {
        if(startInJail)
            m_JailEnemies.Add(enemy);
        else
            m_FreeEnemies.Add(enemy);

        m_SkippedUpdates.Add(0);
    }

    public void SendEnemyToJail(BaseEnemy enemy)
    {
        m_FreeEnemies.Remove(enemy);
        m_JailEnemies.Add(enemy);

        enemy.Enabled(false);
        enemy.TeleportToJail();
    }

    public void ReleaseEnemy(BaseEnemy enemy)
    {
        if(m_JailEnemies.Count <= 0)
            throw new UnityException("No enemies to release from jail");

        AlertBuilder.EnemyEscapedAlert(enemy.GetType().ToString());

        m_FreeEnemies.Add(enemy);
        m_JailEnemies.Find(x => x.Equals(enemy)).Enabled(true);
        m_JailEnemies.Remove(enemy);

        GameController.Current.UpdateScore();
    }

    public void ReleaseRandomEnemy()
    {
        if(m_JailEnemies.Count <= 0)
            throw new UnityException("No enemies to release from jail");

        int randEnemy = Random.Range(0, m_JailEnemies.Count);
        AlertBuilder.EnemyEscapedAlert(m_JailEnemies[randEnemy].GetType().ToString());

        m_FreeEnemies.Add(m_JailEnemies[randEnemy]);
        m_JailEnemies[randEnemy].Enabled(true);
        m_JailEnemies.RemoveAt(randEnemy);

        GameController.Current.UpdateScore();
    }

    void MoveEnemy(int index)
    {
        if(m_FreeEnemies[index].Disabled || m_FreeEnemies[index].SkipUpdates)
            return;
        if(!m_FreeEnemies[index].Agent.hasPath || m_SkippedUpdates[index] >= MAX_SKIPPED_UPDATES || m_FreeEnemies[index].GetRunState())
        {
            // Debug.Log("Enemy [" + i + "] moved after [" + m_SkippedUpdates[i] + "] skipped update cycles");
            if(m_FreeEnemies[index].GetRunState())
                m_FreeEnemies[index].RunFromPlayer();
            else
                m_FreeEnemies[index].MoveUpdate();

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
            for(int i = 0; i < FreeEnemyCount; i++)
            {
                MoveEnemy(i);

                for(int j = 0; j < m_IntervalFrames / FreeEnemyCount; j++)
                {
                    yield return null;
                }
            }
        }
    }
}
