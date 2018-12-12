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
    public float fallDamage = 1;

    public int coresCount;

    private List<PowerUpObject> activePowerUps;

    private RaycastHit hit;
    private Ray ray;
    [HideInInspector] public GameObject prevHoveredObject;
    [HideInInspector] public GameObject currHoveredObject;

    [HideInInspector] public bool enableControlls = true;

    private float pushRadius = 3.5f;
    //private float defaultRadius = 0.35f;
    private Vector3 defaultForcePushTriggerSize;
    public float pushForce;
    public BoxCollider forcePushTriggerCollider;

    public LevelGenerator gridHolder;
    public Transform visualsHolder;

    public PlayerShockwaveBehavior shockwaveBehavior;

    void Start()
    {
        //forcePushTriggerCollider = GetComponent<BoxCollider>();
        gridHolder = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<LevelGenerator>();

        defaultForcePushTriggerSize = forcePushTriggerCollider.size;
        manaPoints = totalManaPoints;
        healthPoints = totalHealthPoints;
        coresCount = 0;
        activePowerUps = new List<PowerUpObject>();
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

            float xLook = Input.GetAxis("HorizontalLook");
            float yLook = Input.GetAxis("VerticalLook");
            float angle = AngleFromJoystick(xLook, yLook);

            //rotate only if there is an input
            if (angle != 0)
                this.visualsHolder.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);

            if (ControllerInputDevice.GetDashButtonDown())
            {
                if (manaPoints >= dashManaCost)
                {
                    manaPoints -= dashManaCost;

                    Vector3 dashDir = new Vector3(xMove, 0, yMove).normalized;
                    if (dashDir == Vector3.zero)
                        dashDir = this.visualsHolder.forward *-1;//this.visualsHolder.rotation.eulerAngles.normalized;
                    print("DASH! dir: "+dashDir);
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

            DetectPlayerPositionOnGrid();
        }

        if (transform.position.y < -1f)
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
        while (time > 0)
        {
            time -= Time.deltaTime;
            enableControlls = false;
            transform.Translate(direction * movementSpeed * 2);
            yield return null;
        }
        enableControlls = true;
    }

    void Update()
    {
        ManipulateFloor();
        RegenMana();

        //force push
        //forcePushTrigger.radius = defaultRadius;
        forcePushTriggerCollider.center = Vector3.zero;
        forcePushTriggerCollider.size = defaultForcePushTriggerSize;

        //print("R2: " + Input.GetAxis("RightTrigger"));

        if (Input.GetMouseButtonDown(1) || ControllerInputDevice.GetRightTriggerDown())
        {
            //print(" MouseButtonDown(1)");
            //forcePushTrigger.radius = pushRadius;
            if (manaPoints >= pushManaCost)
            {
                forcePushTriggerCollider.size = new Vector3(forcePushTriggerCollider.size.x + 1, forcePushTriggerCollider.size.y, forcePushTriggerCollider.size.z + pushRadius);
                forcePushTriggerCollider.center = new Vector3(0, 0, -pushRadius / 2);
                manaPoints -= pushManaCost;
                //print(forcePushTriggerCollider.size);
            }
        }
        
    }

    public void AddPowerup(PowerUpObject powerUp)
    {
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

            case PowerUpType.PushForceBoost:
            case PowerUpType.RegenBoost:
            case PowerUpType.PushRangeBoost:
                if (activePowerUps.Count == 0 || !activePowerUps.Exists(p => p.name == powerUp.name))
                {
                    activePowerUps.Add(powerUp);
                }
                else
                {
                    PowerUpObject pu = activePowerUps.Find(p => p.name == powerUp.name);
                    pu.effectTime += powerUp.effectTime;
                }
                break;
        }
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
        }
    }


    public void TakeDamage(float damage)
    {
        healthPoints -= damage;

        GameManager.Instance.SetScoreMultiplier(1);
        UIManager.Instance.SetHealth(healthPoints / totalHealthPoints);
        if (healthPoints <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void FellIntoAPit()
    {
        enableControlls = false;
        Transform respawnPoint = prevHoveredObject.transform;
        transform.position = new Vector3(respawnPoint.position.x, 10, respawnPoint.position.z);
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
                string[] posArr = name.Split(',');
                if (gridHolder.GetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1])) != TileType.Occupied)
                {
                    //move the tile down
                    tileTransform.GetComponent<BaseTileBehaviour>().Drop();

                    //tileTransform.position = new Vector3(tileTransform.position.x, tileTransform.position.y - 2, tileTransform.position.z);
                    
                    /*Animator animator = tileTransform.parent.GetComponent<Animator>();
                    if (animator != null)
                        animator.SetBool("IsHole", true);
                    else
                        Debug.LogError("Could not find animator on Cube");*/

                    gridHolder.SetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1]), TileType.Pit, holeTimeToRegen);
                    manaPoints -= holeManaCost;
                }
                else
                {
                    print("Pressed on occupied tile! tile: " + name);
                }
            }
        }
    }

    /*private void DetectAndManipulateFloor()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "FloorCube")
            {
                if (prevHoveredObject != null)
                {
                    prevHoveredObject.GetComponent<Renderer>().material.color = Color.white;
                }
                prevHoveredObject = hit.transform.gameObject;
                hit.transform.GetComponent<Renderer>().material.color = Color.red;

                //make a hole
                if (Input.GetMouseButtonDown(0))
                {
                    if (manaPoints >= holeManaCost)
                    {
                        string name = hit.transform.name;
                        Debug.Log("pressed on grid cube: " + name);
                        string[] posArr = name.Split(',');
                        if (gridHolder.GetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1])) != TileType.Occupied)
                        {
                            hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 2, hit.transform.position.z);
                            gridHolder.SetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1]), TileType.Pit, holeTimeToRegen);
                            manaPoints -= holeManaCost;
                        }
                        else
                        {
                            print("Pressed on occupied tile! tile: " + name);
                        }
                    }
                }
            }
        }
    }*/

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
