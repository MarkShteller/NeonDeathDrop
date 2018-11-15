using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    public float damage;
    public float speed = 0.5f;
	
	void Update ()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerBehaviour>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
