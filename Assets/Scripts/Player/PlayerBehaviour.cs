using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int shockwaveCoreCost;
    public float dashDuration;
    public float dashSpeed;
    public float fallDamage = 1;
    public float takenDamageCooldown;

    public int coresCount;
    public int enemyDefeatedCount;

    public AudioSource soundEffectSource;

    private List<BasePowerupBehaviour> activePowerUps;

    private Transform checkpoint;
    [HideInInspector] public Vector3 spawnPosition;

    private RaycastHit hit;
    private Ray ray;

    private float lastTimeDamageTaken=0;
    private bool enableDash = true;

    [HideInInspector] public GameObject prevHoveredObject;
    [HideInInspector] public GameObject currHoveredObject;

    [HideInInspector] public bool enableControlls = true;

    private float pushRadius = 3.5f;
    //private float defaultRadius = 0.35f;
    private Vector3 defaultForcePushTriggerSize;

    public float pushForce;
    public BoxCollider forcePushTriggerCollider;
    public GameObject forcePushEffect;

    public LevelGenerator gridHolder;
    public Transform visualsHolder;

    public PlayerShockwaveBehavior shockwaveBehavior;
    public ForcePushFloorTrigger forcePushFloorTrigger;

    public Animator animator;

    public bool IsTestMode = false;

    void Start()
    {
        if(!IsTestMode)
            gridHolder = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();

        defaultForcePushTriggerSize = forcePushTriggerCollider.size;
        manaPoints = totalManaPoints;
        healthPoints = totalHealthPoints;
        coresCount = 0;
        enemyDefeatedCount = 0;
        activePowerUps = new List<BasePowerupBehaviour>();
    }

    void FixedUpdate()
    {
        if (enableControlls)
        {
            float xMove = Input.GetAxis("HorizontalMove");
            float yMove = Input.GetAxis("VerticalMove");

            if (xMove == 0 && yMove == 0)
            {
                xMove = Input.GetAxis("Horizontal");
                yMove = Input.GetAxis("Vertical");
            }
            transform.Translate(xMove * movementSpeed, 0, yMove * movementSpeed);

            /*
            float xLook = Input.GetAxis("HorizontalLook");
            float yLook = Input.GetAxis("VerticalLook");
            float angle = AngleFromJoystick(xLook, yLook);

            //Debug.LogFormat("x: {0} y: {1} angle: {2}",xLook, yLook, angle);

            //rotate only if there is an input
            if (xLook != 0 || yLook != 0)
                this.visualsHolder.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);
                */

            Vector3 playerRotation = Vector3.right * -Input.GetAxisRaw("HorizontalLook") + Vector3.forward * Input.GetAxisRaw("VerticalLook");

            if (Input.GetAxis("AltVLook") != 0 || Input.GetAxis("AltHLook") != 0)
                playerRotation = Vector3.right * -Input.GetAxis("AltHLook") + Vector3.forward * (Input.GetAxis("AltVLook"));

            if (playerRotation.sqrMagnitude > 0.0f)
            {
                this.visualsHolder.rotation = Quaternion.LookRotation(playerRotation, Vector3.up);
            }

            if (ControllerInputDevice.GetDashButtonDown())
            {
                if (manaPoints >= dashManaCost && enableDash)
                {
                    manaPoints -= dashManaCost;

                    Vector3 dashDir = new Vector3(xMove, 0, yMove).normalized;
                    if (dashDir == Vector3.zero)
                        dashDir = this.visualsHolder.forward *-1;//this.visualsHolder.rotation.eulerAngles.normalized;
                    print("DASH! dir: "+dashDir);
                    //AudioManager.Instance.PlayEffect(soundEffectSource, 3);
                    StartCoroutine(DashCoroutine(dashDir, dashDuration));
                }
            }

            if (ControllerInputDevice.GetSpecialButtonDown())
            {
                if (coresCount >= shockwaveCoreCost)
                {
                    coresCount -= shockwaveCoreCost;
                    UIManager.Instance.SetCoreCount(coresCount);
                    print("SHOCKWAVE!");
                    CameraShaker.Instance.ShakeOnce(2f, 8f, 0.1f, 2.5f);
                    shockwaveBehavior.gameObject.SetActive(true);
                    StartCoroutine(shockwaveBehavior.Shockwave(10));
                }
            }
        }
        DetectPlayerPositionOnGrid();


        if (transform.position.y < -0.2f)
        {
            FellIntoAPit();
        }

       /* Vector2 mousepos = Input.mousePosition;
        Vector2 screenCenter = Camera.main.WorldToScreenPoint(this.transform.position);

        float angle = AngleBetweenTwoPoints(screenCenter, mousepos) + 180;

        this.visualsHolder.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);
        */
    }

    private IEnumerator DashCoroutine(Vector3 direction, float duration)
    {
        float time = duration;
        while (time > 0 && enableDash)
        {
            time -= Time.deltaTime;
            enableControlls = false;
            transform.Translate(direction * movementSpeed * dashSpeed);
            yield return null;
        }
        enableControlls = true;
    }

    void Update()
    {
        ManipulateFloor();
        RegenMana();

        forcePushTriggerCollider.center = Vector3.zero;
        forcePushTriggerCollider.size = defaultForcePushTriggerSize;

        if (enableControlls)
        {
            if (Input.GetMouseButtonDown(1) || ControllerInputDevice.GetRightTriggerDown())
            {
                if (manaPoints >= pushManaCost)
                {
                    forcePushTriggerCollider.size = new Vector3(forcePushTriggerCollider.size.x + 1, forcePushTriggerCollider.size.y, forcePushTriggerCollider.size.z + pushRadius);
                    forcePushTriggerCollider.center = new Vector3(0, 0, -pushRadius / 2);
                    manaPoints -= pushManaCost;

                    animator.SetTrigger("PushA");
                    StartCoroutine(ShowForcePushEffect(0.1f));
                    StartCoroutine(forcePushFloorTrigger.PlayEffectCoroutine(0.1f));
                }
            }
        }

        BasePowerupBehaviour powerupToRemove = null;
        if (activePowerUps.Count > 0)
        {
            foreach (BasePowerupBehaviour powerUp in activePowerUps)
            {
                powerUp.effectTime -= Time.deltaTime;
                print(powerUp.powerUpName + " time: " + powerUp.effectTime);
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

    private IEnumerator ShowForcePushEffect(float duration)
    {
        forcePushEffect.SetActive(true);
        yield return new WaitForSeconds(duration);
        forcePushEffect.SetActive(false);
    }

    public void AddPowerup(BasePowerupBehaviour powerUp)
    {
        Debug.Log("Picked up: "+powerUp.name);
        switch (powerUp.type)
        {
            case PowerUpType.Health:
                healthPoints += powerUp.bonus;
                if (healthPoints > totalHealthPoints)
                    healthPoints = totalHealthPoints;
                UIManager.Instance.SetHealth(healthPoints / totalHealthPoints);
                break;
            case PowerUpType.Core:
                coresCount += powerUp.count;
                UIManager.Instance.SetCoreCount(coresCount);
                break;
        }

        if (powerUp.type != PowerUpType.Health && powerUp.type != PowerUpType.Core)
        {
            if (activePowerUps.Count == 0 || !activePowerUps.Exists(p => p.powerUpName == powerUp.powerUpName))
            {
                activePowerUps.Add(powerUp);
                switch (powerUp.type)
                {
                    case PowerUpType.PushForceBoost:
                        pushForce += powerUp.bonus;
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
            }
        }
    }

    private void RemovePowerupBonus(BasePowerupBehaviour powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.PushForceBoost:
                pushForce -= powerUp.bonus;
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
            TakeDamage(enemy.damage);
        }
        if (collision.gameObject.CompareTag("GoalCube"))
        {
            GameManager.Instance.NextLevel();
            gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("WallCube") || collision.gameObject.CompareTag("GateCube"))
        {
            enableDash = false;
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
        if (Time.time - lastTimeDamageTaken > takenDamageCooldown)
        {
            lastTimeDamageTaken = Time.time;

            healthPoints -= damage;

            GameManager.Instance.SetScoreMultiplier(1);
            UIManager.Instance.SetHealth(healthPoints / totalHealthPoints);
            if (healthPoints <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    public void SetCheckpoint(Transform checkpointTransform)
    {
        this.checkpoint = checkpointTransform;
    }

    private void FellIntoAPit()
    {
        enableControlls = false;

        //Transform respawnPoint = prevHoveredObject.transform;
        //transform.position = new Vector3(respawnPoint.position.x, 10, respawnPoint.position.z);
        if(checkpoint != null)
            transform.position = new Vector3(checkpoint.position.x, 10, checkpoint.position.z);
        else
            transform.position = new Vector3(spawnPosition.x, 10, spawnPosition.z);
        TakeDamage(fallDamage);
        StartCoroutine(WaitToRecover());
    }

    private IEnumerator WaitToRecover()
    {
        //SphereCollider sc = GetComponent<SphereCollider>();
        yield return new WaitForSeconds(1);
        enableControlls = true;
    }

    private void RegenMana()
    {
        if (manaPoints < totalManaPoints)
        {
            manaPoints += Time.deltaTime * manaRegenAmount;
            UIManager.Instance.SetMana(manaPoints / totalManaPoints);
        }
    }

    private void ManipulateFloor()
    {
        if (prevHoveredObject != null)
            prevHoveredObject.GetComponent<Renderer>().material.color = Color.white;
        if (prevHoveredObject != null)
            currHoveredObject.GetComponent<Renderer>().material.color = Color.red;

        //make a hole
        if (Input.GetMouseButtonDown(0) || ControllerInputDevice.GetLeftTriggerDown())
        {
            if (manaPoints >= holeManaCost)
            {
                Transform tileTransform = currHoveredObject.transform;
                string name = tileTransform.parent.name;
                Debug.Log("pressed on grid cube: " + name);

                GridNode node = gridHolder.GetGridNode(name);
                if (node.GetTileType() != TileType.Occupied)
                {
                    //move the tile down
                    tileTransform.GetComponent<BaseTileBehaviour>().Drop();

                    gridHolder.SetGridNodeType(node, TileType.Pit, holeTimeToRegen);
                    manaPoints -= holeManaCost;
                }
                else
                {
                    print("Pressed on occupied tile! tile: " + name);
                }
            }
        }
    }

    private void DetectPlayerPositionOnGrid()
    {
        //Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.tag == "FloorCube")
            {
                string name = hit.transform.parent.name;
                string[] posArr = name.Split(',');

                Point pPoint = GameManager.Instance.playerPointPosition;

                if (pPoint != null)
                    if(gridHolder.GetGridNodeType(pPoint.x, pPoint.y) == TileType.Occupied)
                        gridHolder.SetGridNodeType(pPoint.x, pPoint.y, TileType.Normal);

                GameManager.Instance.playerPointPosition = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));
                //this is a temp if
                pPoint = GameManager.Instance.playerPointPosition;
                if (pPoint != null && gridHolder.GetGridNodeType(pPoint.x, pPoint.y) != TileType.Pit)
                    gridHolder.SetGridNodeType(pPoint.x, pPoint.y, TileType.Occupied);
            }
        }
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
