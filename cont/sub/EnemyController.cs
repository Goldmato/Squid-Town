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

    private List<Enemy> m_Enemies = new List<Enemy>();

    private static EnemyController m_Instance;
    private static bool m_ExceptionFlag;

    private bool m_UpdateEnemies;
    private int m_IntervalFrames = 120;

    void Update()
    {
        // DEBUGGING/TESTING
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            MoveAllEnemies();
        }
    }

    public void Register(Enemy enemy)
    {
        m_Enemies.Add(enemy);
    }

    public void MoveAllEnemies()
    {
        int numEnemiesMoved = 0;

        for(int i = 0; i < m_Enemies.Count; i++)
        {
            if(!m_Enemies[i].Agent.hasPath)
            {
                m_Enemies[i].Move();
                numEnemiesMoved++;
            }
        }

        Debug.Log(numEnemiesMoved + " Enemies moved during update");
    }

    public IEnumerator UpdateEnemies()
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
