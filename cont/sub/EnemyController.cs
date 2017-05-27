using System.Collections;
using System.Collections.Generic;

using UnityEngine;

///<summary>
/// Static master class controlling all overhead enemy behaviour
///</summary>
public class EnemyController : MonoBehaviour
{
    public static EnemyController Current { get { return m_Instance; } }

    private List<Enemy> m_Enemies = new List<Enemy>();

    private static EnemyController m_Instance;
    private static bool m_ExceptionFlag;

    public void Register(Enemy enemy)
    {
        m_Enemies.Add(enemy);
    }

    public void MoveAllEnemies()
    {
        for(int i = 0; i < m_Enemies.Count; i++)
        {
            m_Enemies[i].Move();
        }
    }
}
