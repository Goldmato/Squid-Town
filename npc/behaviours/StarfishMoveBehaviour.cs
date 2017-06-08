using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class StarfishMoveBehaviour : EnemyMoveBehaviour
{
    private Terrain m_Terrain;

    private float m_JailBreakChance;
    private bool m_TurnState;

    public StarfishMoveBehaviour(BaseEnemy enemy, float jailBreakChance = 0.05f) : base(enemy)
    {
        m_SkipUpdates = true;
        m_Terrain = Terrain.activeTerrain;
        m_JailBreakChance = jailBreakChance;
    }

    public override bool MoveNext()
    {
        // TODO: Change to actual starfish behaviour instead of placeholder
        if(Random.value < m_JailBreakChance)
        {
            BreakEnemyOut();
            return true;
        }

        SweepTerrain();
        return false;
    }

    public override void RunFromPlayer(float distance = 10)
    {
        var player = GameController.Current.Player;
        m_TurnState = !m_TurnState;
        float zigZagAngle = 45 * (m_TurnState ? 1 : -1);

        Vector3 direction = (m_Enemy.transform.position - player.transform.position).normalized;
        direction = Quaternion.Euler(0, zigZagAngle, 0) * direction;
        Vector3 targetPos = m_Enemy.transform.position + (direction * distance);

        m_Enemy.Agent.destination = targetPos;
    }

    protected void SweepTerrain()
    {
        float posX = m_Terrain.terrainData.size.x + m_Terrain.transform.position.x;
        float posZ = m_Terrain.terrainData.size.z + m_Terrain.transform.position.z;
        
        if(Random.value > 0.5)
            posX = Random.Range(m_Terrain.transform.position.x, posX);
        if(Random.value > 0.5)
            posZ = Random.Range(m_Terrain.transform.position.z, posZ);

        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(posX, 0, posZ), out hit,
            m_Terrain.terrainData.size.x, NavMesh.AllAreas);

        m_Enemy.Agent.destination = hit.position;
    }

    protected void BreakEnemyOut()
    {
        throw new System.NotImplementedException();
    }
}
