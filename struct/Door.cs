using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Class controlling door behaviour/data
///</summary>
[RequireComponent(typeof(Collider))]
public class Door : MonoBehaviour 
{
    [SerializeField] [Range(0.1f, 10f)] private float m_DisableDelay = 2f; 

    private bool m_DisableEnemyFlag = true;

    void OnTriggerEnter(Collider other)
    {
        if(m_DisableEnemyFlag && other.CompareTag("Enemy")) 
        {
            Debug.Log("Enemy entering door!");
            StartCoroutine(DisableEnemy(other.gameObject));
        }
    }

    IEnumerator DisableEnemy(GameObject obj)
    {
        m_DisableEnemyFlag = false;
        var rend = obj.GetComponentsInChildren<Renderer>();
        var agent = obj.GetComponent<NavMeshAgent>();
        var enemy = obj.GetComponent<EnemyTest>();

        agent.isStopped = true;
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = false;
        }
        
        yield return new WaitForSeconds(m_DisableDelay);

        enemy.Continue = true;
        agent.isStopped = false;
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].enabled = true;
        }

        yield return new WaitForSeconds(m_DisableDelay);
        m_DisableEnemyFlag = true;
    }
}
