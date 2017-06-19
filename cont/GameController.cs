﻿using System.Collections;
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

    ///<summary>
    /// Enemy Controller reference
    ///</summary>
    public EnemyController EC { get { return m_EnemyController; } }
    ///<summary>
    /// Enemy Spawner reference
    ///</summary>
    public EnemySpawner ES { get { return m_EnemySpawner; } }
    
    public JailFloor Jail { get { return m_MainJail; } }
    public GameObject Player { get { return m_MainPlayer; } }
    public List<Door> Doors { get { return m_Doors; } }

    [SerializeField] private JailFloor m_MainJail;
    [SerializeField] private GameObject m_MainPlayer;

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
            m_TextController.UpdateScore(value, m_EnemyController.EnemyCount - value);
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

        if(m_MainPlayer == null)
            m_MainPlayer = GameObject.FindGameObjectWithTag("Player");

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
        StartCoroutine(SpawnFirstWaveTest());
    }

    IEnumerator SpawnFirstWaveTest()
    {
        yield return new WaitForSeconds(0.5f);
        ES.SpawnEnemies(SpawnMethod.InJail, EnemyType.Squid, BehaviourType.RandomMovement, numEnemies: 5);
        ES.SpawnEnemies(SpawnMethod.Random, EnemyType.Squid, BehaviourType.SeekDoors, numEnemies: Doors.Count);
        ES.SpawnEnemies(SpawnMethod.Random, EnemyType.Starfish, numEnemies: 1);
        EC.StartEnemyUpdateCycle();
        Score = 0;
    }

    void GameWon()
    {
        // TODO: Load game winning scene

        Debug.LogWarning("All enemies captured! You win!");
    }
}
