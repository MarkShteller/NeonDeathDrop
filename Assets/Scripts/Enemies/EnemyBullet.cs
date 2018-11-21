using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    public float damage;
    public float speed = 0.5f;
    public float lifetime;

	void Update ()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
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
}
