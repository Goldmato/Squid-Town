using System.Collections;
using System.Collections.Generic;

using UnityEngine;

///<summary>
/// Main game controller singleton class
///</summary>
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(EnemySpawner))]
[RequireComponent(typeof(TextController))]
public class GameController : MonoBehaviour
{
    public static GameController Current { get { return m_Instance; } }

    public EnemyController EC { get { return m_EnemyController; } }
    public EnemySpawner ES { get { return m_EnemySpawner; } }
    public GameObject Jail { get { return m_MainJail; } }
    public List<Door> Doors { get { return m_Doors; } }

    [SerializeField] private GameObject m_MainJail;

    private static GameController m_Instance;
    private static bool m_ExceptionFlag;

    private List<Door> m_Doors = new List<Door>();
    private EnemyController m_EnemyController;
    private EnemySpawner m_EnemySpawner;
    private TextController m_TextController;

    private int m_Score;
    public int Score 
    { 
        get { return m_Score; } 
        set 
        {
            m_Score = value; 
            m_TextController.UpdateScore(value); 
            if(m_Score >= m_EnemyController.EnemyCount &&
                m_Score > 0)
            {
                GameWon();
            }
        } 
    }

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
        m_TextController = GetComponent<TextController>();

        // Find all doors in scene and add to list
        foreach(Door d in FindObjectsOfType(typeof(Door)))
        {
            m_Doors.Add(d);
        }
    }

    void Start()
    {
        // Set intial score to 0;
        Score = 0;

        StartCoroutine(SpawnFirstWaveTest());
    }

    IEnumerator SpawnFirstWaveTest()
    {
        yield return new WaitForSeconds(0.5f);
        m_EnemySpawner.SpawnEnemies(SpawnMethod.InJail);
        yield return new WaitForSeconds(2.0f);
        m_EnemyController.StartEnemyUpdateCycle();
    }

    void GameWon()
    {
        // TODO: Load game winning scene

        Debug.LogWarning("All enemies captured! You win!");
    }
}
