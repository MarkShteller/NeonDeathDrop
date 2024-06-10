using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour, IPooledObject {

    public float damage;
    public float speed = 0.5f;
    private float m_speed;
    public float rotationSpeed;
    public float lifetime;
    public bool isParried;

    public Transform visuals;

    private float lifetimeRemaining;
    private Vector3 vForward;

    private void Start()
    {
        vForward = transform.forward;
        isParried = false;
        m_speed = speed;
    }

    void Update ()
    {
        transform.position += vForward * m_speed * Time.deltaTime;
        Vector3 rot = visuals.rotation.eulerAngles;
        visuals.Rotate(0, rotationSpeed *Time.deltaTime , 0);

        lifetimeRemaining -= Time.deltaTime;
        if (lifetimeRemaining <= 0)
            gameObject.SetActive(false);
	}

    void OnTriggerEnter(Collider other)
    {
        //print("trigger: "+other.name);

        if (other.tag == "Player")
        {
            var player = other.GetComponent<PlayerBehaviour>();
            player.TakeDamage(damage);
            //player.isDashing = false;
            gameObject.SetActive(false);
        }
        if (other.tag == "Enemy" && isParried)
        {
            /*other.gameObject.GetComponent<Enemy>()
                .ForcePush(-vForward, GameManager.Instance.PlayerInstance.currentPushForce, PlayerBehaviour.PlayerAttackType.ParryBullet);*/
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                GameManager.Instance.PlayerInstance.MakeHole(enemy.GetPointPos());
                other.GetComponent<Enemy>().lastAttackType = PlayerBehaviour.PlayerAttackType.ParryBullet;
            }
            gameObject.SetActive(false);
        }
    }

    public void OnObjectSpawn()
    {
        vForward = transform.forward;
        lifetimeRemaining = lifetime;
        isParried = false;
        m_speed = speed;
    }

    public void Parry(Vector3 newDirection, float speedMul)
    {
        vForward = newDirection;
        m_speed = speed * -speedMul;
        isParried = true;
    }
}
