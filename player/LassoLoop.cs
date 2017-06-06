using UnityEngine;

public class LassoLoop : MonoBehaviour
{

    public float WaitTime = 0.51f;
    public GameObject TeleportParticleSystem;

    void OnTriggerEnter(Collider other)
    {
        // do this if we have hit an enemy
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            
            // if enemy is disabled, return to caller
            if(enemy.Disabled)
                return;

            // the NavMeshAgent needs to be disabled before we can teleport to jail
            enemy.DisableNavMeshAgent();

            // disable the animator. it would be better to use an idle
            //		animation instead in the future.
            enemy.DisableAnimator();

            // teleport
            enemy.TeleportToJail();

            // play a sound
            gameObject.GetComponent<AudioSource>().Play();

            // play particle system
            //			gameObject.GetComponent<ParticleSystem> ().Play ();
            Instantiate(TeleportParticleSystem, transform.position, Quaternion.identity);

            // reset the position of the lasso
            gameObject.transform.parent.gameObject.GetComponent<Lasso>().Carry();

            // add to score
            GameController.Current.Score++;
        }
    }
}
