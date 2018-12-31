using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour, IPooledObject {

    public float damage;
    public float speed = 0.5f;
    public float rotationSpeed;
    public float lifetime;

    public Transform visuals;

    private float lifetimeRemaining;

	void Update ()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        Vector3 rot = visuals.rotation.eulerAngles;
        visuals.Rotate(0, rotationSpeed *Time.deltaTime , 0);

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
