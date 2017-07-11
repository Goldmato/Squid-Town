using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class StarfishMoveBehaviour : EnemyMoveBehaviour
{
    protected override BaseEnemy Enemy { get { return m_StarEnemy; } set { m_StarEnemy = (StarfishEnemy)value; } }

    protected StarfishEnemy m_StarEnemy;

    public bool BreakoutState { get { return m_BreakoutState; } }

    private Terrain m_Terrain;

    private float m_JailBreakChance = BASE_BREAKOUT_CHANCE;
    private float m_CornerExtent;
    private float m_BreakoutTimer;
    private bool m_TurnState;
    private bool m_BreakoutState;
    private bool m_TimerFlag;

    const float BREAKOUT_INTERVAL = 3f;
    const float ENTROPY_INTERVAL = 0.05f;
    const float BASE_BREAKOUT_CHANCE = 0.25f;
    const float MAX_BREAKOUT_CHANCE = 0.75f;

    public StarfishMoveBehaviour(StarfishEnemy enemy, float jailBreakChance = 0.25f,
        float cornerExtent = 50f) : base(enemy)
    {
        Enemy = enemy;
        m_Terrain = Terrain.activeTerrain;
        m_JailBreakChance = jailBreakChance;
        m_CornerExtent = cornerExtent;
    }

    public override bool MoveNext()
    {
        if(m_BreakoutState)
        {
            Vector3 jailFront = GameController.Current.Jail.Front(Enemy.Agent.radius);

            Enemy.Agent.destination = jailFront;
            if(Vector3.Distance(Enemy.transform.position, jailFront) > Enemy.Agent.radius * 5)
                return false;

            if(m_TimerFlag)
            {
                Enemy.transform.LookAt(GameController.Current.Jail.transform.position);
                Enemy.Animator.SetBool("starfish_smash", true);
                m_TimerFlag = false;
                m_BreakoutTimer = Time.timeSinceLevelLoad + BREAKOUT_INTERVAL;
            }

            if(Time.timeSinceLevelLoad > m_BreakoutTimer)
            {
                ReleaseEnemy();
                EndBreakout();

                return true;
            }

            return false;
        }
        if(Random.value < m_JailBreakChance && GameController.Current.JailOccupied)
        {
            Debug.Log("Starfish going to break an enemy out of jail");
            BreakEnemyOut();

            return false;
        }
        else if(m_JailBreakChance < MAX_BREAKOUT_CHANCE && GameController.Current.JailOccupied)
        {
            m_JailBreakChance += ENTROPY_INTERVAL;
        }

        Debug.Log("Starfish going to random map corner");
        GoToRandomCorner();
        return false;
    }

    public override void RunFromPlayer(float distance = 25)
    {
        EndBreakout();

        base.RunFromPlayer();
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

        Enemy.Agent.destination = hit.position;
    }

    protected void BreakEnemyOut()
    {
        var jail = GameController.Current.Jail;

        Enemy.Animator.SetBool("starfish_smash", false);
        Enemy.Animator.SetTrigger("starfish_walk");
        Enemy.Agent.speed = m_StarEnemy.WalkSpeed;
        m_TimerFlag = true;
        m_BreakoutState = true;

        AlertBuilder.StarfishBreakoutAlert();
    }

    protected void ReleaseEnemy()
    {
        GameController.Current.EC.ReleaseRandomEnemy();
    }

    protected void EndBreakout()
    {
        Enemy.Animator.SetBool("starfish_smash", false);
        Enemy.Animator.SetTrigger("starfish_spin");
        m_JailBreakChance = BASE_BREAKOUT_CHANCE;
        Enemy.Agent.speed = Enemy.MoveSpeed;
        m_BreakoutState = false;
    }
}
