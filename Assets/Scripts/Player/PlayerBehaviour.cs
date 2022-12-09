using Cinemachine;
using EZCameraShake;
using SplineEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerBehaviour : MonoBehaviour
{
    public float movementSpeed = 0.1f;

    private float healthPoints;
    public float totalHealthPoints;

    private float manaPoints;
    public float totalManaPoints;
    public float manaRegenAmount;

    public float holeTimeToRegen;

    public float pushManaCost;
    public float holeManaCost;

    public float dashManaCost;
    public float somersaultManaCost;
    public float AOEManaCost;
    public float slomoFrameCost;
    public int shockwaveCoreCost;
    public float dashDuration;
    public float dashSpeed;
    public float fallDamage = 1;
    public float takenDamageCooldown;
    public float buttonPressCooldown = 0.1f;
    private float currentButtonCooldown;

    public int coresCount;
    public int enemyDefeatedCount;

    public AudioSource soundEffectSource;

    private List<BasePowerupBehaviour> activePowerUps;

    private Transform checkpoint;
    [HideInInspector] public Vector3 spawnPosition;

    private float lastTimeDamageTaken = 0;
    private bool enableDash = true;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isInvinsible = false;
    private Vector3 lastDashDir;
    private bool isDashImpact = false;

    [HideInInspector] public bool isDoingSomersault = false;

    private bool isCharging;
    private float chargeTime;
    public float chargeTimeAttract;
    public float chargeTimeRepel;
    private float chargeCost = 5;

    private float timeOverPit = 0;

    private float slomoTriggerCooldown = 0.2f;
    private float currentSlomoTriggerCooldown;
    private bool isHoleSlomo;
    private bool isChargeMana;

    [HideInInspector] public GameObject prevHoveredObject;
    [HideInInspector] public GameObject currHoveredObject;

    [HideInInspector] public bool enableControlls = true;
    [HideInInspector] public bool enableMovement = true;
    private bool isFalling;
    private float prevRotationAngle;
    private float revolutionCount;
    private float currentRevolutionCooldown;
    private float revolutionCooldown = 1;
    private List<Enemy> launchedEnemies;
    private Vector3 dashImpactEnemyLocation;

    private CapsuleCollider capsuleCollider;

    private PlayerAimAssist aimAssist;

    public float sprintCountdown = 3;
    private float sprintTimer;
    private bool isSprinting;
    private float sprintSpeedMul = 1.5f;

    private CinemachineImpulseSource shakeSource;
    private FMOD.Studio.PARAMETER_ID pushParameterId;
    
    private FMOD.Studio.EventInstance sprintingSoundEvent;
    private FMOD.Studio.EventInstance slomoSoundEvent;

    //private float defaultRadius = 0.35f;
    private Vector3 defaultForcePushTriggerSize;
    public float pushRadius = 4f;

    [SerializeField]
    private float pushForce;
    [HideInInspector]
    public float currentPushForce;
    public BoxCollider forcePushTriggerCollider;
    public VisualEffect forcePushEffect;
    public VisualEffect somersaultEffect;
    public VisualEffect dashEffect;
    public BezierSpline finisherSpline;

    public LevelGenerator gridHolder;
    public Transform visualsHolder;

    public PlayerShockwaveBehavior shockwaveBehavior;
    public ForcePushFloorTrigger forcePushFloorTrigger;

    //public Animation shockSphereAnimation;

    public Animator animator;

    public Color tileHighlightColor;
    public Color tileOriginalColor;
    public Color tileWallOriginalColor;

    public GameObject spotlight;
    public GameObject mainDirectionalLight;

    public GameObject[] trails;

    public enum PlayerAttackType { None, Push, Dash, Heavy, Launch }

    public bool IsTestMode = false;
    private bool isDead;

    void Start()
    {
        if (!IsTestMode)
            gridHolder = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();

        capsuleCollider = GetComponent<CapsuleCollider>();
        aimAssist = GetComponentInChildren<PlayerAimAssist>();
        shakeSource = GetComponent<CinemachineImpulseSource>();
        currentPushForce = pushForce;
        defaultForcePushTriggerSize = forcePushTriggerCollider.size;
        manaPoints = totalManaPoints;
        healthPoints = totalHealthPoints;
        //coresCount = 0;
        enemyDefeatedCount = 0;
        chargeTime = 0;
        currentButtonCooldown = 0;
        currentSlomoTriggerCooldown = 0;

        prevRotationAngle = 0;
        revolutionCount = 0;
        currentRevolutionCooldown = revolutionCooldown;

        sprintTimer = sprintCountdown;

        launchedEnemies = new List<Enemy>();

        isCharging = false;
        isFalling = false;
        isHoleSlomo = false;
        isChargeMana = true;
        mainDirectionalLight = GameObject.FindGameObjectWithTag("MainLight");

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", 0);

        activePowerUps = new List<BasePowerupBehaviour>();

        sprintingSoundEvent = FMODUnity.RuntimeManager.CreateInstance(AudioManager.Instance.PlayerSprinting);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sprintingSoundEvent, transform, GetComponent<Rigidbody>());
        
        slomoSoundEvent = FMODUnity.RuntimeManager.CreateInstance(AudioManager.Instance.PlayerSlomoEnter);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(slomoSoundEvent, transform, GetComponent<Rigidbody>());
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(forcePushTriggerCollider.center, forcePushTriggerCollider.size);
    }
    void FixedUpdate()
    {
        if (enableControlls)
        {
            float xMove = 0, zMove = 0;
            float totalMoveSpeed = movementSpeed;
            if (enableMovement)
            {
                xMove = Input.GetAxis("HorizontalMove");
                zMove = Input.GetAxis("VerticalMove");

                if (xMove == 0 && zMove == 0)
                {
                    xMove = Input.GetAxis("Horizontal");
                    zMove = Input.GetAxis("Vertical");

                    sprintTimer = sprintCountdown;
                    isSprinting = false;
                }

                sprintTimer -= Time.deltaTime;
                if (sprintTimer <= 0)
                {
                    //start sprinting
                    totalMoveSpeed *= sprintSpeedMul;
                    isSprinting = true;
                    //sprintingSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    sprintingSoundEvent.setParameterByName("Skate_Exit", 0);
                    sprintingSoundEvent.start();
                }
                else
                {
                    isSprinting = false;
                    totalMoveSpeed = movementSpeed;
                    sprintingSoundEvent.setParameterByName("Skate_Exit", 1);

                }
            }

            Vector3 rotation = this.visualsHolder.forward;

            Vector2 moveAnimDirection = GetMovementDirection(new Vector2(xMove, zMove), new Vector2(rotation.x, rotation.z));
            if (float.IsNaN(moveAnimDirection.x) || float.IsNaN(moveAnimDirection.y))
                moveAnimDirection = Vector2.zero;

            float targetX = Mathf.Lerp(animator.GetFloat("MoveX"), moveAnimDirection.x, 0.3f);
            float targetY = Mathf.Lerp(animator.GetFloat("MoveY"), moveAnimDirection.y, 0.3f);

            //print(animator.GetFloat("MoveX") +" , "+ animator.GetFloat("MoveY") + "  "+ moveAnimDirection.x + "  " + moveAnimDirection.y);
            animator.SetFloat("MoveX", targetX);
            animator.SetFloat("MoveY", targetY);

           
            Vector3 playerAimRotation = Vector3.right * -Input.GetAxisRaw("HorizontalLook") + Vector3.forward * Input.GetAxisRaw("VerticalLook");
            Vector3 playerDefaultRotation = Vector3.right * -xMove + Vector3.forward * -zMove;

            if (Input.GetAxis("AltVLook") != 0 || Input.GetAxis("AltHLook") != 0)
                playerAimRotation = Vector3.right * -Input.GetAxis("AltHLook") + Vector3.forward * (Input.GetAxis("AltVLook"));

            if (playerAimRotation.sqrMagnitude > 0.0f)
            {
                this.visualsHolder.rotation = Quaternion.Slerp(visualsHolder.rotation, Quaternion.LookRotation(playerAimRotation, Vector3.up), 0.25f);

                StopSprinting();
                totalMoveSpeed = movementSpeed;
            }
            else if (playerDefaultRotation.sqrMagnitude > 0.0f)
            {
                this.visualsHolder.rotation = Quaternion.LookRotation(playerDefaultRotation, Vector3.up);
            }

            animator.SetBool("Sprinting", isSprinting);
            TrailsEnabled(isSprinting);
            transform.Translate(xMove * totalMoveSpeed, 0, zMove * totalMoveSpeed);

            if (ControllerInputDevice.GetDashButtonDown())
            {
                if (manaPoints >= dashManaCost && enableDash)
                {
                    manaPoints -= dashManaCost;

                    Vector3 dashDir = new Vector3(xMove, 0, zMove).normalized;
                    if (dashDir == Vector3.zero)
                        dashDir = this.visualsHolder.forward * -1;
                    print("DASH! dir: " + dashDir);

                    //AudioManager.Instance.PlayEffect(soundEffectSource, 3);
                    FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerDash, transform.position);

                    animator.SetTrigger("Dash");

                    //Vector3 rotation = this.visualsHolder.forward;//this.visualsHolder.rotation.eulerAngles.normalized * -1;
                    print("dash vis dir: " + rotation);

                    animator.SetFloat("DashX", moveAnimDirection.x);
                    animator.SetFloat("DashY", moveAnimDirection.y);

                    print("dashAnimDirection: " + moveAnimDirection);

                    StartCoroutine(DashCoroutine(dashDir, dashDuration));
                }
                else
                    LowMana();

            }

            if (ControllerInputDevice.GetSpecialButtonDown())
            {
                if (coresCount >= shockwaveCoreCost)
                {
                    coresCount -= shockwaveCoreCost;
                    UIManager.Instance.SetCoreCount(coresCount);
                    print("SHOCKWAVE!");
                    animator.SetTrigger("Shockwave");
                    FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerShockwave, transform.position);
                    TrailsEnabled(true);

                    isInvinsible = true;
                    enableControlls = false;
                    EnemyManager.Instance.isUpdateEnemies = false;
                }
                else
                    FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerLowEnergy, transform.position);
            }

            if (ControllerInputDevice.GetHeavyButtonDown())
            {
                print("heavy button down");
                //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                //if (stateInfo.IsName("Force_Push_Right_3"))
                {
                    if (manaPoints >= somersaultManaCost)
                    {
                        animator.SetTrigger("HeavyA");
                        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerSomersault, transform.position);
                    }
                    else
                        LowMana();
                }
            }

            if (ControllerInputDevice.GetLeftTriggerDown())
            {
                /*Enemy e = EnemyManager.Instance.GetStunnedEnemy();
                if (e != null)
                {
                    Point p = e.GetPointPos();
                    BaseTileBehaviour tile = gridHolder.GetGridNode(p.x, p.y).GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>();
                    tile?.Popup();
                    animator.SetTrigger("Launch");
                    launchedEnemy = e;
                    enableMovement = false;
                }*/
                List<Enemy> enemies = EnemyManager.Instance.GetStunnedEnemiesByDistance(6, transform.position);
                if (enemies.Count > 0)
                {
                    animator.SetTrigger("Launch");
                    enableMovement = false;
                    foreach (Enemy e in enemies)
                    {
                        Point p = e.GetPointPos();
                        BaseTileBehaviour tile = gridHolder.GetGridNode(p.x, p.y).GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>();
                        tile?.Popup();
                        launchedEnemies.Add(e);
                    }
                }
                else
                {
                    //animator.SetTrigger("Launch");
                    print("# finisher button down but no stunned enemies.");
                }
            }

            if (ControllerInputDevice.GetCompanionButtonDown())
            {
                print("alex buttom pressed");
                List<Enemy> launchedEnemies = EnemyManager.Instance.GetLaunchedEnemies();
                foreach (Enemy enemy in launchedEnemies)
                {
                    Point p = enemy.GetPointPos();
                    MakeFinisherHole(p.x, p.y);
                }
            }

            /*if (ControllerInputDevice.GetChargeButtonDown())
            {
                isCharging = true;
                chargeTime += Time.deltaTime;
                animator.SetBool("ChargeA", isCharging);
                //print("charging..");
            }

            if (!ControllerInputDevice.GetChargeButtonDown() && isCharging)
            {
                if (chargeTime >= chargeTimeAttract && manaPoints >= chargeCost)
                    AttractAttackAction();
                else if (chargeTime >= chargeTimeRepel && manaPoints >= chargeCost)
                    RepelAttackAction();
                else
                    LowMana();
                chargeTime = 0;
                isCharging = false;
                animator.SetBool("ChargeA", isCharging);
            }*/
        }
        DetectPlayerPositionOnGrid();


        if (transform.position.y < -0.2f)
        {
            FellIntoAPit();
            StopSprinting();
        }

    }

    public void TrailsEnabled(bool enabled)
    {
        foreach (var go in trails)
        {
            go.SetActive(enabled);
        }
    }

    private void StopSprinting()
    {
        sprintTimer = sprintCountdown;
        isSprinting = false;
        animator.SetBool("Sprinting", false);
        TrailsEnabled(false);
    }

    private void LowMana()
    {
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerLowEnergy, transform.position);

    }

    public void PreformLaunch()
    { 
        foreach(Enemy e in launchedEnemies)
            e.Launch();
        //GameManager.Instance.DashSlomo(2f);

        shakeSource.GenerateImpulse();

        launchedEnemies.Clear();
        //enableControlls = true;
        enableMovement = true;
    }

    private void RepelAttackAction()
    {
        animator.SetTrigger("AOERepel");
        manaPoints -= chargeCost;
    }

    public void PreformRepelAttack()
    {
        print("aaaa");
        shockwaveBehavior.gameObject.SetActive(true);
        currentPushForce *= 1.3f;
        StartCoroutine(shockwaveBehavior.Shockwave(4, false));
    }

    public void FinishRepelAttack()
    {
        enableControlls = true;
        currentPushForce = pushForce;
    }

    private void AttractAttackAction()
    {
        animator.SetTrigger("AOEAttract");
        manaPoints -= chargeCost;

    }

    public void PreformShockwave()
    {
        EnemyManager.Instance.isUpdateEnemies = true;

        //GameManager.Instance.ShockwaveSlomo(8);
        //CameraShaker.Instance.ShakeOnce(2f, 8f, 0.1f, 2.5f);
        shakeSource.GenerateImpulse();

        shockwaveBehavior.gameObject.SetActive(true);
        TrailsEnabled(true);
        StartCoroutine(shockwaveBehavior.Shockwave(10, true));
    }

    public void PreformSomersault()
    {
        isDoingSomersault = true;
        manaPoints -= somersaultManaCost;
        currentPushForce *= 1.5f;
        enableControlls = false;
        ObjectPooler.Instance.SpawnFromPool("SomersaultEffect", transform.position, visualsHolder.rotation);
        TrailsEnabled(true);

        //FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerSomersault, transform.position);
    }

    public void FinishSomersault()
    {
        currentPushForce = pushForce;
        enableControlls = true;
        isDoingSomersault = false;
        TrailsEnabled(false);
    }

    private IEnumerator DashCoroutine(Vector3 direction, float duration)
    {
        lastDashDir = direction;
        isDashing = true;

        dashEffect.Play();

        float time = duration;

        float capsuleR = capsuleCollider.radius;
        capsuleCollider.radius *= 3.5f;
        //StartCoroutine(DebugSlomo());

        while (time > 0 && enableDash && isDashing)
        {
            time -= Time.deltaTime;
            enableControlls = false;
            transform.Translate(direction * movementSpeed * dashSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        capsuleCollider.radius = capsuleR;
        isDashing = false;
        if (isDashImpact)
        {
            transform.position = dashImpactEnemyLocation;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            yield return new WaitForSeconds(0.3f);
            //transform.Translate(direction * movementSpeed * dashSpeed * Time.deltaTime);

            isDashImpact = false;
        }
        if(!isDead)
            enableControlls = true;
    }

    public void PreformPush(float pushRadiusMul, float effectTime, float floorEffectLength, float pushRadiusWidth = 2)
    {
        //StartCoroutine(DebugSlomo());
        enableMovement = true;
        manaPoints -= pushManaCost;
        
        Vector3 size = new Vector3(forcePushTriggerCollider.size.x + pushRadiusWidth, forcePushTriggerCollider.size.y, forcePushTriggerCollider.size.z + pushRadius * pushRadiusMul);
        Vector3 center = new Vector3(0, 0, -pushRadius * pushRadiusMul / 2);
        StartCoroutine(OverridePushCollider(0.1f, size, center));

        //StartCoroutine(ShowForcePushEffect(effectTime));

        forcePushEffect.SetFloat("ForceMultiplier", pushRadiusMul);
        if (pushRadiusMul <= 1.7f) //exclude Somersault 
            forcePushEffect.Play();

        StartCoroutine(forcePushFloorTrigger.PlayEffectCoroutine(floorEffectLength));
    }

    private IEnumerator OverridePushCollider(float time, Vector3 size, Vector3 center)
    {
        float delayTime = 0;
        while (delayTime <= time)
        {
            forcePushTriggerCollider.size = size;
            forcePushTriggerCollider.center = center;
            delayTime += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator DebugSlomo()
    {
        Time.timeScale = 0.016f;
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1f;

    }

    public void UseMana(float amount)
    {
        manaPoints -= amount;
    }

    void Update()
    {
        ManipulateFloor();
        RegenMana();

        forcePushTriggerCollider.center = Vector3.zero;
        forcePushTriggerCollider.size = defaultForcePushTriggerSize;

        if (enableControlls)
        {
            currentButtonCooldown -= Time.deltaTime;
            if ((Input.GetMouseButtonDown(1) || ControllerInputDevice.GetRightButtonDown()) && currentButtonCooldown <= 0)
            {
                currentButtonCooldown = buttonPressCooldown;
                if (manaPoints >= pushManaCost)
                {
                    enableMovement = false;

                    GameObject closestEnemy = aimAssist.GetClosestEnemyObject();
                    if (closestEnemy != null)
                    {
                        this.visualsHolder.LookAt(closestEnemy.transform, Vector3.up);
                        visualsHolder.Rotate(new Vector3(0, 180, 0));
                    }

                    //the animation triggers PreformPush()
                    animator.SetTrigger("PushA");
                    StopSprinting();
                }
                else
                    LowMana();
            }

            float newAngle = Mathf.Atan2(Input.GetAxisRaw("VerticalLook"), Input.GetAxisRaw("HorizontalLook")) * Mathf.Rad2Deg;

            float angleDifference = newAngle - prevRotationAngle;
            if (angleDifference > 180f) angleDifference -= 360f;
            if (angleDifference < -180f) angleDifference += 360f;
            prevRotationAngle = newAngle;

            currentRevolutionCooldown -= Time.deltaTime;
            if (currentRevolutionCooldown <= 0)
            {
                currentRevolutionCooldown = revolutionCooldown;
                revolutionCount = 0;
            }

            revolutionCount += angleDifference;
            if (revolutionCount >= 340)
            {
                print("Completed rotation!");
                revolutionCount = 0;
                if (AOEManaCost <= manaPoints)
                {
                    animator.SetTrigger("AOERepel");
                    manaPoints -= AOEManaCost;
                    enableControlls = false;
                }
                else
                    LowMana();
            }
            //print(angleDifference);
        }

        BasePowerupBehaviour powerupToRemove = null;
        if (activePowerUps.Count > 0)
        {
            foreach (BasePowerupBehaviour powerUp in activePowerUps)
            {
                powerUp.effectTime -= Time.deltaTime;
                //print(powerUp.powerUpName + " time: " + powerUp.effectTime);
                if (powerUp.effectTime <= 0)
                {
                    powerupToRemove = powerUp;
                    break;
                }
            }
        }
        if (powerupToRemove != null)
        {
            print("removing powerup "+ powerupToRemove.powerUpName);
            activePowerUps.Remove(powerupToRemove);
            RemovePowerupBonus(powerupToRemove);
        }

    }

    /*public void PlayCorrespondingPushSound()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        FMOD.Studio.EventInstance pushSound = FMODUnity.RuntimeManager.CreateInstance(AudioManager.Instance.PlayerPush);
        pushSound.setParameterByID(pushParameterId, stateInfo.IsName("Force_Push_Right_3") ? 1.0f : 0.0f);
        pushSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        pushSound.start();
        pushSound.release();
    }*/

    /*private IEnumerator ShowForcePushEffect(float duration)
    {
        forcePushEffect.SetActive(true);
        yield return new WaitForSeconds(duration);
        forcePushEffect.SetActive(false);
    }*/

    public void AddPowerup(BasePowerupBehaviour powerUp)
    {
        Debug.Log("Picked up: "+powerUp.name);
        animator.SetTrigger("PowerUp");

        switch (powerUp.type)
        {
            case PowerUpType.Health:

                healthPoints += powerUp.bonus;

                if (healthPoints > totalHealthPoints)
                    healthPoints = totalHealthPoints;
                if (healthPoints > 2)
                    GameManager.Instance.cameraRef.SetLowHealth(false);
                if(powerUp.bonus == 1) // TODO: change this to be dynamic somehow
                    FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerItemPickupHP1, transform.position);
                else
                    FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerItemPickupHP2, transform.position);


                UIManager.Instance.SetHealth(healthPoints / totalHealthPoints);
                break;
            case PowerUpType.Core:
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerItemPickupCore, transform.position);

                coresCount += powerUp.count;
                UIManager.Instance.SetCoreCount(coresCount);
                break;
        }

        if (powerUp.type != PowerUpType.Health && powerUp.type != PowerUpType.Core)
        {
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerItemPickup, transform.position);

            if (activePowerUps.Count == 0 || !activePowerUps.Exists(p => p.powerUpName == powerUp.powerUpName))
            {
                activePowerUps.Add(powerUp);
                UIManager.Instance.AddPowerup(powerUp.powerUpData);
                switch (powerUp.type)
                {
                    case PowerUpType.PushForceBoost:
                        pushForce += powerUp.bonus;
                        currentPushForce = pushForce;
                        break;
                    case PowerUpType.RegenBoost:
                        manaRegenAmount += powerUp.bonus;
                        break;
                    case PowerUpType.PushRangeBoost:
                        pushRadius += powerUp.bonus;
                        break;
                    case PowerUpType.MoveSpeedBoost:
                        movementSpeed += powerUp.bonus;
                        break;
                    case PowerUpType.DashBoost:
                        dashSpeed += powerUp.bonus;
                        break;
                }
            }
            else
            {
                BasePowerupBehaviour pu = activePowerUps.Find(p => p.powerUpName == powerUp.powerUpName);
                pu.effectTime += powerUp.effectTime;
                UIManager.Instance.UpdatePowerupTimer(powerUp.powerUpData, pu.effectTime);
            }
        }
    }

    private void RemovePowerupBonus(BasePowerupBehaviour powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.PushForceBoost:
                pushForce -= powerUp.bonus;
                currentPushForce = pushForce;
                break;
            case PowerUpType.RegenBoost:
                manaRegenAmount -= powerUp.bonus;
                break;
            case PowerUpType.PushRangeBoost:
                pushRadius -= powerUp.bonus;
                break;
            case PowerUpType.MoveSpeedBoost:
                movementSpeed -= powerUp.bonus;
                break;
            case PowerUpType.DashBoost:
                dashSpeed -= powerUp.bonus;
                break;
        }
        Destroy(powerUp.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (isDashing)
            {
                isDashing = false;
                isDashImpact = true;
                dashImpactEnemyLocation = enemy.transform.position;
                enemy.ForcePush(lastDashDir, currentPushForce * 1.5f, PlayerAttackType.Dash, true);

                GameManager.Instance.DashSlomo(2f);
                GameManager.Instance.cameraRef.FastZoom(enemy.transform);
                ObjectPooler.Instance.SpawnFromPool("HitEffect", enemy.transform.position, Quaternion.identity);
            }
            else
            {
                TakeDamage(enemy.damage);
            }
        }
        if (collision.gameObject.CompareTag("GoalCube"))
        {
            //GameManager.Instance.NextLevel();
            GameManager.Instance.LevelFinished();
            gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("WallCube") || collision.gameObject.CompareTag("GateCube"))
        {
            enableDash = false;
        }
        if (isFalling && (collision.gameObject.CompareTag("FloorCube")|| collision.gameObject.CompareTag("CheckpointCube")))
        {
            shockwaveBehavior.gameObject.SetActive(true);
            StartCoroutine(shockwaveBehavior.Shockwave(3, false));
            isFalling = false;
            animator.SetBool("Falling", isFalling);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("WallCube") || collision.gameObject.CompareTag("GateCube"))
        {
            enableDash = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckpointCube"))
        {
            print("# setting new checkpoint: "+other.name);
            SetCheckpoint(other.transform);
        }
        /*if (other.CompareTag("EnemyPulseTrigger"))
        {
            Enemy enemy = other.transform.parent.GetComponent<Enemy>();
            TakeDamage(enemy.damage);
            print("taken damage from pulse");
        }*/
    }


    public void TakeDamage(float damage)
    {
        StopSprinting();
        currentPushForce = pushForce;

        if (Time.time - lastTimeDamageTaken > takenDamageCooldown && !isInvinsible)
        {
            enableControlls = true;
            lastTimeDamageTaken = Time.time;

            healthPoints -= damage;
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerTakeDamage, transform.position);
            GameManager.Instance.TakeDamage();

            //GameManager.Instance.SetScoreMultiplier(1);
            GameManager.Instance.ResetKillPointsAndMultiplier();
            GameManager.Instance.AddDamageCount(damage);
            UIManager.Instance.SetHealth(healthPoints / totalHealthPoints);
            if (healthPoints <= 0)
            {
                //GameManager.Instance.GameOver();
                isDead = true;
                EnemyManager.Instance.isUpdateEnemies = false;
                animator.SetTrigger("Dead");
                FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerDeath, transform.position);

                enableControlls = false;
                isInvinsible = true;
                
                spotlight.SetActive(true);
                mainDirectionalLight.SetActive(false);
                GameManager.Instance.cameraRef.SetLowHealth(false);
            }
            else if (healthPoints <= 2)
            {
                GameManager.Instance.cameraRef.SetLowHealth(true);
                animator.SetTrigger("TakeDamage");
            }
            else
            {
                animator.SetTrigger("TakeDamage");
            }

            TrailsEnabled(false);
            GameManager.Instance.cameraRef.GlitchScreen();
        }
    }

    public void SetCheckpoint(Transform checkpointTransform)
    {
        this.checkpoint = checkpointTransform;
    }

    private void FellIntoAPit()
    {
        if (!isFalling)
        {
            enableControlls = false;
            isFalling = true;
            EnemyManager.Instance.isUpdateEnemies = false;
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerFall, transform.position);

            //Transform respawnPoint = prevHoveredObject.transform;
            //transform.position = new Vector3(respawnPoint.position.x, 10, respawnPoint.position.z);
            TrailsEnabled(true);

            StartCoroutine(WaitToRecover());
        }
    }

    private IEnumerator WaitToRecover()
    {
        animator.SetBool("Falling", isFalling);
        //enableDash = false; 
        enableControlls = false;

        Vector3 soundPos = new Vector3(transform.position.x, 0, transform.position.z);
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerFallLand, soundPos);

        yield return new WaitForSeconds(1);
        TakeDamage(fallDamage);
        enableControlls = false; //taking damage if not dead can enable controls back

        if (isDead)
            yield break;

        isInvinsible = true;

        Vector3 retryPostion;
        if (checkpoint != null)
            retryPostion = new Vector3(checkpoint.position.x, 10, checkpoint.position.z);
        else
            retryPostion = new Vector3(spawnPosition.x, 10, spawnPosition.z);
        transform.position = retryPostion;

        yield return new WaitForSeconds(4f);
        TrailsEnabled(false);

        isInvinsible = false;
        enableControlls = true;
        EnemyManager.Instance.isUpdateEnemies = true;
    }

    private void RegenMana()
    {
        if (manaPoints < totalManaPoints)
        {
            if(isChargeMana)
                manaPoints += Time.deltaTime * manaRegenAmount;
            UIManager.Instance.SetMana(manaPoints / totalManaPoints);
        }
    }

    private void ManipulateFloor()
    {
        if (prevHoveredObject != null)
        {
            if (prevHoveredObject.tag != "WeakCube")
            {
                prevHoveredObject.GetComponent<BaseTileBehaviour>().DeselectPillar();

                /*if (prevHoveredObject.tag == "WallCube")
                    prevHoveredObject.GetComponent<Renderer>().material.SetColor("_Color", tileWallOriginalColor);
                else
                    prevHoveredObject.GetComponent<BaseTileBehaviour>().DeselectPillar();*/
                    //prevHoveredObject.GetComponent<Renderer>().material.SetColor("_Color", tileOriginalColor);
            }
        }

        if (currHoveredObject != null)
            if (currHoveredObject.tag != "WeakCube")
                //currHoveredObject.GetComponent<Renderer>().material.SetColor("_Color", tileHighlightColor);
                if(isHoleSlomo)
                    currHoveredObject.GetComponent<BaseTileBehaviour>().SelectPillar();

        //make a hole
        /*if (Input.GetMouseButtonDown(0) || ControllerInputDevice.GetLeftTriggerDown())
        {
            TriggerMakeHoleAction();
        }*/

        // make a hole v2.0
        if ((ControllerInputDevice.GetLeftButtonUp() && isHoleSlomo) || ( manaPoints <= holeManaCost && isHoleSlomo))
        {
            //stop slomo and make hole
            isHoleSlomo = false;
            isChargeMana = true;
            GameManager.Instance.EndSlomo();
            TriggerMakeHoleAction();
            FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerSlomoExit, transform.position);
            slomoSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            print("ending hole slomo");
        }
        if (ControllerInputDevice.GetLeftButtonDown() && !isHoleSlomo && manaPoints >= holeManaCost) // add back mouse support
        {
            //start slomo
            currentSlomoTriggerCooldown -= Time.deltaTime;
            if (currentSlomoTriggerCooldown <= 0)
            {
                isHoleSlomo = true;
                GameManager.Instance.SetSlomo(0.1f);
                slomoSoundEvent.start();
                currentSlomoTriggerCooldown = slomoTriggerCooldown;
                print("starting hole slomo");
            }

        }
        if (isHoleSlomo)
        {
            isChargeMana = false;
            manaPoints -= slomoFrameCost;
            if (manaPoints <= 0)
            {
                isHoleSlomo = false;
                isChargeMana = true;
                currentSlomoTriggerCooldown = slomoTriggerCooldown;
                GameManager.Instance.EndSlomo();
            }
        }
    }

    private void TriggerMakeHoleAction()
    {
        if (manaPoints >= holeManaCost)
        {
            Transform tileTransform = currHoveredObject.transform;
            string name = tileTransform.parent.name;
            Debug.Log("pressed on grid cube: " + name);

            GridNode node = gridHolder.GetGridNode(name);
            if (node.GetTileType() != TileType.Occupied && node.GetTileType() != TileType.Pit && node.GetTileType() != TileType.PlayerPit && node.GetTileType() != TileType.EnemyPit)
            {
                if (EnemyManager.Instance.GetLaunchedEnemies().Count == 0)
                    animator.SetTrigger("HoleA");
                else
                {
                    Vector3 holePos = node.GetGameNodeRef().transform.position;
                    StartCoroutine(EnemyManager.Instance.SendLaunchedEnemiesIntoHole(holePos));
                    animator.SetTrigger("FinisherA");
                    isInvinsible = true;
                    enableControlls = false;
                    MakeHole();
                    StartCoroutine(GameManager.Instance.TriggerFinisher(node.GetGameNodeRef().transform,2f));
                }
            }
            else
            {
                print("Pressed on occupied tile! tile: " + name);
            }
        }
    }

    public void MakeHole()
    {
        Transform tileTransform = currHoveredObject.transform;
        
        //move the tile down
        tileTransform.GetComponent<BaseTileBehaviour>().Drop();
        string name = tileTransform.parent.name;

        var location = gridHolder.GetNodeLocation(name);
        GridNode node = gridHolder.GetGridNode(location[0], location[1]);

        //set the selected node as a hole
        gridHolder.SetGridNodeType(node, TileType.PlayerPit, holeTimeToRegen);

        //set adjacent available nodes as holes
        GridNode up = null, down = null, left = null, right = null;
        try { up = gridHolder.GetGridNode(location[0], location[1]+1); }
        catch (IndexOutOfRangeException e) { }
        try { down = gridHolder.GetGridNode(location[0], location[1]-1); }
        catch (IndexOutOfRangeException e) { }
        try { left = gridHolder.GetGridNode(location[0]-1, location[1]); }
        catch (IndexOutOfRangeException e) { }
        try { right = gridHolder.GetGridNode(location[0]+1, location[1]); }
        catch (IndexOutOfRangeException e) { }

        if (up != null && up.GetTileType() == TileType.Normal)
        {
            up.GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>().Drop();
            gridHolder.SetGridNodeType(up, TileType.PlayerPit, holeTimeToRegen);
        }
        if (down != null && down.GetTileType() == TileType.Normal)
        {
            down.GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>().Drop();
            gridHolder.SetGridNodeType(down, TileType.PlayerPit, holeTimeToRegen);
        }
        if (left != null && left.GetTileType() == TileType.Normal)
        {
            left.GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>().Drop();
            gridHolder.SetGridNodeType(left, TileType.PlayerPit, holeTimeToRegen);
        }
        if (right != null && right.GetTileType() == TileType.Normal)
        {
            right.GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>().Drop();
            gridHolder.SetGridNodeType(right, TileType.PlayerPit, holeTimeToRegen);
        }

        manaPoints -= holeManaCost;

        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerMakeHole, transform.position);
    }

    public void MakeFinisherHole(int x, int y)
    {
        GridNode node = gridHolder.GetGridNode(x, y);
        node.GetGameNodeRef().GetComponentInChildren<BaseTileBehaviour>().Drop();
        gridHolder.SetGridNodeType(node, TileType.PlayerPit, holeTimeToRegen);
        
        FMODUnity.RuntimeManager.PlayOneShot(AudioManager.Instance.PlayerMakeHole, transform.position);

        manaPoints = 0;
    }

    public void FinishFinisher()
    {
        isInvinsible = false;
        enableControlls = true;
    }

    private void DetectPlayerPositionOnGrid()
    {
        //Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.tag == "FloorCube" || hit.transform.tag == "WeakCube")
            {
                string name = hit.transform.parent.name;
                string[] posArr = name.Split(',');

                Point pPoint = GameManager.Instance.playerPointPosition;

                if (pPoint != null)
                {
                    GridNode gNode = gridHolder.GetGridNode(pPoint.x, pPoint.y);
                    if (gNode.GetTileType() == TileType.Occupied)
                        gNode.SetType(TileType.Normal);
                }

                GameManager.Instance.playerPointPosition = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));
                pPoint = GameManager.Instance.playerPointPosition;

                if (pPoint != null)
                {
                    GridNode gNode = gridHolder.GetGridNode(pPoint.x, pPoint.y);

                    if (gNode.GetTileType() == TileType.Weak)
                    {
                        gNode.GetGameNodeRef().GetComponentInChildren<WeakTileBehaviour>().StepOnTile(()=> gNode.SetType(TileType.Pit));
                    }

                    else if (gNode.GetTileType() != TileType.Pit)
                    {
                        gridHolder.SetGridNodeType(pPoint.x, pPoint.y, TileType.Occupied);
                        timeOverPit = 0;
                    }

                    else if (gNode.GetTileType() == TileType.Pit)
                    {
                        timeOverPit += Time.deltaTime;
                        if (!isDashing && timeOverPit >= 0.3f)
                            FellIntoAPit();
                    }
                }
            }
        }
    }

    private Vector2 GetMovementDirection(Vector2 moveDir, Vector2 lookDir)
    {
        float angleDiff = GetAngle(moveDir.x, moveDir.y) - GetAngle(lookDir.x, lookDir.y)-90;
        return new Vector2(Mathf.Cos(angleDiff), Mathf.Sin(angleDiff));
    }

    float GetAngle(float x, float y)
    {
        float angle = Mathf.Atan(y / x);
        if (x < 0)
            angle = angle + Mathf.PI;
        return angle;
    }

    float AngleFromJoystick(float x, float y)
    {
        if (x != 0.0f || y != 0.0f)
        {
            return Mathf.Atan2(y, x) * Mathf.Rad2Deg; // flip x and y for 90 deg result
        }
        return 0;
    }

    float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return -Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
