using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    // TODO: Implement features shown in https://www.draw.io/#DEnemyManager

    [SerializeField] private Terrain m_Terrain;
    [SerializeField] private List<Enemy> m_SpawnableEnemies;

    private GameObject m_Container;

    void Start()
	{
        m_Container = new GameObject("Enemies");

        // DEBUGGING/TESTING
        SpawnEnemies(10, SpawnMethod.Random);
    }

    public void SpawnEnemies(int numEnemies, SpawnMethod method)
	{
		switch (method)
		{
			case SpawnMethod.Random:
                Debug.Log("Spawning [" + numEnemies + "] enemies randomly");
                SpawnEnemiesRandom(numEnemies);
                break;
            case SpawnMethod.InHouses:
				Debug.Log("Spawning [" + numEnemies + "] enemies in houses");
				SpawnEnemiesInHouses(numEnemies);
                break;
            case SpawnMethod.InJail:
				Debug.Log("Spawning [" + numEnemies + "] enemies in jail");
				SpawnEnemiesInJail(numEnemies);
                break;
            default:
                throw new UnityException("ERROR Invalid spawn method requested (EnemyManager)");
        }
	}

	void SpawnEnemiesRandom(int numEnemies)
	{
        float terrainRadius = (m_Terrain.terrainData.size.x + m_Terrain.terrainData.size.z) / 2;

        for (int i = 0; i < numEnemies; i++)
		{
            NavMeshHit hit;
            Vector3 randomPos = new Vector3(Random.Range(m_Terrain.transform.position.x, m_Terrain.terrainData.size.x + m_Terrain.transform.position.x), 0,
										    Random.Range(m_Terrain.transform.position.z, m_Terrain.transform.position.z + m_Terrain.terrainData.size.z));
            NavMesh.SamplePosition(randomPos, out hit, terrainRadius, NavMesh.AllAreas);

            var newEnemy = Instantiate(m_SpawnableEnemies[Random.Range(0, m_SpawnableEnemies.Count)],
				 hit.position, Quaternion.identity) as Enemy;
            newEnemy.transform.SetParent(m_Container.transform);

            // Debug.Log("Moving enemy [" + i + "] to nearest door...");
            newEnemy.Move();
        }
	}

	void SpawnEnemiesInHouses(int numEnemies)
	{
		throw new System.NotImplementedException();
	}

	void SpawnEnemiesInJail(int numEnemies)
	{
        throw new System.NotImplementedException();
    }
}
