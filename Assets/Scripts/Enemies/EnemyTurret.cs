using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : Enemy
{
    public float shootInterval;
    private float timeToShoot;

    public string bulletNameInPool;
    public Transform bulletSpawnPoint;

    internal override void Init()
    {
        base.Init();
        timeToShoot = shootInterval;
    }

    internal override void LookAtPlayer()
    {
        Vector3 lookPosition = new Vector3(playerObject.position.x, transform.position.y, playerObject.position.z);
        transform.LookAt(lookPosition);
    }

    internal override void StaticAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer > minDistanceTargeting && distanceFromPlayer < maxDistanceTargeting)
            movementStatus = MovementType.Shooting;
    }

    internal override void ShootingAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer < minDistanceTargeting || distanceFromPlayer > maxDistanceTargeting)
        {
            movementStatus = MovementType.Static;
            return;
        }

        timeToShoot -= Time.deltaTime;
        if (timeToShoot <= 0)
        {
            timeToShoot = shootInterval;
            GameObject newBullet = ObjectPooler.Instance.SpawnFromPool(bulletNameInPool, bulletSpawnPoint.position, transform.rotation);
            newBullet.GetComponent<EnemyBullet>().damage = this.damage;
        }
    }

}
