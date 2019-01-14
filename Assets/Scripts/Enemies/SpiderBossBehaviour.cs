﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossBehaviour : MonoBehaviour
{
    enum SpiderBossState { Stunned, Die, Idle, MachineGun, Pulse, Wave, Stomp, PulseStomp, SpawnEnemies}

    private Transform playerObject;
    private List<SpiderBossState> ActionsQueue;
    private SpiderBossState currentState;

    private float timeToNextState;
    private float timeToShoot;

    public Transform bodyVisuals;
    public Transform[] pulseBulletSpawnPoints;
    public Transform[] waveBulletSpawnPoints;
    public SpiderLegBehaviour[] spiderLegs;
    public BossEnemySpawnPoint[] EnemySpawnPoints;

    public string bulletNameInPool;

    private float bulletDamage = 1;
    private bool hasSpawnedEnemies = false;

    void Start ()
    {
        playerObject = GameManager.Instance.PlayerInstance.transform;
        ActionsQueue = new List<SpiderBossState>() { SpiderBossState.Idle, SpiderBossState.SpawnEnemies };
        timeToNextState = 7;
        timeToShoot = 0;

        foreach (SpiderLegBehaviour spiderLeg in spiderLegs)
        {
            spiderLeg.Init();
        }

        print("# Boss finish init");
    }
	
	void Update ()
    {
        timeToNextState -= Time.deltaTime;
        if (timeToNextState <= 0)
        {
            currentState = ActionsQueue[1];
            ActionsQueue.RemoveAt(0);
            SpiderBossState nextState = (SpiderBossState) Random.Range(2, 9);
            ActionsQueue.Add(nextState);

            //ActionsQueue.Add(SpiderBossState.Idle);
            print("Boss changed state: " +currentState.ToString());
        }

        switch (currentState)
        {
            case SpiderBossState.Idle:
                if(timeToNextState <= 0)
                    timeToNextState = 3;
                LookAtPlayer();
                break;
            case SpiderBossState.Stunned:
                if (timeToNextState <= 0)
                    timeToNextState = 10;
                break;
            case SpiderBossState.MachineGun:
                if (timeToNextState <= 0)
                    timeToNextState = 4;

                LookAtPlayer();
                if (timeToShoot <= 0)
                {
                    ShootFromPoint(pulseBulletSpawnPoints[0]);
                    timeToShoot = 0.3f;
                }
                timeToShoot -= Time.deltaTime;
                break;
            case SpiderBossState.Pulse:
                if (timeToNextState <= 0)
                    timeToNextState = 7;

                LookAtPlayer();
                if (timeToShoot <= 0)
                {
                    foreach (Transform sp in pulseBulletSpawnPoints)
                    {
                        ShootFromPoint(sp);
                    }
                    timeToShoot = 0.8f;
                }
                timeToShoot -= Time.deltaTime;
                break;

            case SpiderBossState.Wave:
                if (timeToNextState <= 0)
                    timeToNextState = 7;

                LookAtPlayer();
                if (timeToShoot <= 0)
                {
                    foreach (Transform sp in waveBulletSpawnPoints)
                    {
                        ShootFromPoint(sp);
                    }
                    timeToShoot = 0.8f;
                }
                timeToShoot -= Time.deltaTime;
                break;
            case SpiderBossState.PulseStomp:
                if (timeToNextState <= 0)
                    timeToNextState = 0;
                break;
            case SpiderBossState.SpawnEnemies:
                if (timeToNextState <= 0)
                    timeToNextState = 5;

                if (!hasSpawnedEnemies)
                {
                    hasSpawnedEnemies = true;
                    EnemyManager.Instance.SpawnEnemiesForBossBattle(EnemySpawnPoints);
                }
                break;
            case SpiderBossState.Stomp:
                if (timeToNextState <= 0)
                    timeToNextState = 0;
                break;

            case SpiderBossState.Die:
                break;
        }
	}

    private void ShootFromPoint(Transform point)
    {
        GameObject newBullet = ObjectPooler.Instance.SpawnFromPool(bulletNameInPool, point.position, point.rotation);
        newBullet.GetComponent<EnemyBullet>().damage = this.bulletDamage;
    }

    internal virtual void LookAtPlayer()
    {
        Vector3 lookPosition = new Vector3(playerObject.position.x, transform.position.y, playerObject.position.z);
        //lookPosition = Vector3.Lerp()
        bodyVisuals.LookAt(lookPosition, Vector3.up);
    }
}
