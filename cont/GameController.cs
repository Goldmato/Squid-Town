using System.Collections;
using System.Collections.Generic;

using UnityEngine;

///<summary>
/// Main game controller singleton class
///</summary>
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(EnemySpawner))]
public class GameController : MonoBehaviour
{
    public static GameController Current { get { return m_Instance; } }

    public EnemyController EC { get { return m_EnemyController; } }
    public EnemySpawner ES { get { return m_EnemySpawner; } }
    public GameObject Jail { get { return m_MainJail; } }

    [SerializeField] private GameObject m_MainJail;

    private static GameController m_Instance;
    private static bool m_ExceptionFlag;

    private EnemyController m_EnemyController;
    private EnemySpawner m_EnemySpawner;

    void Awake()
    {
        if(m_Instance == null)
            m_Instance = this;
        else if(!m_ExceptionFlag)
        {
            m_ExceptionFlag = true;
            string activeControllers = "GameObject(s) with GameController component(s)\n" +
                                      "--------------------------------------\n";
            foreach(var cont in GameObject.FindObjectsOfType(typeof(GameController)))
            {
                activeControllers += "\"" + cont.name + "\"\n";
            }

            Debug.Break();
            Debug.LogError(activeControllers);
            throw new UnityException("Please ensure there is only one active gameController in the scene");
        }

        m_EnemyController = GetComponent<EnemyController>();
        m_EnemySpawner = GetComponent<EnemySpawner>();
    }

    void Start()
    {
        StartCoroutine(SpawnFirstWaveTest());
    }

    IEnumerator SpawnFirstWaveTest()
    {
        yield return new WaitForSeconds(0.5f);
        m_EnemySpawner.SpawnEnemies(SpawnMethod.InJail);
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(m_EnemyController.UpdateEnemies());
    }
}
