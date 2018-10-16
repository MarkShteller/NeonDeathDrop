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
    [HideInInspector] public float pushForce = 1000;
    public BoxCollider forcePushTriggerCollider;

    public TerrainManager gridHolder;
    public Transform visualsHolder;

    void Start()
    {
        //forcePushTriggerCollider = GetComponent<BoxCollider>();
        defaultForcePushTriggerSize = forcePushTriggerCollider.size;
        manaPoints = totalManaPoints;
        healthPoints = totalHealthPoints;
    }

    void FixedUpdate()
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
        if(angle != 0)
            this.visualsHolder.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);


        DetectPlayerPositionOnGrid();

       /* Vector2 mousepos = Input.mousePosition;
        Vector2 screenCenter = Camera.main.WorldToScreenPoint(this.transform.position);

        float angle = AngleBetweenTwoPoints(screenCenter, mousepos) + 180;

        this.visualsHolder.rotation = Quaternion.AngleAxis(angle - 90, Vector3.up);
        */
    }

    void Update()
    {
        DetectAndManipulateFloor();
        RegenMana();

        //force push
        //forcePushTrigger.radius = defaultRadius;
        forcePushTriggerCollider.center = Vector3.zero;
        forcePushTriggerCollider.size = defaultForcePushTriggerSize;

        //print("R2: " + Input.GetAxis("RightTrigger"));

        if (Input.GetMouseButtonDown(1) || Input.GetAxis("RightTrigger") == 1)
        {
            //print(" MouseButtonDown(1)");
            //forcePushTrigger.radius = pushRadius;
            if (manaPoints >= pushManaCost)
            {
                forcePushTriggerCollider.size = new Vector3(forcePushTriggerCollider.size.x + 1, forcePushTriggerCollider.size.y, forcePushTriggerCollider.size.z + pushRadius);
                forcePushTriggerCollider.center = new Vector3(0, 0, -pushRadius / 2);
                manaPoints -= pushManaCost;
                print(forcePushTriggerCollider.size);
            }
        }

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
