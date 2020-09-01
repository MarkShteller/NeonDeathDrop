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
        if (other.gameObject.CompareTag("FloorCube") || other.gameObject.CompareTag("WallCube"))
        {
            if(playerBehaviour.currHoveredObject != null)
                playerBehaviour.prevHoveredObject = playerBehaviour.currHoveredObject;
            playerBehaviour.currHoveredObject = other.transform.GetChild(0).GetChild(0).gameObject;
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("FloorCube") || other.gameObject.CompareTag("WallCube"))
        {
            other.GetComponent<Renderer>().material.color = Color.white;
        }
    }*/
}
