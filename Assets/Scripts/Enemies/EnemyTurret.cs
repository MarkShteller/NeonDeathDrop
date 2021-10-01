﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyTurret : Enemy
{
    public float shootInterval;
    private float timeToShoot;

    public string bulletNameInPool;
    public Transform bulletSpawnPoint;
    public VisualEffect chargeEffect;
    public bool isFastShooter;

    internal override void Init()
    {
        base.Init();
        timeToShoot = shootInterval;
    }

    /*internal override void LookAtPlayer()
    {
        Vector3 lookPosition = new Vector3(playerObject.position.x, transform.position.y, playerObject.position.z);
        transform.LookAt(lookPosition);
    }*/

    internal override void StaticAction()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerObject.position);
        if (distanceFromPlayer > minDistanceTargeting && distanceFromPlayer < maxDistanceTargeting)
            movementStatus = MovementType.Shooting;
    }

    internal override void StunnedAction()
    {
        base.StunnedAction();
        chargeEffect.SetBool("IsCharging", false);
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
            if(!isFastShooter)
                animator.SetTrigger("Shoot");
            else
                animator.SetTrigger("ShootFast");

            chargeEffect.SetBool("IsCharging",true);
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.EnemyTurretShoot, transform.position);
        }
    }

    /*internal override void DyingAction()
    {
        base.DyingAction();
        gameObject.SetActive(false);
    }*/

    internal override void FireEvent()
    {
        chargeEffect.SetBool("IsCharging", false);
        GameObject newBullet = ObjectPooler.Instance.SpawnFromPool(bulletNameInPool, bulletSpawnPoint.position, transform.rotation);
        newBullet.GetComponent<EnemyBullet>().damage = this.damage;
    }

    internal override void TrackingAction()
    {
        return;
    }

}
