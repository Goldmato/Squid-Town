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

    void Start()
	{
        m_Container = new GameObject("Enemies");

        // DEBUGGING/TESTING
        SpawnEnemies(SpawnMethod.InHouses);
    }

    public void SpawnEnemies(SpawnMethod method, int numEnemies = 10)
	{
		switch (method)
		{
			case SpawnMethod.Random:
                SpawnEnemiesRandom(numEnemies);
                break;
            case SpawnMethod.InHouses:
				SpawnEnemiesInHouses();
                break;
            case SpawnMethod.InJail:
				SpawnEnemiesInJail(numEnemies);
                break;
            default:
                throw new UnityException("ERROR Invalid spawn method requested (EnemyManager)");
        }

        EnemyController.Current.MoveAllEnemies();
	}

	void SpawnEnemiesRandom(int numEnemies)
	{
        Debug.Log("Spawning [" + numEnemies + "] enemies randomly");

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
        }
    }

	void SpawnEnemiesInHouses()
	{
        Debug.Log("Spawning [" + Registry.Current.Doors.Count + "] enemies in houses");
        
        for (int i = 0; i < Registry.Current.Doors.Count; i++)
        {
            var newEnemy = Instantiate(m_SpawnableEnemies[Random.Range(0, m_SpawnableEnemies.Count)],
				 Registry.Current.Doors[i].transform.position, Quaternion.identity) as Enemy;
            newEnemy.transform.SetParent(m_Container.transform);

            newEnemy.Move();
        }
	}

	void SpawnEnemiesInJail(int numEnemies)
	{
        Debug.Log("Spawning [" + numEnemies + "] enemies in jail");
        
        throw new System.NotImplementedException();
    }
}
