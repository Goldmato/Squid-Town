using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class SimpleEnemy : BaseEnemy 
{
	[SerializeField] [Range(0, 10f)] protected float m_HideDelayLow = 1f;
    [SerializeField] [Range(0, 10f)] protected float m_HideDelayHigh = 5f;

    const float DISABLE_INTERVAL = 5f;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(Time.timeSinceLevelLoad > m_DisableDelay && other.CompareTag("Door"))
        {
            Debug.Log("Enemy entered doorway!");

            // Cooldown delay for door trigger events
            m_DisableDelay = Time.timeSinceLevelLoad + DISABLE_INTERVAL;
            StartCoroutine(TimedDisable());
        }
    }

    protected virtual IEnumerator TimedDisable()
    {
        m_EnemyDisabled = true;
        m_Agent.isStopped = true;
        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = false; }

        var hideDelay = Random.Range(m_HideDelayLow, m_HideDelayHigh);
        yield return new WaitForSeconds(hideDelay);

        for(int i = 0; i < m_Renderer.Length; i++) { m_Renderer[i].enabled = true; }

        m_EnemyDisabled = false;

        MoveUpdate();
    }
}
