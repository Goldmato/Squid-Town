using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class StarfishMoveBehaviour : EnemyMoveBehaviour
{
    public bool BreakoutState { get { return m_BreakoutState; } }

    private Terrain m_Terrain;

    private float m_JailBreakChance;
    private float m_CornerExtent;
    private float m_BreakoutTimer;
    private bool m_TurnState;
    private bool m_BreakoutState;
    private bool m_TimerFlag;

    const float BREAKOUT_INTERVAL = 3f;

    public StarfishMoveBehaviour(BaseEnemy enemy, float jailBreakChance = 0.25f, 
        float cornerExtent = 50f) : base(enemy)
    {
        m_Terrain = Terrain.activeTerrain;
        m_JailBreakChance = jailBreakChance;
        m_CornerExtent = cornerExtent;
    }

    public override bool MoveNext()
    {
        if(m_BreakoutState)
        {
            Vector3 jailFront = GameController.Current.Jail.Front(m_Enemy.Agent.radius);

            m_Enemy.Agent.destination = jailFront;
            if(Vector3.Distance(m_Enemy.transform.position, jailFront) > m_Enemy.Agent.radius * 5)
                return false;
            
            if(m_TimerFlag)
            {
                m_TimerFlag = false;
                m_BreakoutTimer = Time.timeSinceLevelLoad + BREAKOUT_INTERVAL;
            }

            if(Time.timeSinceLevelLoad > m_BreakoutTimer)
            {
                ReleaseEnemy();
                m_SkipUpdates = false;
                m_BreakoutState = false;
                return true;
            }

            return false;
        }
        if(Random.value < m_JailBreakChance && GameController.Current.Score > 0)
        {
            Debug.Log("Starfish going to break an enemy out of jail");
            BreakEnemyOut();
            return false;
        }

        Debug.Log("Starfish going to random map corner");
        GoToRandomCorner();
        return false;
    }

    public override void RunFromPlayer(float distance = 10)
    {
        var player = GameController.Current.Player;
        // m_TurnState = !m_TurnState;
        // float zigZagAngle = 45 * (m_TurnState ? 1 : -1);

        //FIXME: Only zigzag when not close to a wall or obstacle
        Vector3 direction = (m_Enemy.transform.position - player.transform.position).normalized;
        // direction = Quaternion.Euler(0, zigZagAngle, 0) * direction;
        Vector3 targetPos = m_Enemy.transform.position + (direction * distance);

        m_Enemy.Agent.destination = targetPos;
    }

    protected void GoToRandomCorner()
    {
        float posX;
        float posZ;
        float randX = Random.Range(0, m_CornerExtent);
        float randZ = Random.Range(0, m_CornerExtent);

        // TODO: Refactor/Optimize
        float randValue = Random.value;
        if(randValue > 0.75f)
        {
            posX = m_Terrain.transform.position.x + randX;
            posZ = m_Terrain.transform.position.z + randZ;
        }
        else if(randValue > 0.5f)
        {
            posX = m_Terrain.transform.position.x + m_Terrain.terrainData.size.x - randX;
            posZ = m_Terrain.transform.position.z + randZ;
        }
        else if(randValue > 0.25f)
        {
            posX = m_Terrain.transform.position.x + randX;
            posZ = m_Terrain.transform.position.z + m_Terrain.terrainData.size.z - randZ;
        }
        else
        {
            posX = m_Terrain.transform.position.x + m_Terrain.terrainData.size.x - randX;
            posZ = m_Terrain.transform.position.z + m_Terrain.terrainData.size.z - randZ;
        }

        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(posX, 0, posZ), out hit,
            m_Terrain.terrainData.size.x, NavMesh.AllAreas);

        m_Enemy.Agent.destination = hit.position;
    }

    protected void BreakEnemyOut()
    {
        var jail = GameController.Current.Jail;

        m_SkipUpdates = true;
        m_TimerFlag = true;
        m_BreakoutState = true;
    }

    protected void ReleaseEnemy()
    {
        GameController.Current.EC.ReleaseRandomEnemy();
    }
}
