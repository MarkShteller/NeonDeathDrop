﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossBehaviour : MonoBehaviour
{
    enum SpiderBossState { Stunned, Die, Idle, MachineGun, Pulse, Wave, Stomp, PulseStomp, SpawnEnemies}
    enum SpiderBossPhase { PhaseA, PhaseB, PhaseC }

    private Transform playerObject;
    private List<SpiderBossState> ActionsQueue;
    private SpiderBossState currentState;
    private SpiderBossPhase currentPhase;

    private float timeToNextState;
    private float timeToShoot;
    private float timeToStomp;

    private int stompingLegA;
    private int stompingLegB;

    public Transform bodyVisuals;
    public Transform[] pulseBulletSpawnPoints;
    public Transform[] waveBulletSpawnPoints;
    public Transform[] wavePhaseBBulletSpawnPoints;
    public SpiderLegBehaviour[] spiderLegs;
    public BossEnemySpawnPoint[] EnemySpawnPoints;
    public GameObject FinalBlowVFX;
    public Animator animator;
    public CinemachineImpulseSource shakeSource;

    public string bulletANameInPool;
    public string bulletBNameInPool;

    public Transform FinalLegPoint;
    public SpiderLegBehaviour lastLeg;

    public bool shouldSpawnEnemies;

    private float bulletDamage = 1;
    private bool hasSpawnedEnemies = false;
    private bool isTransitioning = false;

    void Start ()
    {
        playerObject = GameManager.Instance.PlayerInstance.transform;
        ActionsQueue = new List<SpiderBossState>() { SpiderBossState.Idle, SpiderBossState.PulseStomp, SpiderBossState.Idle, SpiderBossState.MachineGun, SpiderBossState.SpawnEnemies, SpiderBossState.Idle };
        timeToNextState = 7;
        timeToShoot = 0;
        timeToStomp = 0;

        stompingLegA = -1;
        stompingLegB = -1;

        currentState = ActionsQueue[0];

        currentPhase = SpiderBossPhase.PhaseA;

        foreach (SpiderLegBehaviour spiderLeg in spiderLegs)
        {
            spiderLeg.Init();
        }

        StartCoroutine(InitCamera());
        //ReorderLegs();

        print("# Boss finish init");
    }

    public void IntroBossEvent()
    {
        StartCoroutine(AudioManager.Instance.StartMusic(AudioManager.LevelMusicTracks.Raheem_Battle));
    }
	
	void Update ()
    {
        /////temp
        if (Input.GetKeyUp(KeyCode.Q))
        {
            //TriggerLegsJumping();
            animator.SetTrigger("Jump");
        }

        if (!isTransitioning)
        {
            timeToNextState -= Time.deltaTime;
            if (timeToNextState <= 0)
            {
                currentState = ActionsQueue[1];
                ActionsQueue.RemoveAt(0);

                SpiderBossState nextState = (SpiderBossState)Random.Range(2, 9);
                while (nextState == currentState) //make sure to have a different state every time 
                    nextState = (SpiderBossState)Random.Range(2, 9);
                ActionsQueue.Add(nextState);
                //ActionsQueue.Add(SpiderBossState.Idle);

                hasSpawnedEnemies = false;

                print("Boss changed state: " + currentState.ToString());
            }
        }
        switch (currentPhase)
        {
            case SpiderBossPhase.PhaseA:
                PhaseAStateMachine();
                ShouldMoveToNextStage(4);
                break;
            case SpiderBossPhase.PhaseB:
                if (!isTransitioning)
                {
                    PhaseBStateMachine();
                    ShouldMoveToNextStage(1);
                }
                break;
            case SpiderBossPhase.PhaseC:

                break;
        }
    }

    private IEnumerator InitCamera()
    {
        yield return null;
        yield return null;
        GameManager.Instance.cameraRef.SetSecondTargerAndInterpolate(transform);
    }

    private void ShouldMoveToNextStage(int countLegs)
    {
        int aliveLegs = 0;
        SpiderLegBehaviour lastAliveLeg = null;
        foreach (SpiderLegBehaviour leg in spiderLegs)
        {
            if (!leg.isDead)
            {
                aliveLegs++;
                lastAliveLeg = leg;
            }
        }

        if (aliveLegs <= countLegs)
        {
            currentPhase++;
            isTransitioning = true;
            if (countLegs > 1)
            {
                animator.SetTrigger("Jump");
            }
            else // one leg left
            {
                animator.SetTrigger("Falling");
                FinalBlowVFX.SetActive(true);
                LookDiagonal();
                AudioManager.Instance.StopMusic();
                EnemyManager.Instance.KillAllEnemiesInSublevel(0);
                foreach (SpiderLegBehaviour leg in spiderLegs)
                {
                    leg.gameObject.SetActive(false);
                }
                lastLeg.gameObject.SetActive(true);
                lastLeg.ClingForYourLife(FinalLegPoint);

                /*if (lastAliveLeg == null || aliveLegs == 0 || lastAliveLeg.isActive == false)
                {
                    print("activating last leg");
                    lastAliveLeg = spiderLegs[0];
                    lastAliveLeg.gameObject.SetActive(true);
                    lastAliveLeg.isDead = false;
                }*/

                //astAliveLeg.ClingForYourLife(FinalLegPoint);
            }
            print("# Changing boss phase!");
        }
    }

    private void PhaseAStateMachine()
    {
        switch (currentState)
        {
            case SpiderBossState.Idle:
                if (timeToNextState <= 0)
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
                    timeToNextState = 5;

                if (timeToStomp <= 0)
                {
                    print("## preparing spider leg pulse");

                    int rndLegA = Random.Range(0, spiderLegs.Length / 2); ;
                    while (rndLegA == stompingLegA) //make sure to have a different leg every time 
                        rndLegA = Random.Range(0, spiderLegs.Length / 2);

                    stompingLegA = rndLegA;

                    int rndLegB = Random.Range(spiderLegs.Length / 2, spiderLegs.Length); ;
                    while (rndLegB == stompingLegA) //make sure to have a different leg every time 
                        rndLegB = Random.Range(spiderLegs.Length / 2, spiderLegs.Length);

                    stompingLegB = rndLegB;

                    spiderLegs[stompingLegA].Stopm();
                    spiderLegs[stompingLegB].Stopm();

                    timeToStomp = 1;
                }
                timeToStomp -= Time.deltaTime;
                break;

            case SpiderBossState.SpawnEnemies:
                if (shouldSpawnEnemies)
                {
                    if (timeToNextState <= 0)
                        timeToNextState = 5;

                    LookForward();
                    if (!hasSpawnedEnemies)
                    {
                        hasSpawnedEnemies = true;
                        EnemyManager.Instance.SpawnEnemiesForBossBattle(EnemySpawnPoints);
                        animator.SetTrigger("Scream");
                        shakeSource.GenerateImpulse();
                    }
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

    private void PhaseBStateMachine()
    {
        switch (currentState)
        {
            case SpiderBossState.Idle:
                if (timeToNextState <= 0)
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
                        ShootFromPoint(sp, false);
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
                    foreach (Transform sp in wavePhaseBBulletSpawnPoints)
                    {
                        ShootFromPoint(sp, false);
                    }
                    timeToShoot = 0.8f;
                }
                timeToShoot -= Time.deltaTime;
                break;
            case SpiderBossState.PulseStomp:
                if (timeToNextState <= 0)
                    timeToNextState = 5;

                if (timeToStomp <= 0)
                {
                    int rndLegA = Random.Range(0, spiderLegs.Length / 2); ;
                    while (rndLegA == stompingLegA) //make sure to have a different leg every time 
                        rndLegA = Random.Range(0, spiderLegs.Length / 2);

                    stompingLegA = rndLegA;

                    int rndLegB = Random.Range(spiderLegs.Length / 2, spiderLegs.Length); ;
                    while (rndLegB == stompingLegA) //make sure to have a different leg every time 
                        rndLegB = Random.Range(spiderLegs.Length / 2, spiderLegs.Length);

                    stompingLegB = rndLegB;

                    spiderLegs[stompingLegA].Stopm();
                    spiderLegs[stompingLegB].Stopm();

                    timeToStomp = 1;
                }
                timeToStomp -= Time.deltaTime;
                break;

            case SpiderBossState.SpawnEnemies:
                if (shouldSpawnEnemies)
                {
                    if (timeToNextState <= 0)
                        timeToNextState = 5;

                    LookForward();
                    if (!hasSpawnedEnemies)
                    {
                        hasSpawnedEnemies = true;
                        EnemyManager.Instance.SpawnEnemiesForBossBattle(EnemySpawnPoints);
                        animator.SetTrigger("Scream");
                        shakeSource.GenerateImpulse();
                    }
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

    private void ShootFromPoint(Transform point, bool isBulletTypeA=true)
    {
        string bulletType = isBulletTypeA ? bulletANameInPool : bulletBNameInPool;
        GameObject newBullet = ObjectPooler.Instance.SpawnFromPool(bulletType, point.position, point.rotation);
        newBullet.GetComponent<EnemyBullet>().damage = this.bulletDamage;
    }

    internal virtual void LookAtPlayer()
    {
        Vector3 lookPosition = new Vector3(playerObject.position.x, transform.position.y, playerObject.position.z);
        //transform.LookAt(lookPosition);

        Vector3 direction = lookPosition - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        toRotation = Quaternion.Lerp(bodyVisuals.rotation, toRotation, 0.3f);

        bodyVisuals.rotation = toRotation;

        //bodyVisuals.LookAt(lookPosition, Vector3.up);
    }

    private void LookForward()
    {
        bodyVisuals.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void LookDiagonal()
    {
        bodyVisuals.rotation = Quaternion.Euler(0, 135, 0);
    }

    public void FinalBlow()
    {
        StartCoroutine(FinalBlowCor());
    }

    public IEnumerator FinalBlowCor()
    {
        print("## boss final blow!");
        GameManager.Instance.DashSlomo(20f);
        currentState = SpiderBossState.Die;
        FinalBlowVFX.SetActive(false);
        animator.SetTrigger("Falling");
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerFall, transform.position);

        yield return new WaitForSeconds(3f);
        
        UIManager.Instance.BossSlain();
    }

    public void TriggerLegsJumping()
    {
        print("## TriggerLegsJumping");
        foreach (SpiderLegBehaviour leg in spiderLegs)
        {
            if (!leg.isDead)
            {
                leg.animator.SetTrigger("Jump");
            }
        }
    }

    public void FinishedJump()
    {
        print("#### finished jumping");
        isTransitioning = false;
    }

    public void ReorderLegs()
    {
        foreach (SpiderLegBehaviour leg in spiderLegs)
        {
            leg.isDead = true;
            leg.gameObject.SetActive(false);
        }
        print("## calling leg land");
        spiderLegs[0].isDead = false;
        spiderLegs[0].gameObject.SetActive(true);
        spiderLegs[0].animator.SetTrigger("Land");
        spiderLegs[3].isDead = false;
        spiderLegs[3].gameObject.SetActive(true);
        spiderLegs[3].animator.SetTrigger("Land");
        spiderLegs[4].isDead = false;
        spiderLegs[4].gameObject.SetActive(true);
        spiderLegs[4].animator.SetTrigger("Land");
        spiderLegs[7].isDead = false;
        spiderLegs[7].gameObject.SetActive(true);
        spiderLegs[7].animator.SetTrigger("Land");
    }
}
