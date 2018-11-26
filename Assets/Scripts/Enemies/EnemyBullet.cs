using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour, IPooledObject {

    public float damage;
    public float speed = 0.5f;
    public float lifetime;
    private float lifetimeRemaining;

	void Update ()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifetimeRemaining -= Time.deltaTime;
        if (lifetimeRemaining <= 0)
            gameObject.SetActive(false);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }

    public void OnObjectSpawn()
    {
        lifetimeRemaining = lifetime;
    }
}
