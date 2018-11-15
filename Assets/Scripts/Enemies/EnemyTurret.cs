using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : Enemy
{

    public float minDistanceShooting;
    public float maxDistanceShooting;

    public float shootInterval;
    private float timeToShoot;

    public GameObject bullet;
    public Transform bulletSpawnPoint;

    private Transform playerObject;

    internal override void Init()
    {
        base.Init();
        timeToShoot = shootInterval;
        playerObject = GameManager.Instance.PlayerInstance.transform;
    }

    internal override void LookAtPlayer()
    {
        transform.LookAt(playerObject);
    }

    internal override void StaticAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer > minDistanceShooting && distanceFromPlayer < maxDistanceShooting)
            movementStatus = MovementType.Shooting;
    }

    internal override void ShootingAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer < minDistanceShooting || distanceFromPlayer > maxDistanceShooting)
        {
            movementStatus = MovementType.Static;
            return;
        }

        timeToShoot -= Time.deltaTime;
        if (timeToShoot <= 0)
        {
            timeToShoot = shootInterval;
            GameObject newBullet = Instantiate(bullet, bulletSpawnPoint.position, transform.rotation);
            newBullet.GetComponent<EnemyBullet>().damage = this.damage;
        }
    }

}
