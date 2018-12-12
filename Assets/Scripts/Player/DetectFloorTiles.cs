using UnityEngine;

public class DetectFloorTiles : MonoBehaviour
{
    public PlayerBehaviour playerBehaviour;

    /*private void Awake()
    {
        playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }*/

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FloorCube")
        {
            if(playerBehaviour.currHoveredObject != null)
                playerBehaviour.prevHoveredObject = playerBehaviour.currHoveredObject;
            playerBehaviour.currHoveredObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "FloorCube")
        {
            other.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
