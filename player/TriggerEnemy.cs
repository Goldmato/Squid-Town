using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TriggerEnemy : MonoBehaviour 
{
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<BaseEnemy>();
            if(!enemy.RunState)
                enemy.RunState = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<BaseEnemy>();
            enemy.RunState = false;
        }
    }
}
