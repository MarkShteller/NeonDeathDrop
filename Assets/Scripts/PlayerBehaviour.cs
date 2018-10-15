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

    private RaycastHit hit;
    private Ray ray;
    private GameObject prevHoveredObject;

    private float pushRadius = 3;
    private float defaultRadius = 0.35f;
    private Vector3 defaultForcePushTriggerSize;
    private float pushForce = 1000;
    private BoxCollider forcePushTriggerCollider;

    public TerrainManager gridHolder;
    public Transform visualsHolder;

    void Start()
    {
        forcePushTriggerCollider = GetComponent<BoxCollider>();
        defaultForcePushTriggerSize = forcePushTriggerCollider.size;
        manaPoints = totalManaPoints;
        healthPoints = totalHealthPoints;
    }

    void FixedUpdate()
    {
        float xMove = Input.GetAxis("HorizontalMove");
        float yMove = Input.GetAxis("VerticalMove");
        transform.Translate(xMove * movementSpeed, 0, yMove * movementSpeed);

        if (Input.GetAxis("HorizontalLook") != 0)
            print("looking horizontal " + Input.GetAxis("HorizontalLook"));
        DetectPlayerPositionOnGrid();

        Vector2 mousepos = Input.mousePosition;
        Vector2 screenCenter = Camera.main.WorldToScreenPoint(this.transform.position);

        float angle = AngleBetweenTwoPoints(screenCenter, mousepos) + 180;

        this.visualsHolder.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);
    }

    void Update()
    {
        DetectAndManipulateFloor();
        RegenMana();

        //force push
        //forcePushTrigger.radius = defaultRadius;
        forcePushTriggerCollider.center = Vector3.zero;
        forcePushTriggerCollider.size = defaultForcePushTriggerSize;
        if (Input.GetMouseButtonDown(1))
        {
            //print(" MouseButtonDown(1)");
            //forcePushTrigger.radius = pushRadius;
            if (manaPoints >= pushManaCost)
            {
                forcePushTriggerCollider.size = new Vector3(forcePushTriggerCollider.size.x + 1, forcePushTriggerCollider.size.y, forcePushTriggerCollider.size.z + pushRadius);
                forcePushTriggerCollider.center = new Vector3(0, 0, -pushRadius / 2);
                manaPoints -= pushManaCost;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        //print("In radius: " + other.name);
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Vector3 dir = transform.position - other.transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            enemy.ForcePush(dir, pushForce);
        }
        else print("Collition did not contain enemy.");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            healthPoints -= enemy.damage;
            UIManager.Instance.SetHealth(healthPoints / totalHealthPoints);
        }
    }

    private void RegenMana()
    {
        if (manaPoints < totalManaPoints)
        {
            manaPoints += Time.deltaTime * manaRegenAmount;
            UIManager.Instance.SetMana(manaPoints / totalManaPoints);
        }
    }

    private void DetectAndManipulateFloor()
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
                        if (gridHolder.GetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1])) != GridNode.TileType.Occupied)
                        {
                            hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 2, hit.transform.position.z);
                            gridHolder.SetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1]), GridNode.TileType.Pit, holeTimeToRegen);
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
    }

    private void DetectPlayerPositionOnGrid()
    {
        //Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.tag == "FloorCube")
            {
                string name = hit.transform.name;
                string[] posArr = name.Split(',');
                Point pPoint = GameManager.Instance.playerPointPosition;
                if (pPoint != null && gridHolder.GetGridNodeType(pPoint.x, pPoint.y) == GridNode.TileType.Occupied)
                    gridHolder.SetGridNodeType(pPoint.x, pPoint.y, GridNode.TileType.Normal);

                GameManager.Instance.playerPointPosition = new Point(int.Parse(posArr[0]), int.Parse(posArr[1]));
                //this is a temp if
                pPoint = GameManager.Instance.playerPointPosition;
                if (pPoint != null && gridHolder.GetGridNodeType(pPoint.x, pPoint.y) != GridNode.TileType.Pit)
                    //
                    gridHolder.SetGridNodeType(pPoint.x, pPoint.y, GridNode.TileType.Occupied);
            }
        }
    }

    float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return -Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
