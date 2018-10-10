using UnityEngine;
using System.Collections;

public class FloorManipulator : MonoBehaviour {

    public TerrainManager gridHolder;

    private RaycastHit hit;
    private Ray ray;
    private GameObject prevHoveredObject;

    private float pushRadius = 3;
    private float pushForce = 10;
    private SphereCollider forcePushTrigger;
    //public GameObject forcePushTriggerObject;

    // Use this for initialization
    void Start ()
    {
        forcePushTrigger = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update ()
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
                if(Input.GetMouseButtonDown(0))
                {
                    Debug.Log("pressed on grid cube.");
                    hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 2, hit.transform.position.z);
                    string name = hit.transform.name;
                    string[] posArr = name.Split(',');
                    gridHolder.SetGridNodeType(int.Parse(posArr[0]), int.Parse(posArr[1]), GridNode.TileType.Pit);
                }
            }
        }

        //force push
        if (Input.GetMouseButtonDown(1))
        {
            print(" MouseButtonDown(1)");
            float tempRadius = forcePushTrigger.radius;
            forcePushTrigger.radius = pushRadius;
        }
        
	}

    void OnTriggerEnter(Collider other)
    {
        print("In radius: " + other.name);
    }
}
