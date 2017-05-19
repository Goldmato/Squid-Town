using System.Collections;
using System.Collections.Generic;

using UnityEngine;

///<summary>
/// Registry for global variables/lists
///</summary>
public class Registry : MonoBehaviour
{
    // TODO: Improve singleton design
    public static Registry Current { get { return m_Instance; } } 

    public List<Door> Doors = new List<Door>();
    public List<Enemy> Enemies = new List<Enemy>();

    private static Registry m_Instance;

    void Awake()
    {
        if(m_Instance == null)
            m_Instance = this;
        else
            throw new UnityException("Please ensure there is only one Registry in the scene");
    }

    void Start()
    {
        // Add all active doors in scene to list
        foreach (var d in FindObjectsOfType<Door>())
        {
            Doors.Add(d);
        }

        Debug.Log("Doors: " + Doors.Count);
    }
}
