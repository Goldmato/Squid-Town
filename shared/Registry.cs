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

    private static Registry m_Instance;
    private static bool m_ExceptionFlag;

    void Awake()
    {
        if(m_Instance == null)
            m_Instance = this;
        else if(!m_ExceptionFlag)
        {
            m_ExceptionFlag = true;
            string activeRegistries = "GameObject(s) with Registry component(s)\n" +
                                      "--------------------------------------\n";
            foreach(var reg in GameObject.FindObjectsOfType(typeof(Registry))) 
            {
                activeRegistries += "\"" + reg.name + "\"\n";
            }
            
            Debug.Break();
            Debug.LogError(activeRegistries);
            throw new UnityException("Please ensure there is only one active registry in the scene");
        }
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
