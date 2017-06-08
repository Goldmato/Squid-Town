using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Component that controls all enemy spawning behaviour
/// NOTE: Not a singleton
///</summary>
public class EnemySpawner : MonoBehaviour
{
    // TODO: Implement features shown in https://www.draw.io/#DEnemyManager

    [SerializeField] private Terrain m_Terrain;
    [SerializeField] private SimpleEnemy m_SquidEnemy;
    [SerializeField] private SuperEnemy m_StarfishEnemy;

    private GameObject m_Container;
    private JailFloor m_InJail;

    void Start()
    {
        m_Container = new GameObject("Enemies");
        m_InJail = GameObject.FindGameObjectWithTag("InJail").GetComponent<JailFloor>();
    }

    public void SpawnEnemies(SpawnMethod method, EnemyType enemyType = EnemyType.Squid, BehaviourType ?moveType = null, int numEnemies = 10)
    {
        switch(method)
        {
            case SpawnMethod.Random:
                SpawnEnemiesRandom(moveType, enemyType, numEnemies);
                break;
            case SpawnMethod.InHouses:
                SpawnEnemiesInHouses(moveType, enemyType);
                break;
            case SpawnMethod.InJail:
                SpawnEnemiesInJail(moveType, enemyType, numEnemies);
                break;
            default:
                throw new UnityException("Invalid spawn method requested (EnemyManager)");
        }
    }

    void SpawnEnemiesRandom(BehaviourType? moveType, EnemyType enemyType, int numEnemies)
    {
        Debug.Log("Spawning [" + numEnemies + "] " + enemyType + " randomly");

        float terrainRadius = (m_Terrain.terrainData.size.x + m_Terrain.terrainData.size.z) / 2;

        for(int i = 0; i < numEnemies; i++)
        {
            NavMeshHit hit;
            Vector3 randomPos = new Vector3(Random.Range(m_Terrain.transform.position.x, m_Terrain.terrainData.size.x + m_Terrain.transform.position.x), 0,
                                            Random.Range(m_Terrain.transform.position.z, m_Terrain.transform.position.z + m_Terrain.terrainData.size.z));
            NavMesh.SamplePosition(randomPos, out hit, terrainRadius, NavMesh.AllAreas);

            var newEnemy = SpawnEnemy(enemyType, hit.position, Quaternion.identity);
            newEnemy.SetMoveMode(moveType);
            OrganizeEnemy(newEnemy.transform);

            // Debug.Log("Moving enemy [" + i + "] to nearest door...");
        }
    }

    void SpawnEnemiesInHouses(BehaviourType ?moveType, EnemyType enemyType)
    {
        Debug.Log("Spawning [" + GameController.Current.Doors.Count + "] " + enemyType + " in houses");

        for(int i = 0; i < GameController.Current.Doors.Count; i++)
        {
            var newEnemy = SpawnEnemy(enemyType, GameController.Current.Doors[i].Edge, Quaternion.identity);
            newEnemy.SetMoveMode(moveType);
            OrganizeEnemy(newEnemy.transform);
        }
    }

    void SpawnEnemiesInJail(BehaviourType ?moveType, EnemyType enemyType, int numEnemies)
    {
        Debug.Log("Spawning [" + numEnemies + "] " + enemyType + " in jail");

        for (int i = 0; i < numEnemies; i++)
        {
            //TODO: Spawn enemies in a circular area within the main jail
            Vector3 randomPos = m_InJail.RandomLocation();

            var newEnemy = SpawnEnemy(enemyType, randomPos, Quaternion.identity);
            newEnemy.SetMoveMode(moveType);
            OrganizeEnemy(newEnemy.transform);
        }
    }

    BaseEnemy SpawnEnemy(EnemyType type, Vector3 pos, Quaternion rot)
    {
        BaseEnemy enemyToSpawn;

        switch(type)
        {
            case EnemyType.Squid:
                enemyToSpawn = m_SquidEnemy;
                break;
            case EnemyType.Starfish:
                enemyToSpawn = m_StarfishEnemy;
                break;
            default:
                goto case EnemyType.Squid;
        }

        return Instantiate(enemyToSpawn, pos, rot) as BaseEnemy;
    }

    void OrganizeEnemy(Transform enemyTF)
    {
        enemyTF.SetParent(m_Container.transform);
    }
}
