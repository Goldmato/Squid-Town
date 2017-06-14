using UnityEngine;

public class LassoLoop : MonoBehaviour
{

    public float WaitTime = 0.51f;
    public GameObject TeleportParticleSystem;

    void OnTriggerEnter(Collider other)
    {
        // do this if we have hit an enemy
        if(other.CompareTag("Enemy"))
        {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            
            // if enemy is disabled, return to caller
            if(enemy.Disabled)
                return;

            // use the main EnemyController to handle the jailing sequence
            GameController.Current.EC.SendEnemyToJail(enemy);

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
