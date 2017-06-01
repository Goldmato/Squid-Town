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
    [SerializeField] private List<Enemy> m_SpawnableEnemies;

    private GameObject m_Container;
    private InJail m_InJail;

    void Start()
    {
        m_Container = new GameObject("Enemies");
        m_InJail = GameObject.FindGameObjectWithTag("InJail").GetComponent<InJail>();
    }

    public void SpawnEnemies(SpawnMethod method, BehaviourType ?moveType = null, int numEnemies = 10)
    {
        switch(method)
        {
            case SpawnMethod.Random:
                SpawnEnemiesRandom(moveType, numEnemies);
                break;
            case SpawnMethod.InHouses:
                SpawnEnemiesInHouses(moveType);
                break;
            case SpawnMethod.InJail:
                SpawnEnemiesInJail(moveType, numEnemies);
                break;
            default:
                throw new UnityException("Invalid spawn method requested (EnemyManager)");
        }
    }

    void SpawnEnemiesRandom(BehaviourType ?moveType, int numEnemies)
    {
        Debug.Log("Spawning [" + numEnemies + "] enemies randomly");

        float terrainRadius = (m_Terrain.terrainData.size.x + m_Terrain.terrainData.size.z) / 2;

        for(int i = 0; i < numEnemies; i++)
        {
            NavMeshHit hit;
            Vector3 randomPos = new Vector3(Random.Range(m_Terrain.transform.position.x, m_Terrain.terrainData.size.x + m_Terrain.transform.position.x), 0,
                                            Random.Range(m_Terrain.transform.position.z, m_Terrain.transform.position.z + m_Terrain.terrainData.size.z));
            NavMesh.SamplePosition(randomPos, out hit, terrainRadius, NavMesh.AllAreas);

            var newEnemy = Instantiate(m_SpawnableEnemies[Random.Range(0, m_SpawnableEnemies.Count)],
                 hit.position, Quaternion.identity) as Enemy;
            newEnemy.SetMoveMode(moveType);
            OrganizeEnemy(newEnemy.transform);

            // Debug.Log("Moving enemy [" + i + "] to nearest door...");
        }
    }

    void SpawnEnemiesInHouses(BehaviourType ?moveType)
    {
        Debug.Log("Spawning [" + GameController.Current.Doors.Count + "] enemies in houses");

        for(int i = 0; i < GameController.Current.Doors.Count; i++)
        {
            var newEnemy = Instantiate(m_SpawnableEnemies[Random.Range(0, m_SpawnableEnemies.Count)],
                 GameController.Current.Doors[i].transform.position, Quaternion.identity) as Enemy;
            newEnemy.SetMoveMode(moveType);
            OrganizeEnemy(newEnemy.transform);
        }
    }

    void SpawnEnemiesInJail(BehaviourType ?moveType, int numEnemies)
    {
        Debug.Log("Spawning [" + numEnemies + "] enemies in jail");

        for (int i = 0; i < numEnemies; i++)
        {
            //TODO: Spawn enemies in a circular area within the main jail
            Vector3 randomPos = m_InJail.RandomLocation();

            var newEnemy = Instantiate(m_SpawnableEnemies[Random.Range(0, m_SpawnableEnemies.Count)],
                 randomPos, Quaternion.identity) as Enemy;
            newEnemy.SetMoveMode(moveType);
            OrganizeEnemy(newEnemy.transform);
        }
    }

    void OrganizeEnemy(Transform enemyTF)
    {
        enemyTF.SetParent(m_Container.transform);
    }
}
