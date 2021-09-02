using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBehaviour : MonoBehaviour
{
    private PlayerBehaviour playerRef;
    private Vector3 targetPosition;
    private Transform playerLookDirection;

    public float heightPosition = 4f;
    public float stopDistance = 2;

    void Start()
    {
        
    }

    void Update()
    {
        if (playerRef != null && Vector3.Distance(transform.position, playerRef.transform.position) > stopDistance)
        {
            Vector3 endPos = new Vector3(playerRef.transform.position.x - stopDistance, heightPosition, playerRef.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, endPos, 0.05f);
            transform.LookAt(playerLookDirection);
        }
    }

    public void SetPlayerRef(PlayerBehaviour player)
    {
        playerRef = player;
        playerLookDirection = player.GetComponentInChildren<DetectFloorTiles>().transform;
    }
}
