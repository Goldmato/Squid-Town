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
            if(!enemy.GetRunState())
                enemy.SetRunState(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<BaseEnemy>();
            enemy.SetRunState(false);
        }
    }
}
