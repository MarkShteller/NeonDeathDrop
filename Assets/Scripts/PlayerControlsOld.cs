using UnityEngine;
using System.Collections;
using System;

public class PlayerControlsOld : MonoBehaviour {

	float movementSpeed = 0.1f;
	private float baseMovementSpeed;
	float axisThreshhold = 0.0f;

    private float healthPoints;
    public float totalHealthPoints;

    private float manaPoints;
    public float totalManaPoints;
    public float manaRegenAmount;


    public float holeTimeToRegen;

    public float pushManaCost;
    public float holeManaCost;

    public TerrainManager gridHolder;
    public UIManager UIManager;

    private RaycastHit hit;
    private Ray ray;
    private GameObject prevHoveredObject;

    private float pushRadius = 3;
    private float defaultRadius = 0.35f;
    private Vector3 defaultForcePushTriggerSize;
    private float pushForce = 1000;
    //private SphereCollider forcePushTrigger;
    private BoxCollider forcePushTrigger;

    // Use this for initialization
    void Start () 
	{
        //forcePushTrigger = GetComponent<SphereCollider>();
        forcePushTrigger = GetComponent<BoxCollider>();
        defaultForcePushTriggerSize = forcePushTrigger.size;
		baseMovementSpeed = movementSpeed;
        manaPoints = totalManaPoints;
        healthPoints = totalHealthPoints;
    }

    // Update is called once per frame
    void FixedUpdate () 
	{
		//to prevent double speed when going diagonally 
		/*if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W) || 
		   Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S) ||
		   Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) ||
		   Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
		{ movementSpeed /= 1.5f; }
        */
		//make sure no movement input is being applied the instant the player releses the key
		//if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
		{
			if(Input.GetAxis("HorizontalMove") > axisThreshhold)
			{ transform.localPosition = new Vector3(transform.localPosition.x + movementSpeed, transform.localPosition.y ,transform.localPosition.z); }
			if(Input.GetAxis("HorizontalMove") < -axisThreshhold)
			{ transform.localPosition = new Vector3(transform.localPosition.x - movementSpeed, transform.localPosition.y ,transform.localPosition.z); }
		}
		//if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
		{
			if(Input.GetAxis("VerticalMove") > axisThreshhold)
			{ transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + movementSpeed); }
			if(Input.GetAxis("VerticalMove") < -axisThreshhold)
			{ transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z - movementSpeed); }
		}

        if (Input.GetAxis("HorizontalLook") != 0)
            print("looking horizontal " + Input.GetAxis("HorizontalLook"));
        DetectPlayerPositionOnGrid();

        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //print(mouseWorldPos);

        //this.transform.LookAt(Camera.main.ScreenToWorldPoint(Input.mousePosition), upAxis);

        /*Vector3 v3T = Input.mousePosition;
        v3T.z = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        v3T = Camera.main.ScreenToWorldPoint(v3T);
        transform.LookAt(v3T);
        */

        Vector2 mousepos = Input.mousePosition;//(Vector2)Camera.main.WorldToScreenPoint(Input.mousePosition);//new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //print(mousepos);
        Vector2 screenCenter = Camera.main.WorldToScreenPoint(this.transform.position);//new Vector2(Screen.width/2 , Screen.height/2);
        /*float angle = Vector2.Angle(screenCenter, mousepos);

        float sign = Mathf.Sign(Vector3.Dot(mousepos, screenCenter));
        float finalAngle = sign * angle;*/

        float angle = AngleBetweenTwoPoints(screenCenter, mousepos) + 180;
        //print("mouse: "+ mousepos +" screenCenter: "+ screenCenter +" angle: "+ angle);


        this.transform.rotation = Quaternion.AngleAxis(angle - 45, Vector3.up);
        movementSpeed = baseMovementSpeed;
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
            UIManager.SetHealth(healthPoints/totalHealthPoints);
        }
    }

    void Update()
    {
        DetectAndManipulateFloor();
        RegenMana();

        //force push
        //forcePushTrigger.radius = defaultRadius;
        forcePushTrigger.center = Vector3.zero;
        forcePushTrigger.size = defaultForcePushTriggerSize;
        if (Input.GetMouseButtonDown(1))
        {
            //print(" MouseButtonDown(1)");
            //forcePushTrigger.radius = pushRadius;
            if (manaPoints >= pushManaCost)
            {
                forcePushTrigger.size = new Vector3(forcePushTrigger.size.x + 1, forcePushTrigger.size.y, forcePushTrigger.size.z + pushRadius);
                forcePushTrigger.center = new Vector3(0, 0, -pushRadius / 2);
                manaPoints -= pushManaCost;
            }
        }

    }

    private void RegenMana()
    {
        if (manaPoints < totalManaPoints)
        {
            manaPoints += Time.deltaTime * manaRegenAmount;
            UIManager.SetMana(manaPoints/totalManaPoints);
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
